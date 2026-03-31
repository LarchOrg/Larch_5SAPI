namespace _5sAudit.DTOs
{
    public class UserDto
    {
        public string Firstname { get; set; } = string.Empty;
        public string? Lastname { get; set; }
        public string? Email { get; set; } // This will map to EmailId
        public string Password { get; set; } = string.Empty;
        public string? MobileNo { get; set; }
        public int? RoleId { get; set; }
        public int CompanyId { get; set; }
        public int? DepartmentId { get; set; }
        public int? PlantId { get; set; }
        public int? DeptId { get; set; }
        public int? Experience { get; set; }
        public string? Dob { get; set; }
        public string? Doj { get; set; }
    }
}
