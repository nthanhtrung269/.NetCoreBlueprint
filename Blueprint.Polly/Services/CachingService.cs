namespace Blueprint.Polly
{
    public class CachingService : ICachingService
    {
        public bool IsLoggingDatabase()
        {
            return false;
        }
    }
}
