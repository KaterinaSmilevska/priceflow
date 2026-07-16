using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IPortfoliosNotificationsRepository
    {
        Task<IEnumerable<IzvestuvanjaPortfolija>> GetByUserIdAsync(int userId);

        Task<IzvestuvanjaPortfolija?> GetByPortfolioId(int portfolioId);

        Task<IzvestuvanjaPortfolija> UpdateAsync(IzvestuvanjaPortfolija portfolioNotification);

        Task<IzvestuvanjaPortfolija> AddAsync(IzvestuvanjaPortfolija portfolioNotification);
    }
}
