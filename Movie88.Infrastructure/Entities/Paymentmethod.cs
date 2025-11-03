using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("paymentmethods")]
public partial class Paymentmethod
{
    [Key]
    [Column("methodid")]
    public int Methodid { get; set; }

    [Column("name")]
    [StringLength(50)]
    public string Name { get; set; } = null!;

    [Column("description")]
    [StringLength(255)]
    public string? Description { get; set; }

    [InverseProperty("Method")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
