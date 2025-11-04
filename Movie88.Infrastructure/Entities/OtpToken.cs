using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("otp_tokens")]
[Index("Email", Name = "idx_otp_email")]
[Index("Otpcode", Name = "idx_otp_code")]
[Index("Userid", Name = "idx_otp_userid")]
[Index("Createdat", Name = "idx_otp_createdat")]
[Index("Expiresat", Name = "idx_otp_expiresat")]
[Index("Otpcode", "Otptype", "Email", Name = "idx_otp_code_type", IsUnique = true)]
public partial class OtpToken
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Required]
    [Column("userid")]
    public int Userid { get; set; }

    [Required]
    [Column("otpcode")]
    [StringLength(6)]
    public string Otpcode { get; set; } = null!;

    [Required]
    [Column("otptype")]
    [StringLength(20)]
    public string Otptype { get; set; } = null!;

    [Required]
    [Column("email")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Required]
    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime Createdat { get; set; }

    [Required]
    [Column("expiresat", TypeName = "timestamp without time zone")]
    public DateTime Expiresat { get; set; }

    [Required]
    [Column("isused")]
    public bool Isused { get; set; } = false;

    [Column("usedat", TypeName = "timestamp without time zone")]
    public DateTime? Usedat { get; set; }

    [Column("ipaddress")]
    [StringLength(45)]
    public string? Ipaddress { get; set; }

    [Column("useragent")]
    [StringLength(500)]
    public string? Useragent { get; set; }

    // Navigation property
    [ForeignKey("Userid")]
    [InverseProperty("OtpTokens")]
    public virtual User User { get; set; } = null!;
}
