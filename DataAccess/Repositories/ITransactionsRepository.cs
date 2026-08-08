using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface ITransactionsRepository
    {
        Transakcii? GetById(int id);

        IEnumerable<Transakcii> GetByPortfolioId(int portfolioId);

        IEnumerable<Transakcii> GetByPortfolioIdUntilDate(int portfolioId, DateOnly date);

        List<int> GetOwnedSecuritiesIds(int userId);

        int GetOwnedShares(int portfolioId, int securityId, bool isReal);

        int GetOwnedSharesAtDate(int portfolioId, int securityId, bool isReal, DateOnly date, int? transactionToExclude);

        int GetOwnedSharesByUser(int userId, int securityId);

        Transakcii Add(Transakcii transaction);

        Transakcii Update(Transakcii transaction);

        Transakcii Delete(Transakcii transaction);
    }
}
