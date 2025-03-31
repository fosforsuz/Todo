using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Todo.User.Domain.Entity;

public class LoginHistory
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.CreateVersion7();

    [Required] [Column("user_id")] public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))] public virtual User? User { get; set; }

    [Required] [Column("login_at")] public DateTime LoginAt { get; set; } = DateTime.UtcNow;

    [Column("ip_address")]
    [StringLength(100, MinimumLength = 0,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string? IpAddress { get; set; }

    [Column("user_agent")]
    [StringLength(500, MinimumLength = 0,
        ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.")]
    public string? UserAgent { get; set; }

    [Column("is_successful")] public bool IsSuccessful { get; set; }
}