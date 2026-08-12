using System.Net;

namespace PriceFlowApp.Exceptions
{
    public class PriceFlowException : Exception
    {
        public string Code { get; }
        public int StatusCode { get; }

        public PriceFlowException(string code, string message, int statusCode): base(message)
        { 
            Code = code;
            StatusCode = statusCode;
        }
    }
}
