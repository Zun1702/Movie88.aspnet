using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("reviews")]
public partial class Review
{
    [Key]
    [Column("reviewid")]
    public int Reviewid { get; set; }

    [Column("customerid")]
    public int Customerid { get; set; }

    [Column("movieid")]
    public int Movieid { get; set; }

    [Column("rating")]
    public int? Rating { get; set; }

    [Column("comment")]
    [StringLength(500)]
    public string? Comment { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime? Createdat { get; set; }

    [ForeignKey("Customerid")]
    [InverseProperty("Reviews")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("Movieid")]
    [InverseProperty("Reviews")]
    public virtual Movie Movie { get; set; } = null!;
}
