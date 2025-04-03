using System;

namespace Todo.Shared.Contracts.Constant;

public static class ErrorMessages
{
    public static class Exist
    {
        public const string EmailAlreadyExists = "Email already exists";
        public const string UsernameAlreadyExists = "Username already exists";
        public const string PhoneAlreadyExists = "Phone already exists";
    }
}
