namespace Blueprint.Polly
{
    public enum TransactionLogStep
    {
        Unknown = 0,
        CustomExceptionHandlerMiddleware = 1,
        BaseApiControllerExceptionHandling = 2
    }
}
