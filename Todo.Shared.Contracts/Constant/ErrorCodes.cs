namespace Todo.Shared.Contracts.Constant;

public static class ErrorCodes
{
    public const string EmailAlreadyExists = "email_already_exists";
    public const string UsernameAlreadyExists = "username_already_exists";
    public const string PhoneAlreadyExists = "phone_already_exists";


    public const string UserNotFound = "user_not_found";
    public const string RefreshTokenNotFound = "refresh_token_not_found";

    public const string EmailVerificationTokenExpired = "email_verification_token_expired";
    public const string PhoneVerificationTokenExpired = "phone_verification_token_expired";
    public const string PasswordResetTokenExpired = "password_reset_token_expired";
    public const string OtpExpired = "otp_expired";
    public const string RefreshTokenExpired = "refresh_token_expired";

    public const string TwoFactorAuthenticationNotEnabled = "two_factor_authentication_not_enabled";
    public const string InvalidEmailOrPassword = "invalid_email_or_password";
    public const string InvalidOtp = "invalid_otp";
    public const string InvalidRefreshToken = "invalid_refresh_token";

    public const string EmailNotVerified = "email_not_verified";
    public const string EmailAlreadyVerified = "email_already_verified";

    public const string OtpTryExceeded = "otp_try_exceeded";
}