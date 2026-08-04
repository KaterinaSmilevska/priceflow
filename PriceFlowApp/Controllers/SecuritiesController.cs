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

        [HttpGet("{id}")]
        public ActionResult<Security> GetById(int id)
        {
            Security? security = _securitiesService.FindById(id);
            if (security == null)
                return NotFound();

            return Ok(security);
        }

        [HttpGet]
        public ActionResult<IEnumerable<Security>> GetAll()
        {
            try
            {
                IEnumerable<Security> securities = _securitiesService.FindAll();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }

        [HttpGet("code/{id}")]
        public ActionResult<Security> GetSecurityCode(int id)
        {
            string? code = _securitiesService.FindSecurityCode(id);
            if(code == null)
                return NotFound();

            return Ok(code);
        }

        [HttpGet("{code}/total-shares")]
        public ActionResult<int> GetTotalNumShares(string code)
        {
            try
            {
                int? totalShares = _securitiesService.FindTotalNumShares(code);

                return Ok(totalShares);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = "Error fetching total shares.", detail = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateSecurity security)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdSecurity = _securitiesService.Add(security);

            return CreatedAtAction(nameof(GetById), new { id = createdSecurity.Id }, createdSecurity);
        }

        [HttpPut("{id}")]
        public ActionResult<Security> Update([FromBody] UpdateSecurity updatedSecurity)
        {
            try
            {
                Security security = _securitiesService.Update(updatedSecurity);

                return Ok(security);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating security.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _securitiesService.Delete(id);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting security.", detail = ex.Message });
            }
        }

        [HttpGet("prices")]
        public ActionResult<SecurityDailyPrices> GetLatestPrices([FromQuery] string securityCode, [FromQuery] DateTime date)
        {
            try
            {
                SecurityDailyPrices? result = _securitiesService.GetLatestPrices(securityCode, date);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching latest prices.", detail = ex.Message });
            }
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<Security>> SearchByCode([FromQuery] string searchTerm)
        {
            try
            {
                IEnumerable<Security> securities = _securitiesService.SearchByCode(searchTerm);

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching securities.", detail = ex.Message });
            }
        }
    }
}
