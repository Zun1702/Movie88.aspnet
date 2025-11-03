namespace Movie88.Domain.Models
{
    public class RefreshTokenModel
    {
        public int Id { get; set; }
        public string? Token { get; set; }
        public string? UserId { get; set; }
        public bool? Revoked { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
