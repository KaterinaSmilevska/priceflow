namespace PriceFlowApp.Exceptions
{
    public class AlreadyExistsException: Exception
    {
        public string ErrorCode { get; }

        public AlreadyExistsException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}
