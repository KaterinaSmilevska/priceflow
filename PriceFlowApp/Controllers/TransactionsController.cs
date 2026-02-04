using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/portfolios/{portfolioId}/[controller]")]
    [Authorize(Roles = "Инвеститор")]
    public class TransactionsController: ControllerBase
    {
        private readonly ITransactionsService _transactionsService;
        private readonly IPortfolioValueService _portfolioValueService;

        public TransactionsController(ITransactionsService transactionsService, IPortfolioValueService portfolioValueService)
        {
            _transactionsService = transactionsService;
            _portfolioValueService = portfolioValueService;
        } 

        [HttpGet]
        public async Task<IActionResult> GetAll(int portfolioId)
        {
            try
            {
                IEnumerable<Transaction> transactions = await _transactionsService.FindByPortfolioIdAsync(portfolioId);
                return Ok(transactions);
            }
            catch (Exception ex)
             {
                    return StatusCode(500, new { message = "Error fetching transactions for portfolio.", detail = ex.Message });
             }
         }

        [HttpGet("owned-shares")]
        public async Task<ActionResult<int>> GetOwnedShares(int portfolioId, [FromQuery] string code, [FromQuery] bool isReal)
        {
            try
            {
                int ownedShares = await _transactionsService.FindOwnedSharesAsync(portfolioId, code, isReal);
                return Ok(ownedShares);
            }
            catch (Exception ex)
            {
                return StatusCode(400, new { message = "Error fetching number of owned shares.", detail = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(int portfolioId, [FromBody] Transaction transaction)
        {
            try
            {
                Transaction createdTransaction = await _transactionsService.AddAsync(portfolioId, transaction);
                return Ok(createdTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding transaction to portfolio.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int portfolioId, int id, [FromBody] Transaction transaction)
        {
            try
            {
                Transaction updatedTransaction = await _transactionsService.UpdateAsync(id, transaction);
                return Ok(updatedTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating transaction in portfolio.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int portfolioId, int id)
        {
            try
            {
                await _transactionsService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting transaction from portfolio.", detail = ex.Message });
            }
        }


        [HttpGet("analytics")]
        public async Task<IActionResult> GetAnalytics(int portfolioId, [FromQuery] bool isReal)
        {
            try
            {
                PortfolioAnalytics analytics = await _transactionsService.GetAnalyticsAsync(portfolioId, isReal);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolio analytics", detail = ex.Message });
            }
        }

        [HttpGet("value")]
        public async Task<ActionResult<List<PortfolioValue>>> GetPortfolioValue(int portfolioId, [FromQuery] bool isReal)
        {
            try
            {
                List<PortfolioValue> result = await _portfolioValueService.GetCurrentValueAsync(portfolioId, isReal);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolio value.", detail = ex.Message });
            }
        }
    }
}
