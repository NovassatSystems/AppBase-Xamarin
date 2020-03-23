using System;
using System.Net;

namespace Core
{
    public class ApiException : Exception
    {
        public ApiException(string message) : base(message) { }

        public ApiException(string message, Exception innerException) : base(message, innerException) { }

        public HttpStatusCode StatusCode { get; set; }

        public string Content { get; set; }

        public static ApiException ThrowBadRequest(string message) {
            throw new ApiException(message) { StatusCode = System.Net.HttpStatusCode.BadRequest };
        }
    }
}
