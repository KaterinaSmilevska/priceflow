namespace PriceFlowApp.Exceptions
{
    public class NotFoundException: PriceFlowException
    {
        public NotFoundException(string code, string message): base(code,message, StatusCodes.Status404NotFound) {
            
        }
    }
}
