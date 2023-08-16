using System.Security.Cryptography;

namespace PemAPI.Services
{
    public class HashingService
    {
        private const int SaltSize = 32; // 256 bits
        private const int HashSize = 32; // 256 bits
        private const int Iterations = 10000; // can be adjusted

        public static string GenerateSalt()
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] saltBytes = new byte[SaltSize];
            rng.GetBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public static string HashPassword(string password, string salt)
        {
            using var hasher = new Rfc2898DeriveBytes(password, Convert.FromBase64String(salt), Iterations);
            byte[] hashBytes = hasher.GetBytes(HashSize);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
