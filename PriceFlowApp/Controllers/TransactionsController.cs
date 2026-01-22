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

        public TransactionsController(ITransactionsService transactionsService) => _transactionsService = transactionsService;

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
        public async Task<IActionResult> GetAnalytics(int portfolioId)
        {
            try
            {
                PortfolioAnalytics analytics = await _transactionsService.GetTotalIncomeAsync(portfolioId);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolio analytics", detail = ex.Message });
            }
        }
    }
}
