using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _5sAudit.Models
{

    public class FsaDepartment
    {
        [Key]
        [Column("cd_iId")]
        public int CdIId { get; set; }

        [Column("cd_iPlantId")]
        public int CdIPlantId { get; set; }

        [Column("cd_iCompanyId")]
        public int CdICompanyId { get; set; }

        [Column("cd_vDeptName")]
        public required string CdVDeptName { get; set; }

        [Column("cd_vStatus")]
        public string CdVStatus { get; set; } = "Active";

        [Column("cd_iCreatedBy")]
        public int CdICreatedBy { get; set; }

        [Column("cd_dCreatedDt")]
        public DateTime CdDCreatedDt { get; set; } = DateTime.Now;

        [Column("cd_iModifiedBy")]
        public int? CdIModifiedBy { get; set; }

        [Column("cd_dModifiedDt")]
        public DateTime? CdDModifiedDt { get; set; }
    }
}