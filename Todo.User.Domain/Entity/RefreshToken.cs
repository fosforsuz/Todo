using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Todo.User.Domain.Entity;

[Table("refresh_tokens")]
public class RefreshToken
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }

    [Required] [Column("user_id")] public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))] public virtual User? User { get; set; }

    [Required]
    [Column("token")]
    [StringLength(255, MinimumLength = 0, ErrorMessage = "Token length must be between 0 and 255 characters.")]
    public string Token { get; set; } = null!;

    [Required] [Column("expires_at")] public DateTime ExpiresAt { get; set; }

    [Column("is_used")] public bool IsUsed { get; set; }
    [Column("is_revoked")] public bool IsRevoked { get; set; }


    [Column("created_by_ip")]
    [StringLength(255, MinimumLength = 0, ErrorMessage = "IP address length must be between 0 and 255 characters.")]
    public string? CreatedByIp { get; set; }

    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt, string? createdByIp = null)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            IsUsed = false,
            IsRevoked = false,
            CreatedByIp = createdByIp,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}