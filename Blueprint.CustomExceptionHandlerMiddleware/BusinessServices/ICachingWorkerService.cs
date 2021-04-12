namespace Blueprint.CustomExceptionHandlerMiddleware.Project.BusinessServices
{
    public interface ICachingWorkerService
    {
        public bool IsLoggingDatabase();
        public string GetAuthorizeToken();
    }
}
