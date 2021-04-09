using System;

namespace Blueprint.GuardDesignPattern
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
