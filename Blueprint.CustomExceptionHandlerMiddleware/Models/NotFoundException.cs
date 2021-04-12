using System;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project.Models
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
