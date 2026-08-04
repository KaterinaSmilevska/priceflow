using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class PortfoliosNotificationsRepository : IPortfoliosNotificationsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public PortfoliosNotificationsRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<IzvestuvanjaPortfolija?> GetByUserId(int userId)
        {
            return _dbContext.IzvestuvanjaPortfolija
                .Include(ip => ip.Portfolio)
                .Where(ip => ip.Portfolio.KorisnikId == userId)
                .ToList();
        }

        public IzvestuvanjaPortfolija? GetByPortfolioId(int portfolioId)
        {
            return _dbContext.IzvestuvanjaPortfolija
                .Where(ip => ip.PortfolioId == portfolioId)
                .FirstOrDefault();
        }

        public IzvestuvanjaPortfolija Add(IzvestuvanjaPortfolija portfolioNotification)
        {
            _dbContext.IzvestuvanjaPortfolija.Add(portfolioNotification);
            _dbContext.SaveChanges();

            return portfolioNotification;
        }

        public IzvestuvanjaPortfolija Update(IzvestuvanjaPortfolija portfolioNotification)
        {
            _dbContext.IzvestuvanjaPortfolija.Update(portfolioNotification);
             _dbContext.SaveChanges();

            return portfolioNotification;
        }
    }
}
