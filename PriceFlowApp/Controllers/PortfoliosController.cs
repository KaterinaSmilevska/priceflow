using DataAccess.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [Authorize(Roles = "Investor")]
    [ApiController]
    [Route("api/[controller]")]
    public class PortfoliosController : ControllerBase
    {
        private readonly IPortfoliosService _portfoliosService;
        private readonly IAuthService _authService;

        public PortfoliosController(IPortfoliosService portfoliosService, IAuthService authService)
        {
            _portfoliosService = portfoliosService;
            _authService = authService;
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAll(int userId)
        {
            try
            {
                var korisnik = _authService.FindByIdAsync(userId);
                IEnumerable<PortfolioList> portfolios = await _portfoliosService.FindUserPortfoliosAsync(korisnik.Id);
                return Ok(portfolios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolios for user.", detail = ex.Message });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> Create(CreatePortfolio portfolio)
        //{
        //    try
        //    {
        //        var korisnik = _authService.FindByIdAsync(userId);
        //        PortfolioList? createdPortfolio = await _portfoliosService.CreatePortfolio(korisnik.Id, portfolio);
        //        return Ok(createdPortfolio);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Error fetching portfolios for user.", detail = ex.Message });
        //    }
        //}
    }
}
