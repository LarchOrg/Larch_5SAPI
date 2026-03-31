using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _5sAudit.Data;
using _5sAudit.Models;
using _5sAudit.DTOs;

namespace _5sAudit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

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

                // Map DTO to Entity
                var user = new FsaUser
                {
                    Firstname = userDto.Firstname,
                    Lastname = userDto.Lastname,
                    EmailId = userDto.Email,
                    Password = userDto.Password,
                    MobileNo = userDto.MobileNo,
                    RoleId = userDto.RoleId,
                    CompanyId = userDto.CompanyId,
               
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
            return await _context.FsaUsers.ToListAsync();
        }
    }
}
