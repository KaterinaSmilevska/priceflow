namespace PriceFlowApp.DTOs
{
    public class RegisterResponse
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = [];
        public string Message { get; set; } = null!;
    }
}
