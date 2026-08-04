using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RolesService _rolesService;

        public RolesController(RolesService roleService)
        {
            _rolesService = roleService;
        }

        [HttpGet("{name}")]
        public IActionResult GetRole(string name)
        {
            try
            {
                var response = _rolesService.FindByName(name);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetRoleNames()
        {
            try
            {
                var response = _rolesService.FindNames();

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
