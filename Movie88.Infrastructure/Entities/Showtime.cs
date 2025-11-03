using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("showtimes")]
public partial class Showtime
{
    [Key]
    [Column("showtimeid")]
    public int Showtimeid { get; set; }

    [Column("movieid")]
    public int Movieid { get; set; }

    [Column("auditoriumid")]
    public int Auditoriumid { get; set; }

    [Column("starttime", TypeName = "timestamp without time zone")]
    public DateTime Starttime { get; set; }

    [Column("endtime", TypeName = "timestamp without time zone")]
    public DateTime? Endtime { get; set; }

    [Column("price")]
    [Precision(10, 2)]
    public decimal Price { get; set; }

    [Column("format")]
    [StringLength(20)]
    public string Format { get; set; } = null!;

    [Column("languagetype")]
    [StringLength(50)]
    public string Languagetype { get; set; } = null!;

    [ForeignKey("Auditoriumid")]
    [InverseProperty("Showtimes")]
    public virtual Auditorium Auditorium { get; set; } = null!;

    [InverseProperty("Showtime")]
    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    [InverseProperty("Showtime")]
    public virtual ICollection<Bookingseat> Bookingseats { get; set; } = new List<Bookingseat>();

    [ForeignKey("Movieid")]
    [InverseProperty("Showtimes")]
    public virtual Movie Movie { get; set; } = null!;
}
