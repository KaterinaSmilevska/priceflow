using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrokersController : ControllerBase
    {
        private readonly IBrokersService _brokersService;

        public BrokersController(IBrokersService brokerService)
        {
            _brokersService = brokerService;
        }

        [Authorize(Roles = Roles.Admin + "," + Roles.Investor)]
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                IEnumerable<Broker> brokers = _brokersService.FindAll();

                return Ok(brokers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching brokers.", detail = ex.Message });
            }
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public ActionResult<Broker> AddBroker([FromBody] AddBrokerRequest broker)
        {
            try
            {
                Broker addedBroker = _brokersService.Add(broker);

                return Ok(addedBroker);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating broker.", detail = ex.Message });
            }
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpPut("{id}")]
        public ActionResult<BrokerResponse> UpdateBroker(int id, [FromBody] UpdateBrokerRequest broker)
        {
            if (id != broker.Id)
                return BadRequest(new { message = "Broker Id mismatch." });
            try
            {
                BrokerResponse updatedBroker = _brokersService.Update(id, broker);

                return Ok(updatedBroker);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating broker.", detail = ex.Message });
            }
        }

        [Authorize(Roles = Roles.Admin)]
        [HttpDelete("{id}")]
        public ActionResult<Broker> DeleteBroker(int id)
        {
            try
            {
                Broker deletedBroker = _brokersService.Delete(id);

                return Ok(deletedBroker);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting broker.", detail = ex.Message });
            }
        }
    }
}
