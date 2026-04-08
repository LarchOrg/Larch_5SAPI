
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace _5sAudit.Models
{

    [Table("fsa_score_summary")]
    public class FsaScoreSummary
    {
        [Key]
        public int id { get; set; }

        public int audit_id { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal total_score { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal max_possible_score { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal percentage { get; set; }

        public DateTime? CreatedDt { get; set; } = DateTime.Now;
        public DateTime? ModifiedDt { get; set; }
    }
}
