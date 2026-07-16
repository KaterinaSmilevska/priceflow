using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class PortfoliosRepository : IPortfoliosRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public PortfoliosRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Portfolija> CreateAsync(Portfolija portfolio)
        {
            _dbContext.Portfolija.Add(portfolio);
            await _dbContext.SaveChangesAsync();

            return portfolio;
        }

        public async Task DeleteAsync(Portfolija portfolio)
        {
            if(portfolio != null)
            {
                _dbContext.Portfolija.Remove(portfolio);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Portfolija?> GetByIdAsync(int id)
        {
            return await _dbContext.Portfolija.FindAsync(id);
        }

        public async Task<IEnumerable<Portfolija>> GetByUserAsync(int userId)
        {
           return await _dbContext.Portfolija
                .Where(p => p.KorisnikId == userId)
                .Include(p => p.Transakcii)
                .Include(p => p.PortfolioPrinosi)
                .ToListAsync();
        }

        public async Task<Portfolija> UpdateAsync(Portfolija portfolio)
        {
            var foundPortfolio = await _dbContext.Portfolija
                .FirstOrDefaultAsync(p => p.Id == portfolio.Id && p.KorisnikId == portfolio.KorisnikId);

            if (foundPortfolio == null)
                return null;

            foundPortfolio.Ime = portfolio.Ime;
            foundPortfolio.Opis = portfolio.Opis;

            _dbContext.Portfolija.Update(portfolio);
            await _dbContext.SaveChangesAsync();

            return foundPortfolio;
        }
    }
}
