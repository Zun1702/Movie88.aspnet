using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("movies")]
public partial class Movie
{
    [Key]
    [Column("movieid")]
    public int Movieid { get; set; }

    [Column("title")]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("durationminutes")]
    public int Durationminutes { get; set; }

    [Column("director")]
    [StringLength(100)]
    public string? Director { get; set; }

    [Column("trailerurl")]
    [StringLength(255)]
    public string? Trailerurl { get; set; }

    [Column("releasedate")]
    public DateOnly? Releasedate { get; set; }

    [Column("posterurl")]
    [StringLength(255)]
    public string? Posterurl { get; set; }

    [Column("country")]
    [StringLength(100)]
    public string? Country { get; set; }

    [Column("rating")]
    [StringLength(10)]
    public string Rating { get; set; } = null!;

    [Column("genre")]
    [StringLength(255)]
    public string? Genre { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime? Createdat { get; set; }

    [InverseProperty("Movie")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    [InverseProperty("Movie")]
    public virtual ICollection<Showtime> Showtimes { get; set; } = new List<Showtime>();
}
