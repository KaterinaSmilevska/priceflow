using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IAuthService
    {
        User FindById(int id);

        User FindByUsername(string username);

        User FindByVerificationToken(Guid token);

        IEnumerable<User> FindAll();

        User Update(int id, User user);

        User Delete(int id);

        RegisterResponse Register(RegisterRequest registerRequest);

        Task<LoginResponse> Login(LoginRequest loginRequest);

        PasswordValidationResponse ValidatePassword(PasswordValidationRequest request);

        EmailValidationResponse ValidateEmail(EmailValidationRequest request);

        void VerifyEmail(Guid token);

        void ForgotPassword(string email);

        void ResetPassword(Guid token, string newPassword);

        bool UsernameExists(string username);
    }
}
