using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _5sAudit.Models
{
    [Table("audit_question_mst")]
    public class Question
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("Question")]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;

        [StringLength(100)]
        public string? AuditType { get; set; }

        [StringLength(50)]
        public string? ClauseNo { get; set; }

        public bool? Status { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        public DateTime? CreatedDt { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }
    }
}
