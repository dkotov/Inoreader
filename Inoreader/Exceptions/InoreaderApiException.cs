using System;
using System.Net;

namespace Inoreader.Exceptions
{
    public class InoreaderApiException : Exception
    {
        public HttpStatusCode StatusCode { get; private set; }

        public InoreaderApiException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}