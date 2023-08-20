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

    public class UserDTO
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }
    }

    public class LoginResponseModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public int expiresInSeconds { get; set; }
    }
}
