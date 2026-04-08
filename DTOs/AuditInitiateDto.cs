namespace _5sAudit.DTOs
{
    public class AuditInitiateDto
    {
        public int AuditorId { get; set; }
        public int PlantId { get; set; }
        public int? DeptId { get; set; }
        public string AuditType { get; set; }
        public string Date { get; set; } // Received as YYYY-MM-DD
        public string Time { get; set; } // Received as HH:mm
        public int CompanyId { get; set; }
        public int CreatedBy { get; set; }
    }
}
