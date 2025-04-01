using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Todo.Shared.Enums;

namespace Todo.User.Domain.Entity;

[Table("users")]
public class User
{
    [Key]
    [Column("id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; init; } = Guid.CreateVersion7();

    #region User Information

    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    [Column("name")]
    public required string Name { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
    [Column("username")]
    public required string Username { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters.")]
    [Column("username_lower")]
    public required string UsernameLower { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 5 and 100 characters.")]
    [Column("email")]
    public required string Email { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100, MinimumLength = 5, ErrorMessage = "Email must be between 20 and 100 characters.")]
    [Column("email_lower")]
    public required string EmailLower { get; set; }

    [Phone]
    [StringLength(20, MinimumLength = 10, ErrorMessage = "Phone number must be between 10 and 20 characters.")]
    [Column("phone")]
    public string? Phone { get; set; }

    [Required]
    [StringLength(255, MinimumLength = 20, ErrorMessage = "Password must be between 20 and 255 characters.")]
    [Column("hashed_password")]
    public required string HashedPassword { get; set; }

    [Required][Column("role")] public Role Role { get; set; } = Role.Standard;

    [Required][Column("utc_offset")] public int UtcOffset { get; set; }

    [Required][Column("is_verified")] public bool IsVerified { get; set; }

    [Required][Column("notification_enabled")] public bool NotificationEnabled { get; set; } = true;

    [Required][Column("is_active")] public bool IsActive { get; set; } = true;

    #endregion

    #region Time Information

    [Required][Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required][Column("updated_at")] public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    #endregion

    #region Jwt Informations

    [Column("refresh_token")]
    [StringLength(255, MinimumLength = 20, ErrorMessage = "Refresh token must be between 20 and 255 characters.")]
    public string? RefreshToken { get; set; }

    [Column("refresh_token_expires_at")] public DateTime? RefreshTokenExpiresAt { get; set; }

    #endregion

    #region Password Reset

    [Column("password_reset_token")]
    [StringLength(255, MinimumLength = 20,
        ErrorMessage = "Password reset token must be between 20 and 255 characters.")]
    public string? PasswordResetToken { get; set; }

    [Column("password_reset_token_expires_at")]
    public DateTime? PasswordResetTokenExpiresAt { get; set; }

    #endregion

    #region Email Verification

    [Column("email_verification_token")]
    [StringLength(255, MinimumLength = 20,
        ErrorMessage = "Email verification token must be between 20 and 255 characters.")]
    public string? EmailVerificationToken { get; set; }

    [Column("email_verification_token_expires_at")]
    public DateTime? EmailVerificationTokenExpiresAt { get; set; }

    #endregion

    #region Two-Factor Authentication

    [Column("otp_code")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP code must be 6 characters.")]
    public string? OtpCode { get; set; }

    [Column("otp_code_expires_at")] public DateTime? OtpCodeExpiresAt { get; set; }

    #endregion
}