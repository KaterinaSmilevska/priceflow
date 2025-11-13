namespace PriceFlowApp.DTOs
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = [];
        public required string Message { get; set; }
        public string Token { get; set; } = null!;
    }
}
