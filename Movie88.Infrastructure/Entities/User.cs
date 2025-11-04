using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("User")]
[Index("Email", Name = "User_email_key", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("userid")]
    public int Userid { get; set; }

    [Column("roleid")]
    public int Roleid { get; set; }

    [Column("fullname")]
    [StringLength(100)]
    public string Fullname { get; set; } = null!;

    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("passwordhash")]
    [StringLength(255)]
    public string Passwordhash { get; set; } = null!;

    [Column("phone")]
    [StringLength(20)]
    public string? Phone { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime? Createdat { get; set; }

    [Column("updatedat", TypeName = "timestamp without time zone")]
    public DateTime? Updatedat { get; set; }

    [Column("isverified")]
    public bool Isverified { get; set; } = false;

    [Column("isactive")]
    public bool Isactive { get; set; } = true;

    [Column("verifiedat", TypeName = "timestamp without time zone")]
    public DateTime? Verifiedat { get; set; }

    [InverseProperty("User")]
    public virtual Customer? Customer { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<OtpToken> OtpTokens { get; set; } = new List<OtpToken>();

    [ForeignKey("Roleid")]
    [InverseProperty("Users")]
    public virtual Role Role { get; set; } = null!;
}
