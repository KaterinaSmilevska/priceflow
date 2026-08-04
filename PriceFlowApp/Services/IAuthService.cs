using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IAuthService
    {
        Korisnici? FindById(int id);

        Korisnici? FindByUsername(string username);

        Korisnici? FindByVerificationToken(Guid token);

        IEnumerable<User> FindAll();

        User Update(User user);

        Korisnici Update(Korisnici user);

        User Delete(int id);

        RegisterResponse Register(RegisterRequest registerRequest);

        LoginResponse Login(LoginRequest loginRequest);

        PasswordValidationResponse ValidatePassword(PasswordValidationRequest request);

        EmailValidationResponse ValidateEmail(EmailValidationRequest request);

        void ForgotPassword(string email);

        void ResetPassword(Guid token, string newPassword);

        bool UsernameExists(string username);
    }
}
