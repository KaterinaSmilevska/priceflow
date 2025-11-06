using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly RolesService _rolesService;

        public RolesController(RolesService roleService) => _rolesService = roleService;

        [HttpGet("{name}")]
        public async Task<IActionResult> GetRole(string name)
        {
            try
            {
                var response = await _rolesService.FindByNameAsync(name);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRoleNames()
        {
            try
            {
                var response = await _rolesService.FindNamesAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
