namespace Blueprint.Polly.WebApi
{
    public static class ContextKey
	{
		public const string RetryCount = nameof(RetryCount);
		public const string CircuitStatus = nameof(CircuitStatus);
	}
}
