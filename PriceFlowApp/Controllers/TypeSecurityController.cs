using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/type-security")]
    public class TypeSecurityController : PriceFlowController
    {
        private readonly ITypeSecurityService _typeSecurityService;

        public TypeSecurityController(ITypeSecurityService typeSecurityService)
        {
            _typeSecurityService = typeSecurityService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TypeSecurity>> GetAll()
        {
            return Execute(()  => _typeSecurityService.FindAll());
        }
    }
}
