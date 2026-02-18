using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ThresholdController : ControllerBase
    {
        private readonly IThresholdService _thresholdService;

        public ThresholdController(IThresholdService thresholdService)
        {
            _thresholdService = thresholdService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ThresholdResponse>>> GetUserThresholds()
        {
            try
            {
                int userId = User.GetUserId();
                IEnumerable<ThresholdResponse> response = await _thresholdService.GetUserThresholdsAsync(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching thresholds.", detail = ex.Message });
            }
        }

        [HttpGet("owned")]
        public async Task<ActionResult<IEnumerable<OwnedSecurity>>> GetOwned()
        {
            try
            {
                int userId = User.GetUserId();
                IEnumerable<OwnedSecurity> response = await _thresholdService.GetOwnedSecuritiesAsync(userId);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching owned securities.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddThreshold([FromBody] CreateThresholdRequest request)
        {
            try
            {
                int userId = User.GetUserId();
                await _thresholdService.AddAsync(userId, request);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating threshold.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateThreshold(int id, [FromBody] UpdateThresholdRequest request)
        {
            try
            {
                int userId = User.GetUserId();
                await _thresholdService.UpdateAsync(userId, id, request);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating threshold.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteThreshold(int id)
        {
            try
            {
                int userId = User.GetUserId();
                await _thresholdService.DeleteAsync(userId, id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting threshold.", detail = ex.Message });
            }
        }
    }
}
