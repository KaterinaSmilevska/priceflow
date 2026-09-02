using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class PortfolioValueService: IPortfolioValueService
    {
        private readonly ITransactionsRepository _transactionsRepository;
        private readonly IDailyTurnoverRepository _dailyTurnoverRepository;

        public PortfolioValueService(ITransactionsRepository transactionsRepository, IDailyTurnoverRepository dailyTurnoverRepository)
        {
            _transactionsRepository = transactionsRepository;
            _dailyTurnoverRepository = dailyTurnoverRepository;
        }

        public IEnumerable<PortfolioValue> GetCurrentValue(int portfolioId, bool isReal)
        {
            IEnumerable<Transakcii> transactions = _transactionsRepository.GetByPortfolioId(portfolioId, isReal);
            var holdings = transactions
                .GroupBy(t => new
                {
                    t.Hvid,
                    t.Hv.Kod,
                })
                .Select(g => new
                {
                    SecurityId = g.Key.Hvid,
                    SecurityCode = g.Key.Kod,
                    Quantity =
                        g.Sum(t =>
                            t.TipTransakcija == "Купување" ? t.KolicinaAkcii :
                            t.TipTransakcija == "Продавање" ? -t.KolicinaAkcii :
                            0
                        )
                })
                .Where(g => g.Quantity > 0)
                .ToList();

            IEnumerable<int> securityIds = holdings
                .Select(x => x.SecurityId);

            IEnumerable<DnevenPromet> latestPrices = _dailyTurnoverRepository.GetLatestPrices(securityIds);
            var priceBySecurity = latestPrices
                .ToDictionary(
                    x => x.Hvid,
                    x => x.CenaPoslednaTransakcija);

            return holdings
                .Where(x => priceBySecurity.ContainsKey(x.SecurityId))
                .Select(x =>
                {
                    decimal? lastPrice = priceBySecurity[x.SecurityId];

                    return new PortfolioValue
                    {
                        SecurityId = x.SecurityId,
                        SecurityCode = x.SecurityCode,
                        TotalQuantity = x.Quantity,
                        LastPrice = lastPrice.Value,
                        CurrentValue = x.Quantity * lastPrice.Value,
                        IsReal = isReal
                    };
                })
                .OrderByDescending(x => x.CurrentValue)
                .ToList();
        }
    }
}

