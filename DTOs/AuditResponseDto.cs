namespace _5sAudit.DTOs
{
    public class AuditResponseDto
    {
        public int ChecklistId { get; set; }
        public int Score { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public List<string> ImageUrls { get; set; }
    }
}
