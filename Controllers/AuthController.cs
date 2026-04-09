using _5sAudit.Data;
using _5sAudit.Models;
using _5sAudit.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace _5sAudit.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _context;

        public AuthController(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            // 1. Fetch user. Use FirstOrDefaultAsync to prevent blocking threads.
            var user = await _context.FsaUsers
                .FirstOrDefaultAsync(u => u.EmailId == request.EmailId);

            // 2. Validate
            if (user == null || user.Password != request.Password)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // 3. Map RoleId (based on your dbo.fsa_role_mst)
            string roleName = user.RoleId switch
            {
                1 => "super_admin",
                2 => "admin",
                3 => "auditor",
                4 => "dept_head",
                _ => "auditor"
            };

            // 4. Generate JWT
            var token = GenerateJwtToken(user, roleName);

            return Ok(new LoginResponseDto
            {
                Token = token,
                Role = roleName,
                FullName = $"{user.Firstname} {user.Lastname}",
                Id=user.Id
            });
        }

        private string GenerateJwtToken(FsaUser user, string role)
        {
            var jwtKey = _config["Jwt:Key"];
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // IMPORTANT: Convert all IDs to strings for Claims
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, user.EmailId ?? ""),
                new Claim(ClaimTypes.Role, role),

                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("UserId", user.Id.ToString()),
                new Claim("CompanyId", user.CompanyId.ToString()) // Explicitly calling .ToString()
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(8),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}