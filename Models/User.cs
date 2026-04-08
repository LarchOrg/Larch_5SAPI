using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _5sAudit.Models
{
    [Table("fsa_users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Firstname { get; set; }

        [MaxLength(100)]
        public string? Lastname { get; set; }

        [MaxLength(100)]
        public string? EmailId { get; set; }

        [MaxLength(255)]
        public string? Password { get; set; }

        [MaxLength(20)]
        public string? MobileNo { get; set; }

        [MaxLength(10)]
        public string? Status { get; set; }

        public int? CreatedBy { get; set; }
        
        [Column(TypeName = "datetime")]
        public DateTime? CreatedDt { get; set; }
        public int? ModifiedBy { get; set; }
        
        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDt { get; set; }
        public int? RoleId { get; set; }
        public int? CompanyId { get; set; }
        
        [MaxLength(500)]
        public string? ProfilePicture { get; set; }
        
        public int? DepartmentId { get; set; }
        
        [MaxLength(50)]
        public string? experience { get; set; }

        [Column("dob", TypeName = "datetime2")]
        public DateTime? Dob { get; set; }

        [Column("doj", TypeName = "datetime2")]
        public DateTime? Doj { get; set; }
        public int? DeptId { get; set; }
        public int? PlantId { get; set; }
    }
}
