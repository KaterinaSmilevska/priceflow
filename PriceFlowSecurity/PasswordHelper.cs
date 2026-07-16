using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace PriceFlowSecurity
{
    public class PasswordHelper
    {
        private const int HashLengthInBytes = 32;
        private const int SaltLengthInBytes = 16;
        private const int HashInterations = 100000;

        public static byte[] GenerateSalt()
        {
            return RandomNumberGenerator.GetBytes(16);
        }

        public static byte[] CalculateHashAndSalt(string password)
        {
            byte[] saltBytes = GenerateSalt();
            byte[] hashBytes = HashPassword(password, saltBytes);

            byte[] HashAndSaltBytes = new byte[HashLengthInBytes + SaltLengthInBytes];
            Buffer.BlockCopy(hashBytes, 0, HashAndSaltBytes, 0, 32);
            Buffer.BlockCopy(saltBytes, 0, HashAndSaltBytes, 32, 16);

            return HashAndSaltBytes;
        }

        public static byte[] HashPassword(string password, byte[] salt)
        {
            byte[] hash = KeyDerivation.Pbkdf2(
              password: password,
              salt: salt,
              prf: KeyDerivationPrf.HMACSHA256,
              iterationCount: HashInterations,
              numBytesRequested: 32);

            return hash;
        }

        public static bool VerifyPassword(string enteredPassword, byte[] storedHash)
        {
            var hash = new byte[32];
            var salt = new byte[16];

            Buffer.BlockCopy(storedHash, 0, hash, 0, 32);
            Buffer.BlockCopy(storedHash, 32, salt, 0, 16);

            var enteredHash = HashPassword(enteredPassword, salt);

            return hash.SequenceEqual(enteredHash);
        }

        private const string SpecialCharacters = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-""";
        public static bool ValidatePasswordStrength(string clearTextPassword)
        {
            if (string.IsNullOrEmpty(clearTextPassword))
            {
                throw new ArgumentNullException(nameof(clearTextPassword));
            }

            return (clearTextPassword.Length >= 8 && clearTextPassword.Any(char.IsLower) &&
                    clearTextPassword.Any(char.IsUpper) && clearTextPassword.Any(char.IsDigit) &&
                    clearTextPassword.Any(c => SpecialCharacters.Contains(c)) && !clearTextPassword.Any(char.IsWhiteSpace));
        }
    }
}
