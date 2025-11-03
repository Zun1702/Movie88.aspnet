using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("bookingcombos")]
public partial class Bookingcombo
{
    [Key]
    [Column("bookingcomboid")]
    public int Bookingcomboid { get; set; }

    [Column("bookingid")]
    public int Bookingid { get; set; }

    [Column("comboid")]
    public int Comboid { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("comboprice")]
    [Precision(10, 2)]
    public decimal? Comboprice { get; set; }

    [ForeignKey("Bookingid")]
    [InverseProperty("Bookingcombos")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("Comboid")]
    [InverseProperty("Bookingcombos")]
    public virtual Combo Combo { get; set; } = null!;
}
