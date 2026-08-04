using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IPortfoliosNotificationsRepository
    {
        IEnumerable<IzvestuvanjaPortfolija?> GetByUserId(int userId);

        IzvestuvanjaPortfolija? GetByPortfolioId(int portfolioId);

        IzvestuvanjaPortfolija Add(IzvestuvanjaPortfolija portfolioNotification);

        IzvestuvanjaPortfolija Update(IzvestuvanjaPortfolija portfolioNotification);
    }
}
