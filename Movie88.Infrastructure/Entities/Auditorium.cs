using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("auditoriums")]
public partial class Auditorium
{
    [Key]
    [Column("auditoriumid")]
    public int Auditoriumid { get; set; }

    [Column("cinemaid")]
    public int Cinemaid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("seatscount")]
    public int Seatscount { get; set; }

    [ForeignKey("Cinemaid")]
    [InverseProperty("Auditoria")]
    public virtual Cinema Cinema { get; set; } = null!;

    [InverseProperty("Auditorium")]
    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();

    [InverseProperty("Auditorium")]
    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
