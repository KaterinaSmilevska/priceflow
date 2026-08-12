namespace PriceFlowApp.Exceptions
{
    public class UnauthorizedException: PriceFlowException
    {
        public UnauthorizedException(string code, string message): base(code,message, StatusCodes.Status401Unauthorized)
        {
            
        }
    }
}
