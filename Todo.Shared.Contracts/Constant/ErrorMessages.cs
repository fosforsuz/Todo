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
    }
    
    public static class Expired
    {
        public const string EmailVerificationToken = "Email verification token expired";
        public const string PhoneVerificationToken = "Phone verification token expired";
        
        public const string EmailAlreadyVerified = "Email already verified";
    }
}