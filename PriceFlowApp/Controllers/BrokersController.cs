using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [Authorize(Roles = "Инвеститор")]
    [ApiController]
    [Route("api/[controller]")]
    public class BrokersController : ControllerBase
    {
        private readonly IBrokersService _brokersService;

        public BrokersController(IBrokersService brokerService)
        {
            _brokersService = brokerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                IEnumerable<Broker> brokers = await _brokersService.FindAllAsync();

                return Ok(brokers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching brokers.", detail = ex.Message });
            }
        }
    }
}
