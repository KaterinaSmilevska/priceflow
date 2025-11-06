namespace PriceFlowApp.DTOs
{
    public class ResetPasswordRequest
    {
        public required Guid Token { get; set; }
        public required string NewPassword { get; set; }
    }
}
