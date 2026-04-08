using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _5sAudit.Models
{
    [Table("fsa_audit_init")]
    public class FsaAuditInit
    {
        [Key]
        [Column("audit_id")]
        public int AuditId { get; set; }
        [Column("auditor_id")]
        public int AuditorId { get; set; }
        [Column("plant_id")]
        public int PlantId { get; set; }
        [Column("dept_id")]
        public int? DeptId { get; set; }
        [Column("audit_type")]
        public string AuditType { get; set; }
        [Column("scheduled_date")]
        public DateTime ScheduledDate { get; set; }
        [Column("scheduled_time")]
        public TimeSpan ScheduledTime { get; set; }
        [Column("company_id")]
        public int CompanyId { get; set; }
        [Column("created_by")]
        public int CreatedBy { get; set; }
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(20)]
        public string Status { get; set; } = "Scheduled";
    }
}
