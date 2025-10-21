using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.IdentityModel.Tokens;
using PriceFlowApp.DTOs;
using PriceFlowSecurity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace PriceFlowApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IAuthRepository korisnikRepository, IHttpContextAccessor httpContextAccessor)
        {
            _authRepository = korisnikRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Korisnik>> getAllKorisnikAsync()
        {
            return await _authRepository.getAllKorisnikAsync();
        }

        public async Task<Korisnik> GetKorisnikByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty.");

            var korisnik = await _authRepository.GetKorisnikByUsernameAsync(username);

            if(korisnik == null)
                throw new ArgumentException("Korisnik not found");
            return korisnik;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.Ime) ||
                string.IsNullOrWhiteSpace(registerRequest.Username) ||
                string.IsNullOrWhiteSpace(registerRequest.Email) ||
                string.IsNullOrWhiteSpace(registerRequest.Password))
                throw new ArgumentException("All fields are required.");

            var passwordValidation = await ValidatePasswordAsync(new PasswordValidationRequest
            {
                Password = registerRequest.Password,
                ConfirmPassword = registerRequest.ConfirmPassword
            });
            if (!passwordValidation.IsValid)
                throw new ArgumentException(passwordValidation.Message);

            var emailValidation = await ValidateEmailAsync(new EmailValidationRequest
            {
                Email = registerRequest.Email,
            });
            if (!emailValidation.IsValid)
                throw new ArgumentException(emailValidation.Message);

            if (await _authRepository.GetByUsernameAsync(registerRequest.Username) != null)
                throw new ArgumentException("Username already exists.");

            var validUlogaNames = await _authRepository.GetUlogaNamesAsync();
            if (registerRequest.UlogaNames.Any() && registerRequest.UlogaNames.Any(name => !validUlogaNames.Contains(name)))
                throw new ArgumentException("One or more role names are invalid.");

            var salt = PasswordHasher.GenerateSalt();
            var hash = PasswordHasher.HashPassword(registerRequest.Password, salt);

            var fullPasswordBytes = new byte[48];
            Buffer.BlockCopy(Convert.FromBase64String(hash), 0, fullPasswordBytes, 0, 32);
            Buffer.BlockCopy(salt, 0, fullPasswordBytes, 32, 16);

            var korisnik = new Korisnik
            {
                Ime = registerRequest.Ime,
                Username = registerRequest.Username,
                PasswordHash = fullPasswordBytes,
                Email = registerRequest.Email
            };

            await _authRepository.AddKorisnikAsync(korisnik);

            var ulogaIds = await _authRepository.GetUlogaIdsByNamesAsync(registerRequest.UlogaNames);
            foreach (var ulogaId in ulogaIds)
            {
                await _authRepository.AddKorisnikUlogaAsync(new KorisnikUloga
                {
                    KorisnikId = korisnik.Id,
                    UlogaId = ulogaId
                });
            }

            return new RegisterResponse
            {
                Id = korisnik.Id,
                Username = korisnik.Username,
                Email = korisnik.Email,
                Ulogas = registerRequest.UlogaNames,
                Message = "Registration successfull"
            };
        }

        public async Task<List<string>> GetUlogaNamesAsync()
        {
            return await _authRepository.GetUlogaNamesAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _authRepository.UsernameExistsAsync(username);
        }

        public async Task<PasswordValidationResponse> ValidatePasswordAsync(PasswordValidationRequest request)
        {
            return await Task.Run(() =>
            {

                if (string.IsNullOrWhiteSpace(request.Password) || string.IsNullOrWhiteSpace(request.ConfirmPassword))
                {
                    return new PasswordValidationResponse { IsValid = false, Message = "Password and Confirm Password are required." };
                }
                if (request.Password != request.ConfirmPassword)
                {
                    return new PasswordValidationResponse { IsValid = false, Message = "Passwords do not match." };
                }
                if (request.Password.Length < 8)
                {
                    return new PasswordValidationResponse { IsValid = false, Message = "Password must be at least 8 characters long." };
                }

                if (!Regex.IsMatch(request.Password, @"\d"))
                {
                    return new PasswordValidationResponse { IsValid = false, Message = "Password must contain at least one number." };
                }
                if (!Regex.IsMatch(request.Password, @"[A-Z]"))
                {
                    return new PasswordValidationResponse { IsValid = false, Message = "Password must contain at least one uppercase letter." };
                }

                if (!Regex.IsMatch(request.Password, @"[!@#$%^&*(),.?""':{}|<>]"))
                {
                    return new PasswordValidationResponse { IsValid = false, Message = "Password must contain at least one special character." };
                }
                return new PasswordValidationResponse { IsValid = true, Message = "Password is valid." };
            });
        }

        public async Task<EmailValidationResponse> ValidateEmailAsync(EmailValidationRequest request)
        {
            return await Task.Run(() =>
            {
                if (string.IsNullOrWhiteSpace(request.Email))
                {
                    return new EmailValidationResponse { IsValid = false, Message = "Email is required" };
                }

                string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                if (!Regex.IsMatch(request.Email, emailPattern))
                {
                    return new EmailValidationResponse { IsValid = false, Message = "Invalid email format." };
                }

                return new EmailValidationResponse { IsValid = true, Message = "Email is valid." };

            });

        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginRequest)
        {
            if(string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                throw new ArgumentException("Username and password are required.");
            }

            var korisnik = await _authRepository.GetByUsernameAsync(loginRequest.Username);
            if (korisnik == null)
                throw new ArgumentException("Invalid username or password.");
            if (!PasswordHasher.VerifyPassword(loginRequest.Password, korisnik.PasswordHash))
                throw new ArgumentException("Invalid username or password.");

            var httpContext = _httpContextAccessor.HttpContext;
            httpContext.Session.SetString("Username", korisnik.Username);
            httpContext.Response.Cookies.Append("Username", korisnik.Username,new CookieOptions 
                { HttpOnly = true, Expires = DateTimeOffset.Now.AddHours(1) });

            var ulogas = await _authRepository.GetUlogasForKorisnik(korisnik.Id);
            var ulogaNames = await _authRepository.GetUlogaNames(ulogas);

            return new LoginResponse
            {
                Id = korisnik.Id,
                Username = korisnik.Username,
                Email = korisnik.Email,
                Ulogas = ulogaNames,
                Message = "Login successful"
            };
        }
    }
}
