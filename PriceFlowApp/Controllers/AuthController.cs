using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            try
            {
                var response = await _authService.RegisterAsync(registerRequest);
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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            if (Request.Cookies.ContainsKey(".AspNetCore.Session")) 
            {
                Response.Cookies.Delete(".AspNetCore.Session");
            }
            return Ok(new { success = true, message = "Logged out successfully." });
        }

        [HttpGet("ulogi")]
        public async Task<IActionResult> GetUlogaNames()
        {
            try
            {
                var ulogas = await _authService.GetUlogaNamesAsync();
                return Ok(ulogas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error has occured while fetching roles.", detail = ex.Message });
            }
        }

        [HttpGet("check-username/{username}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            try
            {
                var exists = await _authService.UsernameExistsAsync(username);
                return Ok(new { exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while checking username.", detail = ex.Message });
            }
        }

        [HttpPost("validate-password")]
        public async Task<IActionResult> ValidatePassword([FromBody] PasswordValidationRequest request)
        {
            try
            {
                var response = await _authService.ValidatePasswordAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while validating password.", detail = ex.Message });
            }
        }

        [HttpPost("validate-email")]
        public async Task<IActionResult> ValidateEmail([FromBody] EmailValidationRequest request)
        {
            try
            {
                var response = await _authService.ValidateEmailAsync(request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while validating email.", detail = ex.Message });
            }
        }

        [HttpGet("status")]
        public IActionResult Status()
        {
            var username = HttpContext.Session.GetString("Username");
            if (!string.IsNullOrEmpty(username))
                return Ok(new { isLoggedIn = true, username });
            else
                return Ok(new { isLoggedIn = false });
        }
    }
} 

