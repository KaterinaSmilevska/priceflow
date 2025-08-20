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

        public static bool VerifyPassword(string enteredPassword, string storedHash, byte[] storedSalt)
        {
            string enteredPasswordHash = HashPassword(enteredPassword, storedSalt);
            return enteredPasswordHash == storedHash;
        }

    }
}
