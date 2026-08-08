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
                IEnumerable<ThresholdResponse> userThresholds = _thresholdService.GetUserThresholds(userId);

                return Ok(userThresholds);
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
                IEnumerable<OwnedSecurity> ownedSecurities = _thresholdService.GetOwnedSecurities(userId);

                return Ok(ownedSecurities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching owned securities.", detail = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult<ThresholdResponse> AddThreshold([FromBody] AddThresholdRequest request)
        {
            try
            {
                int userId = User.GetUserId();
                ThresholdResponse addedThreshold = _thresholdService.Add(userId, request);

                return Ok(addedThreshold);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating threshold.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult<ThresholdResponse> UpdateThreshold(int id, [FromBody] UpdateThresholdRequest request)
        {
            try
            {
                int userId = User.GetUserId();
                ThresholdResponse updatedThreshold = _thresholdService.Update(userId, id, request);

                return Ok(updatedThreshold);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating threshold.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<ThresholdResponse> DeleteThreshold(int id)
        {
            try
            {
                int userId = User.GetUserId();
                ThresholdResponse deletedThreshold = _thresholdService.Delete(userId, id);

                return Ok(deletedThreshold);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting threshold.", detail = ex.Message });
            }
        }
    }
}
