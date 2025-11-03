using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("bookingpromotions")]
public partial class Bookingpromotion
{
    [Key]
    [Column("bookingpromotionid")]
    public int Bookingpromotionid { get; set; }

    [Column("bookingid")]
    public int Bookingid { get; set; }

    [Column("promotionid")]
    public int Promotionid { get; set; }

    [Column("discountapplied")]
    [Precision(10, 2)]
    public decimal Discountapplied { get; set; }

    [ForeignKey("Bookingid")]
    [InverseProperty("Bookingpromotions")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("Promotionid")]
    [InverseProperty("Bookingpromotions")]
    public virtual Promotion Promotion { get; set; } = null!;
}
