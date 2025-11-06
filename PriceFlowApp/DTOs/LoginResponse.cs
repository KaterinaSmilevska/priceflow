namespace PriceFlowApp.DTOs
{
    public class LoginResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; } = [];
        public required string Message { get; set; }
        public string Token { get; set; }
    }
}
