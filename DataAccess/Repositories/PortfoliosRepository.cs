using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PortfoliosRepository : IPortfoliosRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public PortfoliosRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<Portfolija> CreateAsync(Portfolija portfolio)
        {
            _dbContext.Portfolija.Add(portfolio);
            await _dbContext.SaveChangesAsync();
            return portfolio;
        }

        public async Task DeleteAsync(int id, int userId)
        {
            Portfolija? portfolio = await _dbContext.Portfolija
                .FirstOrDefaultAsync(p => p.Id == id && p.KorisnikId == userId);

            if(portfolio != null)
            {
                _dbContext.Portfolija.Remove(portfolio);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Portfolija?> GetByIdAsync(int id, int userId)
        {
            return await _dbContext.Portfolija
                .Where(p => p.Id == id && p.KorisnikId == userId)
                .Include(p => p.Transakcii).ThenInclude(t => t.Hv)
                .Include(p => p.PortfolioPrinosi).ThenInclude(pp => pp.Hv)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Portfolija>> GetByUserAsync(int userId)
        {
           return await _dbContext.Portfolija
                .Where(p => p.KorisnikId == userId)
                .Include(p => p.Transakcii)
                .Include(p => p.PortfolioPrinosi)
                .ToListAsync();
        }

        public async Task<Portfolija?> UpdateAsync(Portfolija portfolio)
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
