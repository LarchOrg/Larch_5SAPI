namespace _5sAudit.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Role { get; set; } // "super_admin", "admin", "auditor", etc.
        public string FullName { get; set; }
        public int Id { get; set; }
        public int CompanyId { get; set; }
    }
}
