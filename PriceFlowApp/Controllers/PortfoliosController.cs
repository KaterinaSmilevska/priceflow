using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;
using PriceFlowSecurity;

namespace PriceFlowApp.Controllers
{
    [Authorize(Roles = "Инвеститор")]
    [ApiController]
    [Route("api/[controller]")]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfoliosService _portfoliosService;

        public PortfoliosController(IPortfoliosService portfoliosService)
        {
            _portfoliosService = portfoliosService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                int userId = User.GetUserId();
                IEnumerable<Portfolio> portfolios = await _portfoliosService.FindUserPortfoliosAsync(userId);
                return Ok(portfolios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolios for user.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePortfolio portfolio)
        {
            try
            {
                int userId = User.GetUserId();
                Portfolio createdPortfolio = await _portfoliosService.CreatePortfolio(userId, portfolio);
                return Ok(createdPortfolio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating portfolio.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePortfolio portfolio)
        {
            try
            {
                int userId = User.GetUserId();
                Portfolio updatedPortfolio = await _portfoliosService.UpdatePortfolio(id, userId, portfolio);
                return Ok(updatedPortfolio);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updading portoflio..", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                int userId = User.GetUserId();
                await _portfoliosService.DeletePortfolio(id, userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting portoflio..", detail = ex.Message });
            }
        }
    }
}
