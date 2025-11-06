using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IAuthService
    {
        Task<Korisnici?> FindByIdAsync(int id);

        Task<Korisnici?> FindByUsernameAsync(string username);

        Task<Korisnici?> FindByVerificationTokenAsync(Guid token);

        Task<IEnumerable<User>> FindAllAsync();

        Task UpdateAsync(User user);

        Task UpdateAsync(Korisnici user);

        Task DeleteAsync(int id);

        Task<bool> UsernameExistsAsync(string username);

        Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest);

        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);

        Task<PasswordValidationResponse> ValidatePasswordAsync(PasswordValidationRequest request);

        Task<EmailValidationResponse> ValidateEmailAsync(EmailValidationRequest request);

        Task ForgotPasswordAsync(string email);

        Task ResetPasswordAsync(Guid token, string newPassword);
    }
}
