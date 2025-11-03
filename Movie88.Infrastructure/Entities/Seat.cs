using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("seats")]
[Index("Auditoriumid", "Row", "Number", Name = "uq_seat", IsUnique = true)]
public partial class Seat
{
    [Key]
    [Column("seatid")]
    public int Seatid { get; set; }

    [Column("auditoriumid")]
    public int Auditoriumid { get; set; }

    [StringLength(2)]
    public string Row { get; set; } = null!;

    public int Number { get; set; }

    [Column("type")]
    [StringLength(20)]
    public string? Type { get; set; }

    [Column("isavailable")]
    public bool? Isavailable { get; set; }

    [ForeignKey("Auditoriumid")]
    [InverseProperty("Seats")]
    public virtual Auditorium Auditorium { get; set; } = null!;

    [InverseProperty("Seat")]
    public virtual ICollection<Bookingseat> Bookingseats { get; set; } = new List<Bookingseat>();
}
