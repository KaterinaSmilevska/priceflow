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

        [HttpGet("code/{id}")]
        public async Task<ActionResult<Security>> GetSecurityCode(int id)
        {
            string? code = await _securitiesService.FindSecurityCode(id);
            if(code == null)
                return NotFound();

            return Ok(code);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Security>> GetById(int id)
        {
            Security? security = await _securitiesService.FindByIdAsync(id);
            if (security == null)
                return NotFound();

            return Ok(security);
        }

        [HttpGet("{code}/total-shares")]
        public async Task<ActionResult<int>> GetTotalNumShares(string code)
        {
            try
            {
                int? totalShares = await _securitiesService.FindTotalNumSharesAsync(code);
                return Ok(totalShares);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = "Error fetching total shares.", detail = ex.Message });
            }
        }

        [HttpPost]
        //[Authorize(Roles = "Администратор")]
        public async Task<IActionResult> Create([FromBody] CreateSecurity security)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdSecurity = await _securitiesService.AddAsync(security);
            return CreatedAtAction(nameof(GetById), new { id = createdSecurity.Id }, createdSecurity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _securitiesService.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting security.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Security>> Update(int id, [FromBody] CreateSecurity updatedSecurity)
        {
            try
            {
                Security security = await _securitiesService.UpdateAsync(id, updatedSecurity);
                return Ok(security);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating security.", detail = ex.Message });
            }
        }
    }
}
