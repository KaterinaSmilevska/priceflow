namespace PriceFlowApp.DTOs
{
    public class PasswordValidationResponse
    {
        public bool IsValid { get; set; }
        public string Code { get; set; } = null!;
        public required string Message { get; set; }
    }
}
