using System;

namespace Core
{
    public class ExceptionExtensions : Exception
    {
        public ExceptionExtensions(string message) : base(message)
        {
        }
    }
}
