using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace _5sAudit.Models;

[Table("fsa_users")]
[Index("EmailId", Name = "UQ_fsa_users_email", IsUnique = true)]
public partial class FsaUser
{
    [Key]
    public int Id { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Firstname { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? Lastname { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? EmailId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(10)]
    [Unicode(false)]
    public string? MobileNo { get; set; }

    [StringLength(1)]
    [Unicode(false)]
    public string? Status { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDt { get; set; }

    public int? ModifiedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ModifiedDt { get; set; }

    public int? RoleId { get; set; }

    public int CompanyId { get; set; }

    [Unicode(false)]
    public string? ProfilePicture { get; set; }

    public int? DepartmentId { get; set; }

    [Column("experience")]
    public int? Experience { get; set; }

    [Column("dob")]
    [Unicode(false)]
    public string? Dob { get; set; }

    [Column("doj")]
    [Unicode(false)]
    public string? Doj { get; set; }

    public int? DeptId { get; set; }

    public int? PlantId { get; set; }
}
