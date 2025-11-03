using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("combos")]
public partial class Combo
{
    [Key]
    [Column("comboid")]
    public int Comboid { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Column("price")]
    [Precision(10, 2)]
    public decimal Price { get; set; }

    [Column("imageurl")]
    [StringLength(255)]
    public string? Imageurl { get; set; }

    [InverseProperty("Combo")]
    public virtual ICollection<Bookingcombo> Bookingcombos { get; set; } = new List<Bookingcombo>();
}
