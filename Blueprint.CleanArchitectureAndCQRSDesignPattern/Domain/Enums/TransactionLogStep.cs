namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Enums
{
    public enum TransactionLogStep
    {
        None = 0,
        CustomExceptionHandlerMiddleware = 1,
        BaseApiControllerExceptionHandling = 2
    }
}
