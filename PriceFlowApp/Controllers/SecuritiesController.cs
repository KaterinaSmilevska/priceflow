using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecuritiesController: PriceFlowController
    {
        private readonly ISecuritiesService _securitiesService;

        public SecuritiesController(ISecuritiesService securitiesService)
        {
            _securitiesService = securitiesService;
        }

        [HttpGet("{id}")]
        public ActionResult<Security> GetById(int id)
        {
            return Execute(() => _securitiesService.FindById(id));
        }

        [HttpGet]
        public ActionResult<IEnumerable<Security>> GetAll()
        {
            return Execute(() => _securitiesService.FindAll());
        }

        [HttpGet("code/{id}")]
        public ActionResult<string> GetSecurityCode(int id)
        {
            return Execute(() => _securitiesService.FindSecurityCode(id));
        }

        [HttpGet("{code}/total-shares")]
        public ActionResult<int> GetTotalNumShares(string code)
        {
            return Execute(() => _securitiesService.FindTotalNumShares(code));
        }

        [HttpPost]
        public ActionResult<Security> Add([FromBody] AddSecurityRequest security)
        {
            return Execute(() => _securitiesService.Add(security));
        }

        [HttpPut("{id}")]
        public ActionResult<Security> Update(int id, [FromBody] UpdateSecurity security)
        {
            if(id != security.Id)
                return BadRequest(new {message = "Security Id mismatch."});

            return Execute(() => _securitiesService.Update(id, security));
        }

        [HttpDelete("{id}")]
        public ActionResult<Security> Delete(int id)
        {
            return Execute(() =>  (_securitiesService.Delete(id)));
        }

        [HttpGet("prices")]
        public ActionResult<SecurityDailyPrices> GetLatestPrices([FromQuery] string securityCode, [FromQuery] DateTime date)
        {
            return Execute(() => _securitiesService.GetLatestPrices(securityCode, date));
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<Security>> SearchByCode([FromQuery] string searchTerm)
        {
            return Execute(() => _securitiesService.SearchByCode(searchTerm));
        }
    }
}
