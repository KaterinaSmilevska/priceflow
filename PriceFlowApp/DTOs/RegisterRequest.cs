namespace PriceFlowApp.DTOs
{
    public class RegisterRequest
    {
        public string Ime { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public List<string> UlogaNames { get; set; } = [];
    }
}
