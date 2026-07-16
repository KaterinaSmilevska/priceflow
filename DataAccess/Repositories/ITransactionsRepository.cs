using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface ITransactionsRepository
    {
        Task<Transakcii?> GetByIdAsync(int id);

        Task<List<Transakcii>> GetByPortfolioIdAsync(int portfolioId);

        Task<int> GetOwnedSharesAsync(int portfolioId, int securityId, bool isReal);

        Task<Transakcii> AddAsync(Transakcii transaction);

        Task<Transakcii> UpdateAsync(Transakcii transaction);

        Task DeleteAsync(Transakcii transaction);

        Task<List<int>> GetOwnedSecuritiesIdsAsync(int userId);

        Task<int> GetOwnedSharesAtDateAsync(int portfolioId, int securityId, bool isReal, DateOnly date, int? excludeTransactionId = null);

        Task<IEnumerable<Transakcii>> GetByPortfolioUntilDateAsync(int portfolioId, DateOnly date);
    }
}
