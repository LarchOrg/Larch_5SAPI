using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _5sAudit.Data;
using _5sAudit.Models;

namespace _5sAudit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuestionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Question
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FsaChecklistMst>>> GetQuestions()
        {
            // Using Stored Procedure to GET questions
            return await _context.FsaChecklistMsts.FromSqlRaw("EXEC sp_GetAllQuestions").ToListAsync();
        }

        // GET: api/Question/5
        [HttpGet("{id}")]
        public async Task<ActionResult<FsaChecklistMst>> GetQuestion(int id)
        {
            var question = await _context.FsaChecklistMsts.FindAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return question;
        }

        // POST: api/Question
        [HttpPost]
        public async Task<ActionResult<FsaChecklistMst>> PostQuestion(FsaChecklistMst question)
        {
            question.CreatedDt = DateTime.Now;
            question.ModifiedDt = DateTime.Now;
            if (string.IsNullOrEmpty(question.Status))
            {
                question.Status = "A";
            }

            _context.FsaChecklistMsts.Add(question);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, question);
        }

        // PUT: api/Question/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestion(int id, FsaChecklistMst question)
        {
            // 1. Check if the URL ID matches the body ID
            if (id != question.Id)
            {
                return BadRequest("Question ID mismatch.");
            }

            // 2. Find the existing record in the database
            var existingQuestion = await _context.FsaChecklistMsts.FindAsync(id);
            if (existingQuestion == null)
            {
                return NotFound($"Question with ID {id} not found.");
            }

            // 3. Update only the editable fields
            existingQuestion.AuditType = question.AuditType;
            existingQuestion.Category = question.Category;
            existingQuestion.Question = question.Question;
            existingQuestion.Status = string.IsNullOrEmpty(question.Status) ? "A" : question.Status;

            // 4. Update the modified timestamp
            existingQuestion.ModifiedDt = DateTime.Now;

            // 5. Mark as modified and save
            _context.Entry(existingQuestion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuestionExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Returns 204 Success
        }

        // Helper method to check existence
        private bool QuestionExists(int id)
        {
            return _context.FsaChecklistMsts.Any(e => e.Id == id);
        }

        // DELETE: api/Question/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.FsaChecklistMsts.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }

            _context.FsaChecklistMsts.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
