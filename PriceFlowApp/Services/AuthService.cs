using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowSecurity;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace PriceFlowApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRolesRepository _rolesRepository;
        private readonly IUsersRolesRepository _usersRolesRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;

        public AuthService(IAuthRepository korisnikRepository, IRolesRepository rolesRepository, IUsersRolesRepository usersRolesRepository,
            IHttpContextAccessor httpContextAccessor, IEmailService emailService)
        {
            _authRepository = korisnikRepository;
            _rolesRepository = rolesRepository;
            _usersRolesRepository = usersRolesRepository;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        public async Task<Korisnici?> FindByIdAsync(int id)
        {
            return await _authRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> FindAllAsync()
        {
            var users = await _authRepository.GetAllAsync();
            var foundUsers = users.Select(item => new User
            {
                Id = item.Id,
                Name = item.Ime,
                Username = item.Username,
                Email = item.Email,
                Roles = item.KorisniciUlogi.Select(ku => ku.Uloga.Ime).ToList()
            });

            return foundUsers;
        }

        public async Task<Korisnici?> FindByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty.");

            var korisnik = await _authRepository.GetByUsernameAsync(username);

            if(korisnik == null)
                throw new ArgumentException("Korisnik not found");

            return korisnik;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.Name) ||
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

            var validRoleNames = await _rolesRepository.GetNamesAsync();
            if (registerRequest.RoleNames.Any() && registerRequest.RoleNames.Any(name => !validRoleNames.Contains(name)))
                throw new ArgumentException("One or more role names are invalid.");

            byte[] fullPasswordBytes = PasswordHelper.CalculateHashAndSalt(registerRequest.Password);

            var token = Guid.NewGuid();

            var user = new Korisnici
            {
                Ime = registerRequest.Name,
                Username = registerRequest.Username,
                PasswordHash = fullPasswordBytes,
                Email = registerRequest.Email,
                EmailVerificationToken = token,
                IsEmailVerified = false,
                ResetPasswordToken = null,
                ResetPasswordTokenExpiry = null
            };

            await _authRepository.AddAsync(user);

            var roleIds = await _rolesRepository.GetIdsByNamesAsync(registerRequest.RoleNames);
            foreach (var roleId in roleIds)
            {
                await _usersRolesRepository.AddAsync(new KorisniciUlogi
                {
                    KorisnikId = user.Id,
                    UlogaId = roleId
                });
            }

            var verificationLink = $"https://localhost:44413/api/auth/verify-email?token={token}";
            await _emailService.SendEmailAsync(user.Email, "Verify your PriceFlow account",
                $"<p>Welcome to PriceFlow, {user.Ime}!</p>" + $"<p>Please verify your email by clicking the link below:</p>"
                + $"<a href='{verificationLink}'>Verify Email</a>");

            return new RegisterResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = registerRequest.RoleNames,
                Message = "Registration successfull"
            };
        }

        public async Task<List<string>> FindUlogaNamesAsync()
        {
            return await _rolesRepository.GetNamesAsync();
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

                if(!PasswordHelper.ValidatePasswordStrength(request.Password))
                {
                    return new PasswordValidationResponse
                    {
                        IsValid = false,
                        Message = "Password must contain at least 8 characters, " +
                        "with one lowercase letter, one uppercase letter, one number, one special character and no spaces."
                    };
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

            var user = await _authRepository.GetByUsernameAsync(loginRequest.Username);
            if (user == null)
                throw new ArgumentException("Invalid username or password.");
            if (!PasswordHelper.VerifyPassword(loginRequest.Password, user.PasswordHash))
                throw new ArgumentException("Invalid username or password.");

            if (!user.IsEmailVerified)
                throw new ArgumentException("Please verify your email before logging in.");

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                throw new Exception("No HttpContext available.");

            List<string> roles = await _rolesRepository.GetByUserIdAsync(user.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                }
             );

            return new LoginResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = roles,
                Message = "Login successful"
            };
        }

        public async Task UpdateAsync(User user)
        {
            var existingUser = await _authRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
                throw new Exception("User not found");

            var otherUser = await _authRepository.GetByUsernameAsync(user.Username);
            if (otherUser != null && otherUser.Id != user.Id)
                throw new Exception("Username is already taken by another user.");

            existingUser.Ime = user.Name;
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;

            await _authRepository.UpdateAsync(existingUser);
        }

        public async Task UpdateAsync(Korisnici user)
        {
            var existingUser = await _authRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
                throw new Exception("User not found");

            existingUser.Ime = user.Ime;
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;

            await _authRepository.UpdateAsync(existingUser);
        }

        public async Task DeleteAsync(int id)
        {
            var user = await _authRepository.GetByIdAsync(id);
            if (user == null) throw new Exception("User not found");
            await _authRepository.DeleteAsync(user);
        }

        public async Task<Korisnici?> FindByVerificationTokenAsync(Guid token)
        {
            return await _authRepository.GetByVerificationTokenAsync(token);
        }

        public async Task ForgotPasswordAsync(string username)
        {
            Korisnici? user = await _authRepository.GetByUsernameAsync(username);
            if (user == null)
                throw new Exception("User not found");
            
            Guid resetToken = Guid.NewGuid();
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);
            await _authRepository.UpdateAsync(user);

            string resetLink = $"https://localhost:44413/forgot-password?token={resetToken}";

            await _emailService.SendEmailAsync(user.Email, "Reset password", $"Click <a href='{resetLink}'>here</a> to reset your password.");
        }

        public async Task ResetPasswordAsync(Guid token, string newPassword)
        {
            Korisnici? user = await _authRepository.GetByResetPasswordTokenAsync(token);
            if (user == null || user.ResetPasswordTokenExpiry < DateTime.UtcNow)
                throw new Exception("Invalid or expired token");

            var passwordValidation = await ValidatePasswordAsync(new PasswordValidationRequest
            {
                Password = newPassword,
                ConfirmPassword = newPassword
            });
            if (!passwordValidation.IsValid)
                throw new ArgumentException(passwordValidation.Message);

            user.PasswordHash = PasswordHelper.CalculateHashAndSalt(newPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;

            await _authRepository.UpdateAsync(user);
        }
    }
}
