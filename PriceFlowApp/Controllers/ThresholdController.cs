using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ThresholdController : PriceFlowController
    {
        private readonly IThresholdService _thresholdService;

        public ThresholdController(IThresholdService thresholdService)
        {
            _thresholdService = thresholdService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<ThresholdResponse>> GetUserThresholds()
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _thresholdService.GetUserThresholds(userId);
            });
        }

        [HttpGet("owned")]
        public ActionResult<IEnumerable<OwnedSecurity>> GetOwned()
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _thresholdService.GetOwnedSecurities(userId);
            });
        }

        [HttpPost]
        public ActionResult<ThresholdResponse> Add([FromBody] AddThresholdRequest request)
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _thresholdService.Add(userId, request);
            });
        }

        [HttpPut("{id}")]
        public ActionResult<ThresholdResponse> Update(int id, [FromBody] UpdateThresholdRequest request)
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _thresholdService.Update(userId, id, request);
            });
        }

        [HttpDelete("{id}")]
        public ActionResult<ThresholdResponse> Delete(int id)
        {
            return Execute(() =>
            {
                int userId = User.GetUserId();
                return _thresholdService.Delete(userId, id);
            });
        }
    }
}
