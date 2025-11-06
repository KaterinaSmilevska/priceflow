using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecuritiesController: ControllerBase
    {
        private readonly ISecuritiesService _securitiesService;

        public SecuritiesController(ISecuritiesService securitiesService)
        {
            _securitiesService = securitiesService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Security>>> GetAll()
        {
            try
            {
                IEnumerable<Security> securities = await _securitiesService.FindAllAsync();
                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Security>> GetById(int id)
        {
            Security? security = await _securitiesService.FindByIdAsync(id);
            if(security == null)
                return NotFound();

            return Ok(security);
        }
    }
}
