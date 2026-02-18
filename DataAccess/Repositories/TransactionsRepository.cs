using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public TransactionsRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<Transakcii> AddAsync(Transakcii transaction)
        {
            await _dbContext.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        }

        public async Task DeleteAsync(Transakcii transaction)
        {
            _dbContext.Transakcii.Remove(transaction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Transakcii?> GetByIdAsync(int id)
        {
            return await _dbContext.Transakcii
                .Include(t => t.Hv)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Transakcii>> GetByPortfolioIdAsync(int portfolioId)
        {
            return await _dbContext.Transakcii
                .Include(t => t.Hv)
                .Where(t => t.PortfolioId == portfolioId)
                .OrderByDescending(t => t.Datum)
                .ToListAsync();
        }

        public async Task<List<int>> GetOwnedSecuritiesIdsAsync(int userId)
        {
            IEnumerable<Transakcii> transactions = await _dbContext.Transakcii
                .Include(t => t.Portfolio)
                .Where(t => t.Portfolio.KorisnikId == userId && t.Realna)
                .ToListAsync();

            return transactions
                .GroupBy(t => t.Hvid)
                .Where(g => 
                    g.Sum(t => t.TipTransakcija == "Купување" ? t.KolicinaAkcii : 0)
                    - g.Sum(t => t.TipTransakcija == "Продавање" ? t.KolicinaAkcii : 0)
                    > 0
                    )
                .Select(g => g.Key)
                .ToList();
        }

        public async Task<int> GetOwnedSharesAsync(int portfolioId, int securityId, bool isReal)
        {
            List<Transakcii> transactions = await _dbContext.Transakcii
                .Where(t => t.PortfolioId == portfolioId && t.Hvid == securityId && t.Realna == isReal)
                .ToListAsync();

            int bought = transactions
                .Where(t => t.TipTransakcija == "Купување")
                .Sum(t => t.KolicinaAkcii);

            int sold = transactions
               .Where(t => t.TipTransakcija == "Продавање")
               .Sum(t => t.KolicinaAkcii);

            return bought - sold;
        }

        public async Task<int> GetOwnedSharesAtDateAsync(int portfolioId, int securityId, bool isReal, DateOnly date, int? excludeTransactionId = null)
        {
            var query = _dbContext.Transakcii
                .Where(t => t.PortfolioId == portfolioId && t.Hvid == securityId &&
                    t.Realna == isReal && t.Datum <= date);

            if(excludeTransactionId.HasValue)
                query = query.Where(t => t.Id != excludeTransactionId.Value);
            
            var transactions = await query
                .OrderBy(t => t.Datum)
                .ToListAsync();

            int ownedShares = 0;

            foreach(var t in transactions)
            {
                ownedShares += t.TipTransakcija == "Купување"
                    ? t.KolicinaAkcii
                    : -t.KolicinaAkcii;
            }
            return ownedShares;
        }

        public async Task<Transakcii> UpdateAsync(Transakcii transaction)
        {
            _dbContext.Transakcii.Update(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        }
    }
}
