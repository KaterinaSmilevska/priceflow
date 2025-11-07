using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IssuersController : Controller
    {
        private readonly IIssuersService _issuersService;

        public IssuersController(IIssuersService issuersService) => _issuersService = issuersService;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Issuer>>> GetAll()
        {
            try
            {
                IEnumerable<Issuer> securities = await _issuersService.FindAllAsync();
                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching issuers.", detail = ex.Message });
            }
        }
    }
}
