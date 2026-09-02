using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrokersController : PriceFlowController
    {
        private readonly IBrokersService _brokersService;

        public BrokersController(IBrokersService brokerService)
        {
            _brokersService = brokerService;
        }

        [Authorize(Roles = Roles.Admin + "," + Roles.Investor)]
        [HttpGet]
        public ActionResult<IEnumerable<BrokerResponse>> GetAll()
        {
            return Execute(() => _brokersService.FindAll());
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public ActionResult<BrokerResponse> AddBroker([FromBody] AddBrokerRequest broker)
        {
            return Execute(() => _brokersService.Add(broker));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public ActionResult<BrokerResponse> UpdateBroker(int id, [FromBody] UpdateBrokerRequest broker)
        {
            if (id != broker.Id)
                return BadRequest(new { message = "Broker Id mismatch." });

            return Execute(() => _brokersService.Update(id, broker));
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public ActionResult<BrokerResponse> DeleteBroker(int id)
        {
            return Execute(() =>  _brokersService.Delete(id));
        }
    }
}
