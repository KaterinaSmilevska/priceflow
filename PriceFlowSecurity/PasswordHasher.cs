using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace PriceFlowSecurity
{
    public class PasswordHasher
    {
        public static byte[] GenerateSalt()
        {
            return RandomNumberGenerator.GetBytes(16);

        }

        public static string HashPassword(string password, byte[] salt)
        {
            byte[] hash = KeyDerivation.Pbkdf2(
              password: password,
              salt: salt,
              prf: KeyDerivationPrf.HMACSHA256,
              iterationCount: 100000,
              numBytesRequested: 32);
            return Convert.ToBase64String(hash);
        }

        public static bool VerifyPassword(string enteredPassword, byte[] storedHash)
        {
            var hash = new byte[32];
            var salt = new byte[16];

            Buffer.BlockCopy(storedHash, 0, hash, 0, 32);
            Buffer.BlockCopy(storedHash, 32, salt, 0, 16);

            var enteredHash = Convert.FromBase64String(HashPassword(enteredPassword, salt));
            return hash.SequenceEqual(enteredHash);
        }

    }
}
