namespace PriceFlowApp.DTOs
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public List<string> Roles { get; set; } = [];
        public bool IsEmailVerified { get; set; }
    }
}
