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
        public ActionResult<IEnumerable<TypeSecurity>> GetAll()
        {
            try
            {
                IEnumerable<TypeSecurity> typeSecurities = _typeSecurityService.FindAll();

                return Ok(typeSecurities);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching types of typeSecurities.", detail = ex.Message });
            }
        }
    }
}
