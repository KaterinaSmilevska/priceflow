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
        public ActionResult<IEnumerable<ThresholdResponse>> GetUserThresholds()
        {
            try
            {
                int userId = User.GetUserId();
                IEnumerable<ThresholdResponse> response = _thresholdService.GetUserThresholds(userId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching thresholds.", detail = ex.Message });
            }
        }

        [HttpGet("owned")]
        public ActionResult<IEnumerable<OwnedSecurity>> GetOwned()
        {
            try
            {
                int userId = User.GetUserId();
                IEnumerable<OwnedSecurity> response = _thresholdService.GetOwnedSecurities(userId);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching owned securities.", detail = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AddThreshold([FromBody] AddThresholdRequest request)
        {
            try
            {
                int userId = User.GetUserId();
                _thresholdService.Add(userId, request);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating threshold.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateThreshold(int id, [FromBody] UpdateThresholdRequest request)
        {
            try
            {
                int userId = User.GetUserId();
                _thresholdService.Update(userId, id, request);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating threshold.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteThreshold(int id)
        {
            try
            {
                int userId = User.GetUserId();
                _thresholdService.Delete(userId, id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting threshold.", detail = ex.Message });
            }
        }
    }
}
