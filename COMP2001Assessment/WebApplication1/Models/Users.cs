namespace WebApplication1.Models
{
    public class Users
    {
        public static string Id { get; set; } = "";
        public static string Username { get; set; } = "";
        public static string Email { get; set; } = "";
        public static string PasswordHash { get; set; } = string.Empty;
        public static string PasswordSalt { get; set; } = string.Empty;
        public static bool isAdmin { get; set; }
        public static bool authenticated { get; set; } = false;
    }
}
