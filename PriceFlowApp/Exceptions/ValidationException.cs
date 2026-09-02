namespace PriceFlowApp.Exceptions
{
    public class ValidationException: PriceFlowException
    {
        public ValidationException(string code, string message) : base(code,message, StatusCodes.Status400BadRequest)
        {

        }
    }
}
