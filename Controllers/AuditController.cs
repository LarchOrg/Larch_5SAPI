using _5sAudit.Data;
using _5sAudit.Models;
using _5sAudit.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;

namespace _5sAudit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AuditController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // 1. GET ALL AUDITS (Enhanced with Score Summary Data)
        [HttpGet("List")]
        public async Task<IActionResult> GetAuditList()
        {
            try
            {
                // 1. Get User Claims from Token
                var currentUserIdClaim = User.FindFirst("UserId")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
                
                int userId = string.IsNullOrEmpty(currentUserIdClaim) ? 0 : int.Parse(currentUserIdClaim);
                var currentUser = await _context.FsaUsers.FindAsync(userId);

                // 2. Define the Query with Joins
                var query = from a in _context.FsaAuditInits
                            join u in _context.FsaUsers on a.AuditorId equals u.Id
                            join d in _context.FsaDepartments on a.DeptId equals d.CdIId into deptJoin
                            from d in deptJoin.DefaultIfEmpty()
                            join s in _context.FsaScoreSummaries on a.AuditId equals s.audit_id into scoreJoin
                            from s in scoreJoin.DefaultIfEmpty()
                            select new
                            {
                                id = a.AuditId,
                                auditorId = a.AuditorId,
                                companyId = a.CompanyId,
                                plantId = a.PlantId, // add plantId in select
                                auditor = (u.Firstname + " " + (u.Lastname ?? "")).Trim(),
                                auditType = a.AuditType,
                                department = d != null ? d.CdVDeptName : "N/A",
                                scheduledDate = a.ScheduledDate,
                                title = a.AuditType,
                                time = a.ScheduledTime,
                                status = a.Status ?? "Scheduled",
                                progress = a.Status == "Completed" ? 100 : 0,
                                totalScore = s != null ? (double)s.total_score : 0,
                                maxScore = s != null ? (double)s.max_possible_score : 0,
                                percentage = s != null ? (double)s.percentage : 0
                            };

                // 3. Apply Multi-Tenant Security Filtering

                if (currentUser != null && currentUserRole?.ToLower() != "super_admin")
                {
                    // Filter by Company
                    query = query.Where(x => x.companyId == currentUser.CompanyId);

                    // Filter by Plant if the user is associated with a specific plant
                    if (currentUser.PlantId.HasValue && currentUser.PlantId > 0)
                    {
                        query = query.Where(x => x.plantId == currentUser.PlantId);
                    }

                    // CASE A: If user is an Auditor, they only see audits ASSIGNED to them
                    if (currentUserRole?.ToLower() == "auditor")
                    {
                        query = query.Where(x => x.auditorId == userId);
                    }
                }
                // CASE C: If user is SuperAdmin, we do NOT add any filter (they see everything)

                // 4. Execute the Query
                var rawData = await query.ToListAsync();

                // 5. Final Formatting for React
                var formattedData = rawData.Select(a => new {
                    a.id,
                    a.auditorId,
                    a.auditor,
                    a.auditType,
                    a.department,
                    scheduledDate = a.scheduledDate.ToString("yyyy-MM-dd"),
                    a.title,
                    time = a.time.ToString(@"hh\:mm"),
                    a.status,
                    dueDate = a.scheduledDate.ToString("dd MMM"),
                    a.progress,
                    a.totalScore,
                    a.maxScore,
                    a.percentage
                });

                return Ok(formattedData);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. GET DETAILED RESULTS (For the View Report Page)
        [HttpGet("Results/{auditId}")]
        public async Task<IActionResult> GetAuditResults(int auditId)
        {
            try
            {
                var rawResults = await (from r in _context.FsaAuditResponses
                                        join q in _context.FsaChecklistMsts on r.checklist_id equals q.Id
                                        where r.audit_id == auditId
                                        select new
                                        {
                                            questionText = q.Question,
                                            category = q.Category,
                                            score = r.score,
                                            description = r.description,
                                            comments = r.comments,
                                            imageUrlsRaw = r.image_url // String format: "url1;url2"
                                        }).ToListAsync();

                if (!rawResults.Any()) return NotFound(new { message = "No records found for this audit." });

                // Transform: Split image string into a clean JSON array for React
                var formattedResults = rawResults.Select(r => new
                {
                    r.questionText,
                    r.category,
                    r.score,
                    r.description,
                    r.comments,
                    imageUrls = string.IsNullOrEmpty(r.imageUrlsRaw)
                                ? new string[0]
                                : r.imageUrlsRaw.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries)
                });

                return Ok(formattedResults);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 3. SUBMIT EVALUATION
        [HttpPost("SubmitEvaluation")]
        public async Task<IActionResult> SubmitEvaluation([FromBody] AuditSubmissionDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Save Summary (Percentage is decimal)
                var summary = new FsaScoreSummary
                {
                    audit_id = dto.AuditId,
                    total_score = dto.TotalScore,
                    max_possible_score = (int)dto.MaxPossibleScore,
                    percentage = dto.Percentage, // Handled as decimal
                    CreatedDt = DateTime.Now
                };
                _context.FsaScoreSummaries.Add(summary);

                // 2. Save Individual Responses
                foreach (var resp in dto.Responses)
                {
                    _context.FsaAuditResponses.Add(new FsaAuditResponse
                    {
                        audit_id = dto.AuditId,
                        checklist_id = resp.ChecklistId,
                        score = resp.Score,
                        description = resp.Description,
                        comments = resp.Comments,
                        image_url = resp.ImageUrls != null ? string.Join(";", resp.ImageUrls) : "",
                        CreatedDt = DateTime.Now
                    });
                }

                // 3. Update Audit Status
                var auditRecord = await _context.FsaAuditInits.FindAsync(dto.AuditId);
                if (auditRecord != null)
                {
                    auditRecord.Status = "Completed";
                    _context.FsaAuditInits.Update(auditRecord);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { message = "Audit submitted successfully" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // 4. GET CHECKLIST QUESTIONS
        [HttpGet("Checklist/{auditType}")]
        public async Task<IActionResult> GetChecklist(string auditType)
        {
            try
            {
                string searchKey = auditType.Split(' ')[0];
                var questions = await _context.FsaChecklistMsts
                    .Where(q => q.AuditType.ToLower().Contains(searchKey.ToLower()) && q.Status.ToLower() == "a")
                    .Select(q => new ChecklistQuestionDto
                    {
                        Id = q.Id,
                        Question = q.Question,
                        Category = q.Category,
                        ClauseNo = q.ClauseNo
                    })
                    .ToListAsync();

                return Ok(questions);
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }

        // 5. CHUNK UPLOAD
        [HttpPost("UploadChunk")]
        public async Task<IActionResult> UploadChunk([FromForm] FileChunkDto model)
        {
            if (model.Chunk == null || string.IsNullOrEmpty(model.FileUid))
                return BadRequest("Invalid upload data.");

            string tempRoot = Path.Combine(_environment.ContentRootPath, "temp_audit_chunks", model.FileUid);
            if (!Directory.Exists(tempRoot)) Directory.CreateDirectory(tempRoot);

            string chunkPath = Path.Combine(tempRoot, model.ChunkNumber.ToString("D5"));
            using (var stream = new FileStream(chunkPath, FileMode.Create))
            {
                await model.Chunk.CopyToAsync(stream);
            }

            if (model.ChunkNumber == model.TotalChunks - 1)
            {
                string targetSubFolder = "audit_evidence";
                string targetFolder = Path.Combine(_environment.ContentRootPath, "wwwroot", targetSubFolder);
                if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);

                string fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.FileName)}";
                string finalPath = Path.Combine(targetFolder, fileName);

                var chunkFiles = Directory.GetFiles(tempRoot).OrderBy(f => f).ToList();
                using (var finalStream = new FileStream(finalPath, FileMode.Create))
                {
                    foreach (var chunkFile in chunkFiles)
                    {
                        using (var chunkStream = new FileStream(chunkFile, FileMode.Open))
                        {
                            await chunkStream.CopyToAsync(finalStream);
                        }
                    }
                }
                try { Directory.Delete(tempRoot, true); } catch { }
                return Ok(new { filePath = $"/{targetSubFolder}/{fileName}" });
            }

            return Ok(new { status = "Chunk uploaded" });
        }

        // 6. INITIATE AUDIT
        [HttpPost("Initiate")]
        public async Task<IActionResult> InitiateAudit([FromBody] AuditInitiateDto dto)
        {
            try
            {
                var newAudit = new FsaAuditInit
                {
                    AuditorId = dto.AuditorId,
                    PlantId = dto.PlantId,
                    DeptId = dto.DeptId,
                    AuditType = dto.AuditType,
                    ScheduledDate = DateTime.Parse(dto.Date),
                    ScheduledTime = TimeSpan.Parse(dto.Time),
                    CompanyId = dto.CompanyId,
                    CreatedBy = dto.CreatedBy,
                    Status = "Scheduled"
                };
                _context.FsaAuditInits.Add(newAudit);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, auditId = newAudit.AuditId });
            }
            catch (Exception ex) { return BadRequest(new { message = ex.Message }); }
        }
        // 6. UPDATE STATUS TO IN PROGRESS
        [HttpPost("StartAudit/{auditId}")]
        public async Task<IActionResult> StartAudit(int auditId)
        {
            try
            {
                var auditRecord = await _context.FsaAuditInits.FindAsync(auditId);

                if (auditRecord == null)
                    return NotFound(new { message = "Audit record not found" });

                // Only update if it's currently Scheduled or Upcoming 
                // to avoid overwriting "Completed" by accident
                if (auditRecord.Status == "Scheduled" || string.IsNullOrEmpty(auditRecord.Status))
                {
                    auditRecord.Status = "In Progress";
                    _context.FsaAuditInits.Update(auditRecord);
                    await _context.SaveChangesAsync();
                }

                return Ok(new { message = "Audit status updated to In Progress", status = "In Progress" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        // 7. CRUD OPERATIONS
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuditById(int id)
        {
            var audit = await _context.FsaAuditInits.FindAsync(id);
            if (audit == null) return NotFound();
            return Ok(audit);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAudit(int id, [FromBody] AuditInitiateDto dto)
        {
            var audit = await _context.FsaAuditInits.FindAsync(id);
            if (audit == null) return NotFound();
            audit.AuditorId = dto.AuditorId;
            audit.AuditType = dto.AuditType;
            audit.ScheduledDate = DateTime.Parse(dto.Date);
            audit.ScheduledTime = TimeSpan.Parse(dto.Time);
            _context.FsaAuditInits.Update(audit);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAudit(int id)
        {
            var audit = await _context.FsaAuditInits.FindAsync(id);
            if (audit == null) return NotFound();
            _context.FsaAuditInits.Remove(audit);
            await _context.SaveChangesAsync();
            return Ok(new { success = true });
        }
    }
}