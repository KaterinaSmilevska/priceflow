using DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/type-security")]
    public class TypeSecurityController : ControllerBase
    {
        private readonly ITypeSecurityService _typeSecurityService;

        public TypeSecurityController(ITypeSecurityService typeSecurityService)
        {
            _typeSecurityService = typeSecurityService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TipHv>>> GetAll()
        {
            try
            {
                IEnumerable<TypeSecurity> securities = await _typeSecurityService.FindAllAsync();

                return Ok(securities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching types of securities.", detail = ex.Message });
            }
        }
    }
}
