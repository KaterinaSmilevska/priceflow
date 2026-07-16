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

        public async Task<IzvestuvanjaPortfolija> AddAsync(IzvestuvanjaPortfolija portfolioNotification)
        {
            await _dbContext.IzvestuvanjaPortfolija.AddAsync(portfolioNotification);
            await _dbContext.SaveChangesAsync();

            return portfolioNotification;
        }

        public async Task<IzvestuvanjaPortfolija?> GetByPortfolioId(int portfolioId)
        {
            return await _dbContext.IzvestuvanjaPortfolija
                .Where(ip => ip.PortfolioId == portfolioId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<IzvestuvanjaPortfolija>> GetByUserIdAsync(int userId)
        {
            return await _dbContext.IzvestuvanjaPortfolija
                .Include(ip => ip.Portfolio)
                .Where(ip => ip.Portfolio.KorisnikId == userId)
                .ToListAsync();
        }

        public async Task<IzvestuvanjaPortfolija> UpdateAsync(IzvestuvanjaPortfolija portfolioNotification)
        {
            _dbContext.IzvestuvanjaPortfolija.Update(portfolioNotification);
            await _dbContext.SaveChangesAsync();

            return portfolioNotification;
        }
    }
}
