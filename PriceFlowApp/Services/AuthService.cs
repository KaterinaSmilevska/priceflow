using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowSecurity;
using System.Text.RegularExpressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PriceFlowApp.Exceptions;

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

        public Korisnici? FindById(int id)
        {
            return _authRepository
                .GetById(id);
        }

        public Korisnici? FindByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ValidationException("USERNAME_VALIDATION_REQUIRED", "Username cannot be null or empty.");

            var korisnik = _authRepository.GetByUsername(username);

            if (korisnik == null)
                throw new NotFoundException("USER_NOT_FOUND", $"User with username '{username}' not found.");

            return korisnik;
        }

        public Korisnici? FindByVerificationToken(Guid token)
        {
            return _authRepository.GetByVerificationToken(token);
        }

        public IEnumerable<User> FindAll()
        {
            var users = _authRepository.GetAll();

            return users.Select(item => new User
            {
                Id = item.Id,
                Name = item.Ime,
                Username = item.Username,
                Email = item.Email,
                Roles = item.KorisniciUlogi.Select(ku => ku.Uloga.Ime).ToList()
            });
        }

        public User Update(User user)
        {
            var existingUser = _authRepository.GetById(user.Id);
            if (existingUser == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");

            var otherUser = _authRepository.GetByUsername(user.Username);

            if (string.IsNullOrWhiteSpace(user.Name))
                throw new ValidationException("NAME_VALIDATION_REQUIRED", "Name cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ValidationException("USERNAME_VALIDATION_REQUIRED", "Username cannot be null or empty");

            if (otherUser != null && otherUser.Id != user.Id)
                throw new AlreadyExistsException("USERNAME_EXISTS", "Username is already taken by another user.");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ValidationException("EMAIL_VALIDATION_REQUIRED", "Email cannot be null or empty.");

            var emailValidation = ValidateEmail(new EmailValidationRequest
            {
                Email = user.Email
            });

            if (!emailValidation.IsValid)
                throw new ValidationException("INVALID_EMAIL", emailValidation.Message);

            existingUser.Ime = user.Name;
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;

            _authRepository.Update(existingUser);

            return new User
            {
                Id = existingUser.Id,
                Name = existingUser.Ime,
                Username = existingUser.Username,
                Email = existingUser.Email,
                Roles = existingUser.KorisniciUlogi.Select(x => x.Uloga.Ime).ToList()
            };
        }

        public Korisnici Update(Korisnici user)
        {
            var existingUser = _authRepository.GetById(user.Id);
            if (existingUser == null)
                throw new Exception("User not found");

            var emailValidation = ValidateEmail(new EmailValidationRequest
            {
                Email = user.Email
            });

            if (!emailValidation.IsValid)
                throw new ArgumentException(emailValidation.Message);

            existingUser.Ime = user.Ime;
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;

            _authRepository.Update(existingUser);

            return existingUser;
        }

        public User Delete(int id)
        {
            Korisnici? existingUser = _authRepository.GetById(id);

            if (existingUser == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");

            _authRepository.Delete(existingUser);

            return new User
            {
                Id = existingUser.Id,
                Name = existingUser.Ime,
                Username= existingUser.Username,
                Email = existingUser.Email,
                Roles = existingUser.KorisniciUlogi.Select(x => x.Uloga.Ime).ToList()
            };
        }


        public RegisterResponse Register(RegisterRequest registerRequest)
        {
            if (string.IsNullOrWhiteSpace(registerRequest.Name) ||
                string.IsNullOrWhiteSpace(registerRequest.Username) ||
                string.IsNullOrWhiteSpace(registerRequest.Email) ||
                string.IsNullOrWhiteSpace(registerRequest.Password))
                throw new ValidationException("VALIDATION_REQUIRED_FIELD", "All fields are required.");

            var passwordValidation = ValidatePassword(new PasswordValidationRequest
            {
                Password = registerRequest.Password,
                ConfirmPassword = registerRequest.ConfirmPassword
            });
            if (!passwordValidation.IsValid)
                throw new ValidationException("PASSWORD_VALIDATION_REQUIRED", passwordValidation.Message);

            var emailValidation = ValidateEmail(new EmailValidationRequest
            {
                Email = registerRequest.Email,
            });
            if (!emailValidation.IsValid)
                throw new ValidationException("EMAIL_VALIDATION_REQUIRED", emailValidation.Message);

            if (_authRepository.GetByUsername(registerRequest.Username) != null)
                throw new AlreadyExistsException("USERNAME_EXISTS", "Username already exists.");

            var validRoleNames = _rolesRepository.GetNames();
            if (registerRequest.RoleNames.Any() && registerRequest.RoleNames.Any(name => !validRoleNames.Contains(name)))
                throw new ValidationException("INVALID_ROLES", "One or more role names are invalid.");

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

            _authRepository.Add(user);

            var roleIds = _rolesRepository.GetIdsByNames(registerRequest.RoleNames);
            foreach (var roleId in roleIds)
            {
                _usersRolesRepository.Add(new KorisniciUlogi
                {
                    KorisnikId = user.Id,
                    UlogaId = roleId
                });
            }

            var verificationLink = $"https://localhost:44413/api/auth/verify-email?token={token}";
            _emailService.SendEmail(user.Email, "Verify your PriceFlow account",
                $"<p>Welcome to PriceFlow, {user.Ime}!</p>" + $"<p>Please verify your email by clicking the link below:</p>"
                + $"<a href='{verificationLink}'>Verify Email</a>");

            return new RegisterResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = registerRequest.RoleNames,
                Message = "Registration successful"
            };
        }


        public LoginResponse Login(LoginRequest loginRequest)
        {
            if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                throw new ValidationException("VALIDATION_REQUIRED", "Username and password are required.");
            }

            var user = _authRepository.GetByUsername(loginRequest.Username);
            if (user == null)
                throw new UnauthorizedException("INVALID_CREDENTIALS", "Invalid username or password.");
            if (!PasswordHelper.VerifyPassword(loginRequest.Password, user.PasswordHash))
                throw new UnauthorizedException("INVALID_CREDENTIALS", "Invalid username or password.");

            if (!user.IsEmailVerified)
                throw new UnauthorizedException("EMAIL_NOT_VERIFIED", "Please verify your email before logging in.");

            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                throw new Exception("No HttpContext available.");

            List<string> roles = _rolesRepository.GetByUserId(user.Id);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            httpContext.SignInAsync(
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

        public PasswordValidationResponse ValidatePassword(PasswordValidationRequest request)
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
        }

        public EmailValidationResponse ValidateEmail(EmailValidationRequest request)
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
        }

        public void ForgotPassword(string username)
        {
            Korisnici? user = _authRepository.GetByUsername(username);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");
            
            Guid resetToken = Guid.NewGuid();
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);
            _authRepository.Update(user);

            string resetLink = $"https://localhost:44413/forgot-password?token={resetToken}";

            _emailService.SendEmail(user.Email, "Reset password", $"Click <a href='{resetLink}'>here</a> to reset your password.");
        }

        public void ResetPassword(Guid token, string newPassword)
        {
            Korisnici? user = _authRepository.GetByResetPasswordToken(token);
            if (user == null || user.ResetPasswordTokenExpiry < DateTime.UtcNow)
                throw new ValidationException("INVALID_RESET_TOKEN", "Invalid or expired token");

            var passwordValidation = ValidatePassword(new PasswordValidationRequest
            {
                Password = newPassword,
                ConfirmPassword = newPassword
            });
            if (!passwordValidation.IsValid)
                throw new ArgumentException(passwordValidation.Message);

            user.PasswordHash = PasswordHelper.CalculateHashAndSalt(newPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;

            _authRepository.Update(user);
        }

        public bool UsernameExists(string username)
        {
            return _authRepository.UsernameExists(username);
        }
    }
}
