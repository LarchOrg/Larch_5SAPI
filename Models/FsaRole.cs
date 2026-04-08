using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _5sAudit.Models
{
    [Table("fsa_role_mst")]
    public class FsaRole
    {
        [Key]
        [Column("RoleId")]
        public int RoleId { get; set; }

        [Column("RoleName")]
        [Required]
        public string RoleName { get; set; } = string.Empty;

        [Column("Status")]
        public string? Status { get; set; }
    }
}