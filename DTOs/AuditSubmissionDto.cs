namespace _5sAudit.DTOs
{
    public class AuditSubmissionDto
    {
        public int AuditId { get; set; }
        public decimal TotalScore { get; set; }
        public decimal MaxPossibleScore { get; set; }
        public decimal Percentage { get; set; }
        public List<AuditResponseDto> Responses { get; set; }
    }
}

