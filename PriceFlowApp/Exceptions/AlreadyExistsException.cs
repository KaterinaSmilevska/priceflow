namespace PriceFlowApp.Exceptions
{
    public class AlreadyExistsException: PriceFlowException
    {
        public AlreadyExistsException(string code, string message) : base(code, message, StatusCodes.Status409Conflict)
        {

        }
    }
}
