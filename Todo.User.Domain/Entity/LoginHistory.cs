using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Todo.User.Domain.Entity;

[Table("login_history")]
public class LoginHistory
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; } = Guid.CreateVersion7();

    [Required] [Column("user_id")] public Guid UserId { get; init; }

    [ForeignKey(nameof(UserId))] public virtual User? User { get; init; }

    [Required] [Column("login_at")] public DateTime LoginAt { get; init; } = DateTime.UtcNow;

    [Column("ip_address")]
    [StringLength(100, MinimumLength = 0,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string? IpAddress { get; init; }

    [Column("user_agent")]
    [StringLength(500, MinimumLength = 0,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string? UserAgent { get; init; }

    [Column("is_successful")] public bool IsSuccessful { get; init; }

    public static LoginHistory Create(Guid userId, bool isSuccessful, string? ipAddress, string? userAgent)
    {
        return new LoginHistory
        {
            UserId = userId,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            IsSuccessful = isSuccessful
        };
    }
}