using System;

namespace Blueprint.CleanArchitectureAndCQRSDesignPattern.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
