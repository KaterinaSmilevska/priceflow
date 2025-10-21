namespace PriceFlowApp.DTOs
{
    public class PasswordValidationRequest
    {
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }
}
