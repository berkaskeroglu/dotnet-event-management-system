using System;
using System.Security.Cryptography;
using System.Text;


namespace EventManagementSystem.Utilities
{
    public class PasswordHasher : IPasswordHasher
    {
        public string GetSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }

        public string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combined = Encoding.UTF8.GetBytes(password + salt);
                var hashBytes = sha256.ComputeHash(combined);
                return Convert.ToBase64String(hashBytes);
            }
        }

        public bool ValidatePassword(string password, string salt, string hash)
        {
            var hashedPassword = HashPassword(password, salt);
            return hashedPassword == password;
        }
    }
}
