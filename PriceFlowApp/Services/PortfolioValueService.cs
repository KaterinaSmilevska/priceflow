using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class PortfolioValueService: IPortfolioValueService
    {
        private readonly PriceFlowDbContext _dbContext;

        public PortfolioValueService(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<PortfolioValue> GetCurrentValue(int portfolioId, bool isReal)
        {
            IEnumerable<PortfolioValue> result = _dbContext.Transakcii
                .Where(t => t.PortfolioId == portfolioId && t.Realna == isReal)
                .GroupBy(t => new
                {
                    t.Hvid,
                    t.Hv.Kod,
                })
                .Select(g => new
                {
                    g.Key.Hvid,
                    g.Key.Kod,
                    Quantity =
                        g.Sum(t =>
                            t.TipTransakcija == "Купување" ? t.KolicinaAkcii :
                            t.TipTransakcija == "Продавање" ? -t.KolicinaAkcii :
                            0
                        )
                })
                .Where(g => g.Quantity > 0)
                .Join(
                    _dbContext.DnevenPromet
                    .GroupBy(dp => dp.Hvid)
                    .Select(g => new
                    {
                        HvId = g.Key,
                        LastPrice = g.OrderByDescending(x => x.Datum)
                                .Select(x => x.CenaPoslednaTransakcija)
                                .FirstOrDefault()
                    })
                    .Where(x => x.LastPrice != null),
                    t => t.Hvid,
                    p => p.HvId,
                    (t, p) => new PortfolioValue
                    {
                        HvId = t.Hvid,
                        HvCode = t.Kod,
                        TotalQuantity = t.Quantity,
                        LastPrice = p.LastPrice.Value,
                        CurrentValue = t.Quantity * p.LastPrice.Value,
                        IsReal = isReal
                    }
                )
                .OrderByDescending(x => x.CurrentValue)
                .ToList();

            return result;
        }
    }
}
