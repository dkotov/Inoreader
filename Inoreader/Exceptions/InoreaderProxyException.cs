using System;

namespace Inoreader.Exceptions
{
    public class InoreaderProxyException : Exception
    {
        public InoreaderProxyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}