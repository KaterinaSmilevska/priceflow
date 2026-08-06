using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRolesService _rolesService;
        private readonly IBrokersService _brokersService;

        public AuthController(IAuthService authService, IRolesService rolesService, IBrokersService brokersService)
        {
            _authService = authService;
            _rolesService = rolesService;
            _brokersService = brokersService;
        }

        [HttpPost("register")]
        public ActionResult<RegisterResponse> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                RegisterResponse response = _authService.Register(registerRequest);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                LoginResponse response = await _authService.Login(request);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new { success = true, message = "Logged out successfully." });
        }

        [HttpGet("ulogi")]
        public ActionResult<List<string>> GetRolesNames()
        {
            try
            {
                List<string> roles = _rolesService.FindNames();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error has occured while fetching roles.", detail = ex.Message });
            }
        }

        [HttpGet("check-username/{username}")]
        public ActionResult<bool> CheckUsername(string username)
        {
            try
            {
                bool exists = _authService.UsernameExists(username);

                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking username.", detail = ex.Message });
            }
        }

        [HttpPost("validate-password")]
        public ActionResult<PasswordValidationResponse> ValidatePassword([FromBody] PasswordValidationRequest request)
        {
            try
            {
                PasswordValidationResponse response = _authService.ValidatePassword(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while validating password.", detail = ex.Message });
            }
        }

        [HttpPost("validate-email")]
        public ActionResult<EmailValidationResponse> ValidateEmail([FromBody] EmailValidationRequest request)
        {
            try
            {
                EmailValidationResponse response = _authService.ValidateEmail(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while validating email.", detail = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                _authService.ForgotPassword(request.Username);

                return Ok(new { message = "Reset link has been sent to your email." });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                _authService.ResetPassword(request.Token, request.NewPassword);

                return Ok(new { message = "Password reset successful." });
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            if (!(User.Identity?.IsAuthenticated ?? false))
                return Ok(new { isLoggedIn = false });

            var username = User.Identity?.Name;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            return Ok(new
            {
                isLoggedIn = true,
                username,
                userId,
                roles
            });
        }

        [HttpGet("users")]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            try
            {
                IEnumerable<User> users = _authService.FindAll();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching users.", detail = ex.Message });
            }
        }

        [HttpGet("users/{id}")]
        public ActionResult<User> GetUser(int id)
        {
            User user = _authService.FindById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPut("users/{id}")]
        public ActionResult<User> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest(new { message = "User Id mismatch." });
            try
            {
                User updatedUser = _authService.Update(id, user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        [HttpDelete("users/{id}")]
        public ActionResult<User> DeleteUser(int id)
        {
            try
            {
                User deletedUser = _authService.Delete(id);

                return Ok(deletedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        [HttpGet("verify-email")]
        public IActionResult VerifyEmail([FromQuery] Guid token)
        {
            try
            {
                _authService.VerifyEmail(token);

                return Redirect("https://localhost:44413/register?verified=true");
            }
            catch(ValidationException)
            {
                return BadRequest("Invalid or expired verification link.");
            }
            
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during email verification.", detail = ex.Message });
            }
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

