//using Microsoft.AspNetCore.Mvc;
//using PriceFlowApp.Services;

//namespace PriceFlowApp.Controllers
//{
//    [ApiController]
//    [Route("/api/portfolio-details")]
//    public class PortfolioDetailsController: ControllerBase
//    {
//        private readonly IPortfolioDetailsService _portfolioDetailsService;

//        public PortfolioDetailsController(IPortfolioDetailsService portfolioDetailsService)
//        {
//            _portfolioDetailsService = portfolioDetailsService;
//        }

//        [HttpGet("{id}/holdings")]
//        public async Task<IActionResult> GetHoldings(int id)
//        {
//            try
//            {
//                return Ok(await _portfolioDetailsService.GetHoldings(id));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Error fetching holdings.", detail = ex.Message });
//            }
//        }

//        [HttpGet("{id}/transactions")]
//        public async Task<IActionResult> GetTransactions(int id)
//        {
//            try
//            {
//                return Ok(await _portfolioDetailsService.GetTransactions(id));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Error fetching transactions.", detail = ex.Message });
//            }
//        }

//        [HttpGet("{id}/returns")]
//        public async Task<IActionResult> GetReturns(int id)
//        {
//            try
//            {
//                return Ok(await _portfolioDetailsService.GetReturns(id));
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, new { message = "Error fetching returns.", detail = ex.Message });
//            }
//        }


//    }
//}
