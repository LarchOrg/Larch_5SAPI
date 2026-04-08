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

        [HttpGet("roles")]
        public async Task<ActionResult<IEnumerable<FsaRole>>> GetRoles()
        {
            return await _context.FsaRoleMsts.ToListAsync();
        }

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var data = await _context.FsaDepartments
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
        public async Task<IActionResult> GetPlants()
        {
            var plants = await _context.Plants
                .Select(p => new PlantDto
                {
                    Id = p.Id,
                    PlantName = p.PlantName
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
                // Set default values for the model
                dept.CdVStatus = "Active";
                dept.CdDCreatedDt = DateTime.Now;

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
