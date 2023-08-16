namespace PemAPI.Models
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }

        public string Salt { get; set; }
    }

    public class CreateUserModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class RequestUserModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
    }
}
