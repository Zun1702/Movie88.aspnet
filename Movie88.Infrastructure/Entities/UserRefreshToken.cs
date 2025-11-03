using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Movie88.Infrastructure.Entities
{
    [Table("refresh_tokens", Schema = "public")]
    public class UserRefreshToken
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("token")]
        [StringLength(255)]
        public string Token { get; set; } = null!;

        [Required]
        [Column("user_id")]
        [StringLength(255)]
        public string UserId { get; set; } = null!;

        [Column("revoked")]
        public bool Revoked { get; set; }

        [Column("created_at", TypeName = "timestamp without time zone")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at", TypeName = "timestamp without time zone")]
        public DateTime? UpdatedAt { get; set; }
    }
}
