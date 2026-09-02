namespace PriceFlowApp.Exceptions
{
    public class BusinessRuleException: PriceFlowException
    {
        public BusinessRuleException(string code, string message): base(code, message, StatusCodes.Status400BadRequest)
        {
            
        }
    }
}
