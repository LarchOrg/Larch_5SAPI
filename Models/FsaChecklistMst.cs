using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _5sAudit.Models
{
    [Table("fsa_checklist_mst")]
    public class FsaChecklistMst
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Question { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; }

        [Required]
        [StringLength(50)]
        public string AuditType { get; set; }

        [StringLength(50)]
        public string? ClauseNo { get; set; } // Nullable as per your SQL change

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "A";

        public int CreatedBy { get; set; }

        public DateTime CreatedDt { get; set; } = DateTime.Now;

        public int? ModifiedBy { get; set; }

        public DateTime? ModifiedDt { get; set; }
    }
}