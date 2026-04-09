using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _5sAudit.Data;
using _5sAudit.Models;
using _5sAudit.DTOs;

namespace _5sAudit.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("FsaUsers")]
        public async Task<ActionResult> GetUsers()
        {
            var users = await (from u in _context.FsaUsers
                               join c in _context.Companies on u.CompanyId equals c.Id into joinedCompany
                               from c in joinedCompany.DefaultIfEmpty()
                               join p in _context.Plants on u.PlantId equals p.Id into joinedPlant
                               from p in joinedPlant.DefaultIfEmpty()
                               select new
                               {
                                   u.Id,
                                   u.Firstname,
                                   u.Lastname,
                                   Email = u.EmailId,
                                   u.Status,
                                   u.CompanyId,
                                   u.PlantId,
                                   u.RoleId,
                                   u.MobileNo,
                                   u.Experience,
                                   u.Dob, 
                                   u.Doj,
                                   CompanyName = c != null ? c.CompanyName : "N/A",
                                   PlantName = p != null ? p.PlantName : "N/A"
                               }).ToListAsync();

            return Ok(users);
        }
        [HttpPatch("FsaUsers/{id}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(int id)
        {
            var user = await _context.FsaUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Toggle logic: If 'A' set to 'I', otherwise set to 'A'
            user.Status = (user.Status == "A") ? "I" : "A";
            user.ModifiedDt = DateTime.Now;

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Status updated successfully",
                newStatus = user.Status
            });
        }
        [HttpPost("FsaUsers")]
        public async Task<ActionResult> PostUser(UserDto userDto)
        {
            var user = new FsaUser
            {
                Firstname = userDto.Firstname,
                Lastname = userDto.Lastname,
                EmailId = userDto.Email,
                Password = userDto.Password,
                MobileNo = userDto.MobileNo,
                Status = "A",
                CreatedBy = 1,
                CreatedDt = DateTime.Now,
                // FIX: ModifiedDt is required in your FsaUser model (non-nullable)
                ModifiedDt = DateTime.Now,
                RoleId = userDto.RoleId,
                CompanyId = userDto.CompanyId,
                PlantId = userDto.PlantId,
                DeptId = userDto.DeptId,
                Experience = userDto.Experience,

                // Ensure these are mapped so they don't default to 0001-01-01
                Dob = userDto.Dob,
                Doj = userDto.Doj,
            };

            _context.FsaUsers.Add(user);

            if (userDto.PlantId.HasValue && userDto.PlantId.Value > 0)
            {
                var plant = await _context.Plants.FindAsync(userDto.PlantId.Value);
                if (plant != null)
                {
                    // Update the plant to belong to the selected company
                    plant.CompanyId = userDto.CompanyId;
                    plant.ModifiedDt = DateTime.Now;
                    _context.Entry(plant).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "User created successfully", id = user.Id });
        }

        [HttpPut("FsaUsers/{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserDto userDto)
        {
            var user = await _context.FsaUsers.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            // Map updated fields from DTO to the Entity
            user.Firstname = userDto.Firstname;
            user.Lastname = userDto.Lastname;
            user.EmailId = userDto.Email; // Be careful if you don't want users changing emails
            user.MobileNo = userDto.MobileNo;
            user.RoleId = userDto.RoleId;
            user.CompanyId = userDto.CompanyId;
            user.PlantId = userDto.PlantId;
            user.DeptId = userDto.DeptId;
            user.Experience = userDto.Experience;
            user.Dob = userDto.Dob;
            user.Doj = userDto.Doj;

            // Update metadata
            user.ModifiedDt = DateTime.Now;

            // Only update password if a new one is provided
            if (!string.IsNullOrWhiteSpace(userDto.Password))
            {
                user.Password = userDto.Password;
            }

            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = "User updated successfully" });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id)) return NotFound();
                else throw;
            }
        }

        // Helper method used by the Put logic
        private bool UserExists(int id)
        {
            return _context.FsaUsers.Any(e => e.Id == id);
        }

        [HttpPost("Companies")]
        public async Task<ActionResult> CreateCompany(Company company)
        {
            company.Status = "A";
            company.CreatedDt = DateTime.Now;
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();
            return Ok(company);
        }

        [HttpPost("Plants")]
        public async Task<ActionResult> CreatePlant(Plant plant)
        {
            plant.Status = "A";
            plant.CreatedDt = DateTime.Now;
            _context.Plants.Add(plant);
            await _context.SaveChangesAsync();
            return Ok(plant);
        }

        [HttpGet("Companies")]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies() => await _context.Companies.ToListAsync();

        [HttpGet("Plants")]
        public async Task<ActionResult<IEnumerable<Plant>>> GetPlants() => await _context.Plants.ToListAsync();

        [HttpGet("Departments")]
        public async Task<ActionResult<IEnumerable<DepartmentDTO>>> GetDepartments()
        {
            // Make sure your DbContext has: public DbSet<FsaDepartment> FsaDepartments { get; set; }
            return await _context.FsaDepartments
                .Where(d => d.CdVStatus == "Active")
                .Select(d => new DepartmentDTO
                {
                    Id = d.CdIId,
                    DeptName = d.CdVDeptName,
                    PlantId = d.CdIPlantId
                }).ToListAsync();
        }

        [HttpGet("Auditors")]
        public async Task<ActionResult<IEnumerable<object>>> GetAuditors()
        {
            try
            {
                var auditors = await (from u in _context.FsaUsers
                                      join r in _context.FsaRoleMsts on u.RoleId equals r.RoleId
                                      // Filter for Active users AND the specific Auditor role
                                      where u.Status == "A" && r.RoleName == "Auditor"
                                      select new
                                      {
                                          id = u.Id,
                                          fullName = (u.Firstname + " " + (u.Lastname ?? "")).Trim(),
                                          email = u.EmailId,
                                          role = r.RoleName
                                      }).ToListAsync();

                return Ok(auditors);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}