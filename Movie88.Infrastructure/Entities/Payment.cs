using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

[Table("payments")]
public partial class Payment
{
    [Key]
    [Column("paymentid")]
    public int Paymentid { get; set; }

    [Column("bookingid")]
    public int Bookingid { get; set; }

    [Column("customerid")]
    public int Customerid { get; set; }

    [Column("methodid")]
    public int Methodid { get; set; }

    [Column("amount")]
    [Precision(10, 2)]
    public decimal Amount { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [Column("transactioncode")]
    [StringLength(255)]
    public string? Transactioncode { get; set; }

    [Column("paymenttime", TypeName = "timestamp without time zone")]
    public DateTime? Paymenttime { get; set; }

    [ForeignKey("Bookingid")]
    [InverseProperty("Payments")]
    public virtual Booking Booking { get; set; } = null!;

    [ForeignKey("Customerid")]
    [InverseProperty("Payments")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("Methodid")]
    [InverseProperty("Payments")]
    public virtual Paymentmethod Method { get; set; } = null!;
}
