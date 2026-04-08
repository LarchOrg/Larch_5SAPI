namespace _5sAudit.DTOs
{
    public class ChecklistQuestionDto
    {
        public int Id { get; set; }
        public string Question { get; set; } // Mapped from 'Question'

        public string AuditType { get; set; }
        public string Category { get; set; }
        public string? ClauseNo { get; set; }
    }
}