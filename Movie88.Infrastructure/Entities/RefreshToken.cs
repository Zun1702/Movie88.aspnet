using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Movie88.Infrastructure.Entities;

/// <summary>
/// Auth: Store of tokens used to refresh JWT tokens once they expire.
/// </summary>
[Table("refresh_tokens", Schema = "auth")]
[Index("InstanceId", Name = "refresh_tokens_instance_id_idx")]
[Index("InstanceId", "UserId", Name = "refresh_tokens_instance_id_user_id_idx")]
[Index("Parent", Name = "refresh_tokens_parent_idx")]
[Index("SessionId", "Revoked", Name = "refresh_tokens_session_id_revoked_idx")]
[Index("Token", Name = "refresh_tokens_token_unique", IsUnique = true)]
[Index("UpdatedAt", Name = "refresh_tokens_updated_at_idx", AllDescending = true)]
public partial class RefreshToken
{
    [Column("instance_id")]
    public Guid? InstanceId { get; set; }

    [Key]
    [Column("id")]
    public long Id { get; set; }

    [Column("token")]
    [StringLength(255)]
    public string? Token { get; set; }

    [Column("user_id")]
    [StringLength(255)]
    public string? UserId { get; set; }

    [Column("revoked")]
    public bool? Revoked { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }

    [Column("parent")]
    [StringLength(255)]
    public string? Parent { get; set; }

    [Column("session_id")]
    public Guid? SessionId { get; set; }
}
