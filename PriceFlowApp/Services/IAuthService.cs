using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IAuthService
    {
        Task<Korisnik> GetKorisnikByUsernameAsync(string username);
        Task<IEnumerable<Korisnik>> getAllKorisnikAsync();
        Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest);
        Task<LoginResponse> LoginAsync(LoginRequest loginRequest);
        Task<List<string>> GetUlogaNamesAsync();
        Task<bool> UsernameExistsAsync(string username);
        Task<PasswordValidationResponse> ValidatePasswordAsync(PasswordValidationRequest request);
        Task<EmailValidationResponse> ValidateEmailAsync(EmailValidationRequest request);
    }
}
