namespace Blueprint.Polly
{
    public enum ResponseErrorCode
    {
        None = 0,
        SystemError = 1,
        UnhandleException = 2,
        NotFound = 3,
        ArgumentNullException = 4,
        ArgumentOutOfRangeException = 5,
        ArgumentException = 6,
        NotFoundException = 7,
        InvalidOperationException = 8,
        AuthenticationException = 9,
        ServiceUnavailable = 10,
        RequestTimeout = 11,
        ExternalServiceUnavailable = 12
    }
}
