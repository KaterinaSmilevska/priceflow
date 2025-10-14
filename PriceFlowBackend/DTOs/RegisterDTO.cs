namespace PriceFlowApp.DTOs
{
    public class RegisterDTO
    {
        public string Ime { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<int> UlogaIds { get; set; } = new();
    }
}
