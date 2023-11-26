using System;
using comdeeds.Models;
using System.Net;

namespace comdeeds.App_Code.PinPayments
{
    [Serializable]
    public class PinException : ApplicationException
    {
        public HttpStatusCode HttpStatusCode { get; set; }
        public PinError PinError { get; set; }

        public PinException()
        {
        }

        public PinException(HttpStatusCode httpStatusCode, PinError pinError, string message)
            : base(message)
        {
            HttpStatusCode = httpStatusCode;
            PinError = pinError;
        }
    }
}
