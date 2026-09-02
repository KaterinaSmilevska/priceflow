namespace PriceFlowApp.DTOs
{
    public class EmailValidationResponse
    {
        public bool IsValid { get; set; }
        public string Code { get; set; } = null!;
        public required string Message { get; set; }
    }
}
