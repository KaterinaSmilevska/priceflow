using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrokerController : ControllerBase
    {
        private readonly IBrokerService _brokerService;

       public BrokerController(IBrokerService brokerService) => _brokerService = brokerService;

        [HttpGet("{kompanija}")]
        public async Task<IActionResult> GetBrokerByKompanijaAsyncs(string kompanija)
        {
            try
            {
                var broker = await _brokerService.GetBrokerByKompanijaAsync(kompanija);
                return Ok(broker);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { messaage = ex.Message });
            }
        }
    }
}
