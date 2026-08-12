using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;
using System.Security.Claims;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : PriceFlowController
    {
        private readonly IAuthService _authService;
        private readonly IRolesService _rolesService;

        public AuthController(IAuthService authService, IRolesService rolesService)
        {
            _authService = authService;
            _rolesService = rolesService;
        }

        [HttpPost("register")]
        public ActionResult<RegisterResponse> Register([FromBody] RegisterRequest registerRequest)
        {
            return Execute(() => _authService.Register(registerRequest));   
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            return await ExecuteAsync(() => _authService.Login(request));
        }

        [HttpPost("logout")]
        public async Task<ActionResult<LogoutResponse>> Logout()
        {
            return await ExecuteAsync(async () =>
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return new LogoutResponse
                {
                    Success = true,
                    Message = "Logged out successfully!"
                };
            });
        }

        [HttpGet("ulogi")]
        public ActionResult<List<string>> GetRolesNames()
        {
            return Execute(() => _rolesService.FindNames());
        }

        [HttpGet("check-username/{username}")]
        public ActionResult<bool> CheckUsername(string username)
        {
            return Execute(() => _authService.UsernameExists(username));
        }

        [HttpPost("validate-password")]
        public ActionResult<PasswordValidationResponse> ValidatePassword([FromBody] PasswordValidationRequest request)
        {
            return Execute(() => _authService.ValidatePassword(request));
        }

        [HttpPost("validate-email")]
        public ActionResult<EmailValidationResponse> ValidateEmail([FromBody] EmailValidationRequest request)
        {
            return Execute(() => _authService.ValidateEmail(request));
        }

        [HttpPost("forgot-password")]
        public ActionResult<MessageResponse> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            return Execute(() =>
            {
                _authService.ForgotPassword(request.Username);

                return new MessageResponse
                {
                    Message = "Reset link has been sent to your email."
                };
            });
        }

        [HttpPost("reset-password")]
        public ActionResult<MessageResponse> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            return Execute(() =>
            {
                _authService.ResetPassword(request.Token, request.NewPassword);

                return new MessageResponse
                {
                    Message = "Password reset successful."
                };
            });
        }

        [HttpGet("status")]
        public ActionResult<SessionStatusResponse> Status()
        {
            return Execute(() =>
            {
                if (!(User.Identity?.IsAuthenticated ?? false))
                {
                    return new SessionStatusResponse
                    {
                        IsLoggedIn = false
                    };
                }
                var username = User.Identity?.Name;
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var roles = User.Claims
                    .Where(c => c.Type == ClaimTypes.Role)
                    .Select(c => c.Value)
                    .ToList();

                return new SessionStatusResponse
                {
                    IsLoggedIn = true,
                    Username = username,
                    UserId = userId,
                    Roles = roles
                };
            });
        }

        [HttpGet("users")]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            return Execute(() => _authService.FindAll());
        }

        [HttpGet("users/{id}")]
        public ActionResult<User> GetUser(int id)
        {
            return Execute(() => _authService.FindById(id));
        }

        [HttpPut("users/{id}")]
        public ActionResult<User> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest(new { message = "User Id mismatch." });

            return Execute(() => _authService.Update(id, user));
        }

        [HttpDelete("users/{id}")]
        public ActionResult<User> DeleteUser(int id)
        {
            return Execute(() => _authService.Delete(id));
        }

        [HttpGet("verify-email")]
        public ActionResult<MessageResponse> VerifyEmail([FromQuery] Guid token)
        {
            return Execute(() =>
            {
                _authService.VerifyEmail(token);

                return new MessageResponse
                {
                    Message = "Email verification successful."
                };
            });
        }

        [AllowAnonymous]
        [HttpGet("session-test")]
        public IActionResult SessionTest()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var username = HttpContext.Session.GetString("Username");
            var roles = HttpContext.Session.GetString("Roles");

            return Ok(new { userId, username, roles });
        }
    }
} 

