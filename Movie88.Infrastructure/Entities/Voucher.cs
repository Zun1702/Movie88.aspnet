using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("vouchers")]
[Index("Code", Name = "vouchers_code_key", IsUnique = true)]
public partial class Voucher
{
    [Key]
    [Column("voucherid")]
    public int Voucherid { get; set; }

    [Column("code")]
    [StringLength(50)]
    public string Code { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("discounttype")]
    [StringLength(20)]
    public string? Discounttype { get; set; }

    [Column("discountvalue")]
    [Precision(10, 2)]
    public decimal? Discountvalue { get; set; }

    [Column("minpurchaseamount")]
    [Precision(10, 2)]
    public decimal? Minpurchaseamount { get; set; }

    [Column("expirydate")]
    public DateOnly? Expirydate { get; set; }

    [Column("usagelimit")]
    public int? Usagelimit { get; set; }

    [Column("usedcount")]
    public int? Usedcount { get; set; }

    [Column("isactive")]
    public bool? Isactive { get; set; }

    [InverseProperty("Voucher")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
