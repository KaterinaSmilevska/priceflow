using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace PriceFlowApp.Controllers
{
    [ApiController]
    [Route("api/portfolios/{portfolioId}/[controller]")]
    [Authorize(Roles = "Инвеститор")]
    public class TransactionsController: PriceFlowController
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
            return Execute(() => _transactionsService.FindByPortfolioId(portfolioId));
        }

        [HttpGet("owned-shares")]
        public ActionResult<int> GetOwnedShares(int portfolioId, [FromQuery] string securityCode, [FromQuery] bool isReal)
        {
            return Execute(() => _transactionsService.FindOwnedShares(portfolioId, securityCode, isReal));
        }


        [HttpGet("owned-shares-date")]
        public ActionResult<int> GetOwnedSharesAtDate(int portfolioId, [FromQuery] string securityCode, [FromQuery] bool isReal, [FromQuery] DateOnly date, [FromQuery] int? transactionIdToExclude)
        {
            return Execute(() => _transactionsService.FindOwnedSharesAtDate(portfolioId, securityCode, isReal, date, transactionIdToExclude));
        }

        [HttpPost]
        public ActionResult<Transaction> Add(int portfolioId, [FromBody] Transaction transaction)
        {
            return Execute(() => _transactionsService.Add(portfolioId, transaction));
        }

        [HttpPut("{id}")]
        public ActionResult<Transaction> Update(int portfolioId, int id, [FromBody] Transaction transaction)
        {
            if(id != transaction.Id)
                return BadRequest(new { message = "Transaction id mismatch." });

            return Execute(() => _transactionsService.Update(portfolioId, id, transaction));
        }

        [HttpDelete("{id}")]
        public ActionResult<Transaction> Delete(int id)
        {
            return Execute(() => _transactionsService.Delete(id));
        }

        [HttpGet("analytics")]
        public ActionResult<PortfolioAnalytics> GetAnalytics(int portfolioId, [FromQuery] bool isReal)
        {
            return Execute(() => _transactionsService.GetAnalytics(portfolioId, isReal));
        }

        [HttpGet("value")]
        public ActionResult<IEnumerable<PortfolioValue>> GetPortfolioValue(int portfolioId, [FromQuery] bool isReal)
        {
            return Execute(() => _portfolioValueService.GetCurrentValue(portfolioId, isReal));
        }
    }
}
