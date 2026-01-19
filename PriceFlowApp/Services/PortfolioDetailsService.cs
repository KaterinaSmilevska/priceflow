using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class PortfolioDetailsService : IPortfolioDetailsService
    {
        private readonly PriceFlowDbContext _dbContext;

        public PortfolioDetailsService(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<PortfolioHolding>> GetHoldings(int portfolioId)
        {
            List<Transakcii> transactions = await _dbContext.Transakcii
                .Where(t => t.PortfolioId == portfolioId)
                .Include(t => t.Hv)
                .ToListAsync();

            List<PortfolioHolding> holdings = transactions
                .GroupBy(t => new { t.Hvid, t.Hv.Kod })
                .Select(g =>
                {
                    int quantity = g.Sum(t =>
                    t.TipTransakcija == "Купување" ? t.KolicinaAkcii : -t.KolicinaAkcii);

                    decimal totalCost = g
                    .Where(t => t.TipTransakcija == "Купување")
                    .Sum(t => t.KolicinaAkcii * t.EdinecnaCenaAkcija);

                    decimal avgPrice = quantity > 0 ? totalCost / quantity : 0;

                    return new PortfolioHolding
                    {
                        HVId = g.Key.Hvid,
                        HVCode = g.Key.Kod,
                        Quantity = quantity,
                        AvgPrice = avgPrice
                    };
                })
                .Where(h => h.Quantity > 0)
                .ToList();

            return holdings;
        }

        public async Task<List<PortfolioReturn>> GetReturns(int portfolioId)
        {
            return await _dbContext.PortfolioPrinosi
                .Where(p => p.PortfolioId == portfolioId)
                .Include(p => p.Hv)
                .OrderBy(p => p.Datum)
                .Select(p => new PortfolioReturn
                {
                    Date = p.Datum,
                    NetoAmount = p.NetoIznos,
                    Tax = p.Danok,
                    HVId = p.Hvid,
                    HVCode = p.Hv.Kod
                })
                .ToListAsync();
        }

        public async Task<List<Transaction>> GetTransactions(int portfolioId)
        {

            return await _dbContext.Transakcii
                .Where(t => t.PortfolioId == portfolioId)
                .Include(t => t.Hv)
                .OrderByDescending(t => t.Datum)
                .Select(t => new Transaction
                {
                    Id = t.Id,
                    HVCode = t.Hv.Kod,
                    SharesQuantity = t.KolicinaAkcii,
                    SharesUnitPrice = t.EdinecnaCenaAkcija,
                    Amount = t.Iznos,
                    TypeTransaction = t.TipTransakcija,
                    IsReal = t.Realna,
                    Date = t.Datum
                })
                .ToListAsync();
        }
    }
}
