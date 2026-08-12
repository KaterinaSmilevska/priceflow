namespace PriceFlowApp.DTOs
{
    public class SessionStatusResponse
    {
        public bool IsLoggedIn { get; set; }
        public string? Username { get; set; }
        public string? UserId { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
