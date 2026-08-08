using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
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
        public ActionResult<Role> GetRole(string name)
        {
            try
            {
                Role role = _rolesService.FindByName(name);

                return Ok(role);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult<List<string>> GetRoleNames()
        {
            try
            {
                List<string> roleNames = _rolesService.FindNames();

                return Ok(roleNames);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
