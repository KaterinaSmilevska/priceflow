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
        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                var response = _authService.Register(registerRequest);

                return Ok(response);
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(new { message = ex.Message });
                }

            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = _authService.Login(request);

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new { success = true, message = "Logged out successfully." });
        }

        [HttpGet("ulogi")]
        public IActionResult GetRolesNames()
        {
            try
            {
                var roles = _rolesService.FindNames();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error has occured while fetching roles.", detail = ex.Message });
            }
        }

        [HttpGet("check-username/{username}")]
        public IActionResult CheckUsername(string username)
        {
            try
            {
                var exists = _authService.UsernameExists(username);

                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking username.", detail = ex.Message });
            }
        }

        [HttpPost("validate-password")]
        public IActionResult ValidatePassword([FromBody] PasswordValidationRequest request)
        {
            try
            {
                var response = _authService.ValidatePassword(request);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while validating password.", detail = ex.Message });
            }
        }

        [HttpPost("validate-email")]
        public IActionResult ValidateEmail([FromBody] EmailValidationRequest request)
        {
            try
            {
                var response = _authService.ValidateEmail(request);

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

                return Ok(new { message = "Reset link has been sent to your email. " });
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
            if (!User.Identity!.IsAuthenticated)
                return Ok(new { isLoggedIn = false });

            var username = User.Identity!.Name;
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
        public IActionResult GetUsers()
        {
            try
            {
                var users = _authService.FindAll();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching users.", detail = ex.Message });
            }
        }

        [HttpGet("users/{id}")]
        public IActionResult GetUser(int id)
        {
            var foundUser = _authService.FindById(id);
            if (foundUser == null)
                return NotFound();

            User user = new User
            {
                Id = foundUser.Id,
                Name = foundUser.Ime,
                Username = foundUser.Username,
                Email = foundUser.Email
            };

            return Ok(user);
        }

        [HttpPut("users/{id}")]
        public IActionResult UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest(new { message = "User Id mismatch." });
            try
            {
                _authService.Update(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        [HttpDelete("users/{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                _authService.Delete(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = ex.Message });
            }
        }

        [HttpGet("brokers")]
        public IActionResult GetBrokers()
        {
            try
            {
                var brokers = _brokersService.FindAll();

                return Ok(brokers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching brokers.", detail = ex.Message });
            }
        }

        [HttpPost("brokers")]
        public IActionResult AddBroker([FromBody] CreateBrokerRequest broker)
        {
            try
            {
                Broker result = _brokersService.Add(broker);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating broker.", detail = ex.Message });
            }
        }

        [HttpPut("brokers/{id}")]
        public IActionResult UpdateBroker(int id, [FromBody] UpdateBrokerRequest broker)
        {
            try
            {
                BrokerResponse result = _brokersService.Update(broker);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating broker.", detail = ex.Message });
            }
        }

        [HttpDelete("brokers/{id}")]
        public IActionResult DeleteBroker(int id)
        {
            try
            {
                _brokersService.Delete(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting broker.", detail = ex.Message });
            }
        }

        [HttpGet("verify-email")]
        public IActionResult VerifyEmail([FromQuery] Guid token)
        {
            try
            {
                var user = _authService.FindByVerificationToken(token);
                if (user == null)
                    return BadRequest("Invalid  or expired verification link.");

                if (user.IsEmailVerified)
                    return Redirect("https://localhost:44413/register?verified=true");

                user.IsEmailVerified = true;
                user.EmailVerificationToken = null;

                _authService.Update(user);

                return Redirect("https://localhost:44413/register?verified=true");
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

