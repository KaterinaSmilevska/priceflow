using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : PriceFlowController
    {
        private readonly RolesService _rolesService;

        public RolesController(RolesService roleService)
        {
            _rolesService = roleService;
        }

        [HttpGet("{name}")]
        public ActionResult<Role> GetRole(string name)
        {
            return Execute(() => _rolesService.FindByName(name));
        }

        [HttpGet]
        public ActionResult<List<string>> GetRoleNames()
        {
            return Execute(() => _rolesService.FindNames());
        }
    }
}
