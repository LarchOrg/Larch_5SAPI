using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _5sAudit.Models
{


    [Table("fsa_audit_responses")]
    public class FsaAuditResponse
    {
        [Key]
        public int id { get; set; }

        public int audit_id { get; set; }

        public int checklist_id { get; set; }

        public int score { get; set; }

        public string description { get; set; }

        public string comments { get; set; }

        public string image_url { get; set; }

        public DateTime? CreatedDt { get; set; } = DateTime.Now;
        public DateTime? ModifiedDt { get; set; }
    }
}