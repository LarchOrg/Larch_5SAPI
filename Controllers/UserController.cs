using _5sAudit.Data;
using _5sAudit.DTOs;
using _5sAudit.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace _5sAudit.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int CurrentUserId => int.Parse(User.FindFirst("UserId")?.Value ?? "0");
        private int CurrentCompanyId => int.Parse(User.FindFirst("CompanyId")?.Value ?? "0");

        [HttpPost("save")]
        public async Task<IActionResult> SaveUser([FromBody] UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest("User data is null");
            }

            try
            {
                // Simple validation
                if (string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.Password))
                {
                    return BadRequest("Email and Password are required");
                }

                // Check if user already exists
                var existingUser = await _context.FsaUsers.FirstOrDefaultAsync(u => u.EmailId == userDto.Email);
                if (existingUser != null)
                {
                    return Conflict("User with this email already exists");
                }
                var userCompanyClaim = User.FindFirst("CompanyId")?.Value;
                int targetCompanyId = !string.IsNullOrEmpty(userCompanyClaim)
                    ? int.Parse(userCompanyClaim)
                    : userDto.CompanyId;
                // Map DTO to Entity
                var user = new FsaUser
                {
                    Firstname = userDto.Firstname,
                    Lastname = userDto.Lastname,
                    EmailId = userDto.Email,
                    Password = userDto.Password,
                    MobileNo = userDto.MobileNo,
                    RoleId = userDto.RoleId,
                    CompanyId = targetCompanyId,
                    CreatedBy = CurrentUserId, 
                    PlantId = userDto.PlantId,
                    DeptId = userDto.DeptId,
                    Experience = userDto.Experience,
                    Dob = userDto.Dob,
                    Doj = userDto.Doj,
                    Status = "A", // Default to Active
                    CreatedDt = DateTime.Now,
                    ModifiedDt = DateTime.Now
                };

                _context.FsaUsers.Add(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User saved successfully", userId = user.Id });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FsaUser>>> GetUsers()
        {
            var companyId = CurrentCompanyId;
            if (companyId == 0) return BadRequest("Invalid Company ID");

            var currentUserRole = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();
            var currentUser = await _context.FsaUsers.FindAsync(CurrentUserId);

            // 2. Start query filtered by Company
            var query = _context.FsaUsers.Where(u => u.CompanyId == companyId);

            // 3. Apply Plant filter if not a Super Admin or Company Admin
            // (Assuming 'admin' or 'super_admin' can see the whole company)
            if (currentUser != null)
            {
                // Only SUPER ADMIN can see all plants
                if (currentUserRole != "super_admin")
                {
                    if (currentUser.PlantId.HasValue && currentUser.PlantId > 0)
                    {
                        query = query.Where(u => u.PlantId == currentUser.PlantId);
                    }
                    else
                    {
                        // Optional safety: if no plant assigned → return nothing
                        query = query.Where(u => false);
                    }
                }
            }

            return await query.ToListAsync();
        }

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<FsaRole>>> GetRoles()
        {
            return await _context.FsaRoleMsts.ToListAsync();
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var companyId = CurrentCompanyId;
            if (companyId == 0) return BadRequest("Invalid Company ID");

            // Filter departments by CompanyId to ensure users only see their own company's units
            var data = await _context.FsaDepartments
                .Where(d => d.CdICompanyId == companyId) // Ensure this column exists
                .Select(d => new DepartmentDTO
                {
                    Id = d.CdIId,
                    DeptName = d.CdVDeptName,
                    PlantId = d.CdIPlantId
                })
                .ToListAsync();

            return Ok(data);
        }

        [HttpGet("plants")]
        public async Task<ActionResult> GetPlants()
        {
            var currentUserRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            var currentUser = await _context.FsaUsers.FindAsync(CurrentUserId);

            var query = _context.Plants.Where(p => p.Status == "Active" || p.Status == "A");

            if (currentUser != null && currentUserRole?.ToLower() != "super_admin")
            {
                query = query.Where(p => p.CompanyId == currentUser.CompanyId);

                if (currentUser.PlantId.HasValue && currentUser.PlantId > 0)
                {
                    query = query.Where(p => p.Id == currentUser.PlantId);
                }
            }

            var plants = await query
                .Select(p => new {
                    id = p.Id,
                    plantName = p.PlantName,
                    companyId = p.CompanyId
                })
                .ToListAsync();

            return Ok(plants);
        }


        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
        {
            try
            {
                var user = await _context.FsaUsers.FindAsync(id);
                if (user == null) return NotFound("User not found");

                // Update properties
                user.Firstname = userDto.Firstname;
                user.Lastname = userDto.Lastname;
                user.MobileNo = userDto.MobileNo;
                user.RoleId = userDto.RoleId;
                user.PlantId = userDto.PlantId;
                user.DeptId = userDto.DeptId;
                user.ModifiedDt = DateTime.Now;

                // Only update password if a new one is provided
                if (!string.IsNullOrEmpty(userDto.Password))
                {
                    user.Password = userDto.Password;
                }

                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("SaveDepartment")]
        public async Task<IActionResult> SaveDepartment([FromBody] FsaDepartment dept)
        {
            if (dept == null || string.IsNullOrEmpty(dept.CdVDeptName))
                return BadRequest(new { message = "Department name is required" });

            try
            {
                // 1. Get CompanyId from the logged-in user's claims (Best Practice)
                var userCompanyClaim = User.FindFirst("CompanyId")?.Value;

                if (!string.IsNullOrEmpty(userCompanyClaim))
                {
                    dept.CdICompanyId = int.Parse(userCompanyClaim);
                }
                else if (dept.CdIPlantId > 0)
                {
                    // 2. Fallback: Lookup CompanyId from the Plant table if not in claims
                    var plant = await _context.Plants
                        .FirstOrDefaultAsync(p => p.Id == dept.CdIPlantId);

                    if (plant != null)
                    {
                        dept.CdICompanyId = plant.CompanyId; // Assuming your Plant model has CompanyId
                    }
                }

                // Set default values for the model
                dept.CdVStatus = "Active";
                dept.CdDCreatedDt = DateTime.Now;
                dept.CdICreatedBy = CurrentUserId;
                _context.FsaDepartments.Add(dept);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Department saved successfully", id = dept.CdIId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
