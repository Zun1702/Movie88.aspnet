using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("bookings")]
public partial class Booking
{
    [Key]
    [Column("bookingid")]
    public int Bookingid { get; set; }

    [Column("customerid")]
    public int Customerid { get; set; }

    [Column("showtimeid")]
    public int Showtimeid { get; set; }

    [Column("voucherid")]
    public int? Voucherid { get; set; }

    [Column("bookingcode")]
    [StringLength(20)]
    public string? Bookingcode { get; set; }

    [Column("bookingtime", TypeName = "timestamp without time zone")]
    public DateTime? Bookingtime { get; set; }

    [Column("totalamount")]
    [Precision(10, 2)]
    public decimal? Totalamount { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [InverseProperty("Booking")]
    public virtual ICollection<Bookingcombo> Bookingcombos { get; set; } = new List<Bookingcombo>();

    [InverseProperty("Booking")]
    public virtual ICollection<Bookingpromotion> Bookingpromotions { get; set; } = new List<Bookingpromotion>();

    [InverseProperty("Booking")]
    public virtual ICollection<Bookingseat> Bookingseats { get; set; } = new List<Bookingseat>();

    [ForeignKey("Customerid")]
    [InverseProperty("Bookings")]
    public virtual Customer Customer { get; set; } = null!;

    [InverseProperty("Booking")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("Showtimeid")]
    [InverseProperty("Bookings")]
    public virtual Showtime Showtime { get; set; } = null!;

    [ForeignKey("Voucherid")]
    [InverseProperty("Bookings")]
    public virtual Voucher? Voucher { get; set; }
}
