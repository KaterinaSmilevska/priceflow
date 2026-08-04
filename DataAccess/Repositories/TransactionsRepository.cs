using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public TransactionsRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Transakcii? GetById(int id)
        {
            return _dbContext.Transakcii
                .Include(t => t.Hv)
                .FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Transakcii> GetByPortfolioId(int portfolioId)
        {
            return _dbContext.Transakcii
                .Include(t => t.Hv)
                .Where(t => t.PortfolioId == portfolioId)
                .OrderByDescending(t => t.Datum)
                .ToList();
        }

        public IEnumerable<Transakcii?> GetByPortfolioIdUntilDate(int portfolioId, DateOnly date)
        {
            return _dbContext.Transakcii
                .Where(t => t.PortfolioId == portfolioId && t.Realna && t.Datum <= date)
                .OrderBy(t => t.Datum)
                .ToList();
        }

        public List<int> GetOwnedSecuritiesIds(int userId)
        {
            IEnumerable<Transakcii> transactions = _dbContext.Transakcii
                .Include(t => t.Portfolio)
                .Where(t => t.Portfolio.KorisnikId == userId && t.Realna)
                .ToList();

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

        public int GetOwnedShares(int portfolioId, int securityId, bool isReal)
        {
            List<Transakcii> transactions = _dbContext.Transakcii
                .Where(t => t.PortfolioId == portfolioId && t.Hvid == securityId && t.Realna == isReal)
                .ToList();

            int bought = transactions
                .Where(t => t.TipTransakcija == "Купување")
                .Sum(t => t.KolicinaAkcii);

            int sold = transactions
               .Where(t => t.TipTransakcija == "Продавање")
               .Sum(t => t.KolicinaAkcii);

            return bought - sold;
        }

        public int GetOwnedSharesAtDate(int portfolioId, int securityId, bool isReal, DateOnly date, int? transactionIdToExclude)
        {
            var query = _dbContext.Transakcii
                .Where(t => t.PortfolioId == portfolioId && t.Hvid == securityId &&
                    t.Realna == isReal && t.Datum <= date);

            if (transactionIdToExclude.HasValue)
                query = query.Where(t => t.Id != transactionIdToExclude.Value);

            var transactions = query
                .OrderBy(t => t.Datum)
                .ToList();

            int ownedShares = 0;

            foreach (var t in transactions)
            {
                ownedShares += t.TipTransakcija == "Купување"
                    ? t.KolicinaAkcii
                    : -t.KolicinaAkcii;
            }
            return ownedShares;
        }

        public Transakcii Add(Transakcii transaction)
        {
            _dbContext.Add(transaction);
            _dbContext.SaveChanges();

            return transaction;
        }

        public Transakcii Update(Transakcii transaction)
        {
            _dbContext.Transakcii.Update(transaction);
            _dbContext.SaveChanges();

            return transaction;
        }

        public Transakcii Delete(Transakcii transaction)
        {
            _dbContext.Transakcii.Remove(transaction);
            _dbContext.SaveChanges();

            return transaction;
        }
    }
}
