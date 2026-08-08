using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Helpers;
using PriceFlowSecurity;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace PriceFlowApp.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IRolesRepository _rolesRepository;
        private readonly IUsersRolesRepository _usersRolesRepository;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IAuthRepository korisnikRepository, IRolesRepository rolesRepository, IUsersRolesRepository usersRolesRepository, 
            IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _authRepository = korisnikRepository;
            _rolesRepository = rolesRepository;
            _usersRolesRepository = usersRolesRepository;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public User FindById(int id)
        {
            Korisnici user = GetUserById(id);

            return MapToUser(user);
        }

        public User FindByUsername(string username)
        {
            ValidationHelper.ValidateRequiredField(username, "Username", "USERNAME_VALIDATION_REQUIRED");

            Korisnici user = GetUserByUsername(username);

            return MapToUser(user);
        }

        public User FindByVerificationToken(Guid token)
        {
            Korisnici? user = GetUserByVerificationToken(token);

            return MapToUser(user);
        }

        public IEnumerable<User> FindAll()
        {
            IEnumerable<Korisnici> users = _authRepository.GetAll();

            return users
                .Select(MapToUser)
                .ToList();
        }

        public User Update(int id, User user)
        {
            Korisnici existingUser = GetUserById(id);

            ValidationHelper.ValidateRequiredField(user.Username, "Username", "USERNAME_VALIDATION_REQUIRED");
            ValidateUsernameAvailability(user.Username, user.Id);
            ValidationHelper.ValidateRequiredField(user.Name, "Name", "NAME_VALIDATION_REQUIRED");
            ValidationHelper.ValidateRequiredField(user.Email, "Email", "EMAIL_VALIDATION_REQUIRED");
            ValidateEmailFormat(user.Email);

            existingUser.Ime = user.Name;
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            existingUser.IsEmailVerified = user.IsEmailVerified;

            Korisnici updatedUser = _authRepository.Update(existingUser);

            return MapToUser(updatedUser);
        }

        public User Delete(int id)
        {
            Korisnici existingUser = GetUserById(id);

            Korisnici deletedUser = _authRepository.Delete(existingUser);

            return MapToUser(deletedUser);
        }

        public RegisterResponse Register(RegisterRequest registerRequest)
        {
            ValidateRegistrationFields(registerRequest);
            ValidatePasswordFormat(registerRequest.Password, registerRequest.ConfirmPassword);
            ValidateEmailFormat(registerRequest.Email);
            ValidateUsernameAvailability(registerRequest.Username);
            ValidateRoles(registerRequest.RoleNames);

            byte[] fullPasswordBytes = PasswordHelper.CalculateHashAndSalt(registerRequest.Password);

            var token = Guid.NewGuid();

            Korisnici user = new Korisnici
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

            Korisnici addedUser = _authRepository.Add(user);

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


        public async Task<LoginResponse> Login(LoginRequest loginRequest)
        {
            ValidateLoginFields(loginRequest);

            Korisnici user = GetUserByUsername(loginRequest.Username);

            ValidateCredentials(loginRequest.Password, user);
            ValidateEmailVerification(user);

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

        public void VerifyEmail(Guid token)
        {
            Korisnici user = GetUserByVerificationToken(token);

            if (user.IsEmailVerified)
                return;

            user.IsEmailVerified = true;
            user.EmailVerificationToken = null;

            _authRepository.Update(user);
        }

        public void ForgotPassword(string username)
        {
            Korisnici user = GetUserByUsername(username);
            
            Guid resetToken = Guid.NewGuid();
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1);
            _authRepository.Update(user);

            string resetLink = $"https://localhost:44413/forgot-password?token={resetToken}";

            _emailService.SendEmail(user.Email, "Reset password", $"Click <a href='{resetLink}'>here</a> to reset your password.");
        }

        public void ResetPassword(Guid token, string newPassword)
        {
            Korisnici? user = GetByResetPasswordToken(token);

            ValidatePasswordFormat(newPassword, newPassword);

            user.PasswordHash = PasswordHelper.CalculateHashAndSalt(newPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;

            _authRepository.Update(user);
        }

        public bool UsernameExists(string username)
        {
            return _authRepository.UsernameExists(username);
        }

        private void ValidateRegistrationFields(RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.Username)
                || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
                throw new ValidationException("VALIDATION_REQUIRED_FIELD", "All fields are required.");
        }

        private void ValidateLoginFields(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                throw new ValidationException("VALIDATION_REQUIRED_FIELD", "Username and password are required.");
        }

        private Korisnici GetUserById(int  userId)
        {
            Korisnici? user = _authRepository.GetById(userId);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND", "User not found.");

            return user;
        }

        private Korisnici GetUserByUsername(string username)
        {
            Korisnici? user = _authRepository.GetByUsername(username);
            if (user == null)
                throw new NotFoundException("USER_NOT_FOUND", $"User with '{username}' not found.");

            return user;
        }

        private Korisnici GetUserByVerificationToken(Guid token)
        {
            Korisnici? user = _authRepository.GetByVerificationToken(token);
            if (user == null)
                throw new ValidationException("INVALID_VERIFICATION_TOKEN", "Verification token is invalid.");

            return user;
        }

        private Korisnici GetByResetPasswordToken(Guid token)
        {
            Korisnici? user = _authRepository.GetByResetPasswordToken(token);
            if (user == null || user.ResetPasswordTokenExpiry < DateTime.UtcNow)
                throw new ValidationException("INVALID_RESET_TOKEN", "Invalid or expired token");

            return user;
        }

        private void ValidateEmailFormat(string email)
        {
            EmailValidationResponse emailValidation = ValidateEmail(new EmailValidationRequest
            {
                Email = email
            });

            if (!emailValidation.IsValid)
                throw new ValidationException("EMAIL_VALIDATION_REQUIRED", emailValidation.Message);
        }

        private void ValidatePasswordFormat(string password, string confirmPassword)
        {
            PasswordValidationResponse passwordValidation = ValidatePassword(new PasswordValidationRequest
            {
                Password = password,
                ConfirmPassword = confirmPassword
            });

            if(!passwordValidation.IsValid)
                throw new ValidationException("PASSWORD_VALIDATION_REQUIRED", passwordValidation.Message);
        }

        private void ValidateRoles(IEnumerable<string> roleNames)
        {
            IEnumerable<string> validRoleNames = _rolesRepository.GetNames();
            if(roleNames.Any(name => !validRoleNames.Contains(name)))
                throw new ValidationException("INVALID_ROLES", "One or more role names are invalid.");
        }

        private void ValidateUsernameAvailability(string username, int? userId = null)
        {
            Korisnici? existingUser = _authRepository.GetByUsername(username);
            if (existingUser != null && existingUser.Id != userId)
                throw new AlreadyExistsException("USERNAME_ALREADY_EXISTS", "The username is taken by another user.");
        }

        private void ValidateCredentials(string password, Korisnici user)
        {
            if (!PasswordHelper.VerifyPassword(password, user.PasswordHash))
                throw new UnauthorizedException("INVALID_CREDENTIALS", "Invalid username or password.");
        }

        private void ValidateEmailVerification(Korisnici user)
        {
            if (!user.IsEmailVerified)
                throw new UnauthorizedException("EMAIL_NOT_VERIFIED", "Please verify your email before logging in.");
        }

        private User MapToUser(Korisnici user)
        {
            return new User
            {
                Id = user.Id,
                Name = user.Ime,
                Username = user.Username,
                Email = user.Email,
                Roles = user.KorisniciUlogi.Select(x => x.Uloga.Ime).ToList(),
                IsEmailVerified = user.IsEmailVerified
            };
        }
    }
}
