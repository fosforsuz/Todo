namespace Todo.Shared.Contracts.Constant;

public static class ErrorCodes
{
    public const string EmailAlreadyExists = "email_already_exists";
    public const string UsernameAlreadyExists = "username_already_exists";
    public const string PhoneAlreadyExists = "phone_already_exists";
    
    public const string UserNotFound = "user_not_found";
    
    public const string EmailVerificationTokenExpired = "email_verification_token_expired";
    public const string PhoneVerificationTokenExpired = "phone_verification_token_expired";
    public const string PasswordResetTokenExpired = "password_reset_token_expired";
    
    public const string EmailAlreadyVerified = "email_already_verified";
}