using System;
using System.Collections.Generic;
using System.Net;

namespace Blueprint.AdapterDesignPattern.Services.WebApiClientServices
{
    public class WebApiException : Exception
    {
        public HttpStatusCode StatusCode { get; set; }

        public bool IsTransientError
        {
            get { return StatusCodesWorthRetrying.Contains(StatusCode); }
        }

        private static readonly List<HttpStatusCode> StatusCodesWorthRetrying = new List<HttpStatusCode>
        {
            HttpStatusCode.RequestTimeout,
            HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway,
            HttpStatusCode.ServiceUnavailable,
            HttpStatusCode.GatewayTimeout
        };

        public WebApiException(HttpStatusCode status, string message) : base(message)
        {
            StatusCode = status;
        }

        public WebApiException(HttpStatusCode status, string message, Exception exception) : base(message, exception)
        {
            StatusCode = status;
        }
    }
}
