using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UlogaController : ControllerBase
    {
        private readonly UlogaService _ulogaService;

        public UlogaController(UlogaService ulogaService) => _ulogaService = ulogaService;

        [HttpGet("{name}")]
        public async Task<IActionResult> getUloga(String name)
        {
            try
            {
                var response = await _ulogaService.GetUlogaAsync(name);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
