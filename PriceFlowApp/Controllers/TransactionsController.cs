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
        public ActionResult<IEnumerable<Transaction>> GetAll(int portfolioId)
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
        public ActionResult<Transaction> Add(int portfolioId, [FromBody] Transaction transaction)
        {
            try
            {
                Transaction addedTransaction = _transactionsService.Add(portfolioId, transaction);

                return Ok(addedTransaction);
            }
            catch(InvalidOperationException ex)
            {
                return BadRequest(new {message = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error adding deletedTransaction to portfolio.", detail = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public ActionResult<Transaction> Update(int portfolioId, int id, [FromBody] Transaction transaction)
        {
            try
            {
                Transaction updatedTransaction = _transactionsService.Update(portfolioId, id, transaction);

                return Ok(updatedTransaction);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating deletedTransaction in portfolio.", detail = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<Transaction> Delete(int id)
        {
            try
            {
                Transaction deletedTransaction = _transactionsService.Delete(id);

                return Ok(deletedTransaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting deletedTransaction from portfolio.", detail = ex.Message });
            }
        }

        [HttpGet("analytics")]
        public ActionResult<PortfolioAnalytics> GetAnalytics(int portfolioId, [FromQuery] bool isReal)
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
                IEnumerable<PortfolioValue> value = _portfolioValueService.GetCurrentValue(portfolioId, isReal);

                return Ok(value);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching portfolio value.", detail = ex.Message });
            }
        }
    }
}
