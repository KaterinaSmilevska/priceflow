using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PortfolioReturnsRepository : IPortfolioReturnsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public PortfolioReturnsRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<PortfolioPrinosi> AddAsync(PortfolioPrinosi portfolioPrinosi)
        {
            _dbContext.PortfolioPrinosi.Add(portfolioPrinosi);
            await _dbContext.SaveChangesAsync();
            return portfolioPrinosi;
        }

        public async Task<IEnumerable<PortfolioPrinosi>> GetByPortfolioIdAsync(int portfolioId)
        {
            return await _dbContext.PortfolioPrinosi
                .Where(pp => pp.PortfolioId == portfolioId)
                .ToListAsync();
        }
    }
}
