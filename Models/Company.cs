using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _5sAudit.Models
{
    [Table("fsa_company_mst")]
    public class Company
    {
        [Key]
        [Column("cp_iId")]
        public int Id { get; set; }

        [Column("cp_vCompanyName")]
        [MaxLength(255)]
        public string? CompanyName { get; set; }

        [Column("cp_cStatus")]
        [MaxLength(10)]
        public string? Status { get; set; }

        [Column("cp_iCreatedBy")]
        public int? CreatedBy { get; set; }

        [Column("cp_dCreatedDt")]
        public DateTime? CreatedDt { get; set; }

        [Column("cp_iModifiedBy")]
        public int? ModifiedBy { get; set; }

        [Column("cp_dModifiedDt")]
        public DateTime? ModifiedDt { get; set; }
    }
}
