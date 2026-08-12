using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssuersController : PriceFlowController
    {
        private readonly IIssuersService _issuersService;

        public IssuersController(IIssuersService issuersService)
        {
            _issuersService = issuersService;
        } 

        [HttpGet]
        public ActionResult<IEnumerable<Issuer>> GetAll()
        {
            return Execute(() => _issuersService.FindAll());
        }
    }
}
