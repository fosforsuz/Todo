namespace Todo.Shared.Contracts.Constant;

public static class ErrorMessages
{
    public static class Exist
    {
        public const string EmailAlreadyExists = "Email already exists";
        public const string UsernameAlreadyExists = "Username already exists";
        public const string PhoneAlreadyExists = "Phone already exists";
    }

    public static class NotFound
    {
        public const string User = "User not found";
        public const string RefreshToken = "Refresh token not found";
    }

    public static class Expired
    {
        public const string EmailVerificationToken = "Email verification token expired";
        public const string PhoneVerificationToken = "Phone verification token expired";
        public const string PasswordResetToken = "Password reset token expired";

        public const string Otp = "OTP expired";
        public const string RefreshToken = "Refresh token expired";
    }

    public static class Verified
    {
        public const string Email = "Email already verified";
        public const string Phone = "Phone already verified";
    }

    public static class Unverified
    {
        public const string Email = "Email not verified";
        public const string Phone = "Phone not verified";
    }

    public static class NotEnabled
    {
        public const string TwoFactorAuthentication = "Two factor authentication not enabled";
    }

    public static class Invalid
    {
        public const string EmailOrPassword = "Invalid email or password";
        public const string Email = "Invalid email";
        public const string Phone = "Invalid phone";
        public const string RefreshToken = "Invalid refresh token";
        public const string Password = "Invalid password";
        public const string Otp = "Invalid OTP";
        public const string Role = "Invalid role specified.";
    }

    public static class Blocked
    {
        public const string OtpTryExceeded = "OTP try exceeded";
    }

    public static class NotEmpty
    {
        public const string UserId = "User ID must not be empty";
        public const string Email = "Email must not be empty";
        public const string Phone = "Phone must not be empty";
        public const string Password = "Password must not be empty";
        public const string Username = "Username must not be empty";
        public const string Name = "Name must not be empty";
        public const string RefreshToken = "Refresh token must not be empty";
        public const string PasswordResetToken = "Password reset token must not be empty";
        public const string NewPassword = "New password must not be empty";
        public const string PasswordConfirmation = "Password confirmation must not be empty";
        public const string VerifyToken = "Verify token must not be empty";
        public const string Otp = "OTP must not be empty";
    }

    public static class MinLength
    {
        public const string Password = "Password must be at least 6 characters long";
        public const string NewPassword = "New password must be at least 6 characters long";

        public const string MinimumLength = "Minimum length is {0} characters";
    }

    public static class MaxLength
    {
        public const string MaximumLength = "Maximum length is {0} characters";
    }

    public static class Match
    {
        public const string Password = "Password and confirm password do not match";
    }
}