using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("promotions")]
public partial class Promotion
{
    [Key]
    [Column("promotionid")]
    public int Promotionid { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("startdate")]
    public DateOnly? Startdate { get; set; }

    [Column("enddate")]
    public DateOnly? Enddate { get; set; }

    [Column("discounttype")]
    [StringLength(20)]
    public string? Discounttype { get; set; }

    [Column("discountvalue")]
    [Precision(10, 2)]
    public decimal? Discountvalue { get; set; }

    [InverseProperty("Promotion")]
    public virtual ICollection<Bookingpromotion> Bookingpromotions { get; set; } = new List<Bookingpromotion>();
}
