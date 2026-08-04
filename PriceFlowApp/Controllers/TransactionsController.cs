using DataAccess.Enums;
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
        public IActionResult GetAll(int portfolioId)
        {
            try
            {
                IEnumerable<Transaction> transactions = _transactionsService.FindByPortfolioId(portfolioId);

                return Ok(transactions);
            }
            catch (Exception ex)
             {
                    return StatusCode(500, new { message = "Error fetching transactions for portfolio.", detail = ex.Message });
             }
         }

        [HttpGet("owned-shares")]
        public ActionResult<int> GetOwnedShares(int portfolioId, [FromQuery] string code, [FromQuery] bool isReal)
        {
            try
            {
                int ownedShares = _transactionsService.FindOwnedShares(portfolioId, code, isReal);

                return Ok(ownedShares);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching number of owned shares.", detail = ex.Message });
            }
        }


        [HttpGet("owned-shares-date")]
        public ActionResult<int> GetOwnedSharesAtDate(int portfolioId, [FromQuery] string code, [FromQuery] bool isReal, [FromQuery] DateOnly date, [FromQuery] int? transactionIdToExclude)
        {
            try
            {
                int ownedSharesAtDate = _transactionsService.FindOwnedSharesAtDate(portfolioId, code, isReal, date, transactionIdToExclude);

                return Ok(ownedSharesAtDate);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching number of owned shares at the specified date.", detail = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Create(int portfolioId, [FromBody] Transaction transaction)
        {
            try
            {
                Transaction createdTransaction = _transactionsService.Add(portfolioId, transaction);

                return Ok(createdTransaction);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding transaction to portfolio.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int portfolioId, int id, [FromBody] Transaction transaction)
        {
            try
            {
                Transaction updatedTransaction = _transactionsService.Update(id, transaction);

                return Ok(updatedTransaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating transaction in portfolio.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                Transaction transaction = _transactionsService.Delete(id);

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting transaction from portfolio.", detail = ex.Message });
            }
        }

        [HttpGet("analytics")]
        public IActionResult GetAnalytics(int portfolioId, [FromQuery] bool isReal)
        {
            try
            {
                PortfolioAnalytics analytics = _transactionsService.GetAnalytics(portfolioId, isReal);

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolio analytics", detail = ex.Message });
            }
        }

        [HttpGet("value")]
        public ActionResult<IEnumerable<PortfolioValue>> GetPortfolioValue(int portfolioId, [FromQuery] bool isReal)
        {
            try
            {
                IEnumerable<PortfolioValue> result = _portfolioValueService.GetCurrentValue(portfolioId, isReal);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolio value.", detail = ex.Message });
            }
        }
    }
}
