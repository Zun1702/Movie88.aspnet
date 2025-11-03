using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("bookingseats")]
[Index("Showtimeid", "Seatid", Name = "uq_showtimeseat", IsUnique = true)]
public partial class Bookingseat
{
    [Key]
    [Column("bookingseatid")]
    public int Bookingseatid { get; set; }

    [Column("bookingid")]
    public int Bookingid { get; set; }

    [Column("showtimeid")]
    public int Showtimeid { get; set; }

    [Column("seatid")]
    public int Seatid { get; set; }

    [Column("seatprice")]
    [Precision(10, 2)]
    public decimal Seatprice { get; set; }

    [ForeignKey("Bookingid")]
    [InverseProperty("Bookingseats")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("Seatid")]
    [InverseProperty("Bookingseats")]
    public virtual Seat Seat { get; set; } = null!;

    [ForeignKey("Showtimeid")]
    [InverseProperty("Bookingseats")]
    public virtual Showtime Showtime { get; set; } = null!;
}
