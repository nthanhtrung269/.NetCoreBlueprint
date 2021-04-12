namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Constants
{
    public static class MessageConstants
    {
        public const string Ok = "Ok";
        public const string SystemError = "An error occurred in the system";
        public const string BadRequest = "The request is invalid";
        public const string NotFound = "The object is not found";
        public const string InvalidOperation = "The operation is now allowed";
        public const string InvalidArgument = "The arguments are not valid";
    }
}
