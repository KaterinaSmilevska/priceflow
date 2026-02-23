using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IPortfolioReturnsRepository
    {
        Task<IEnumerable<PortfolioPrinosi>> GetByPortfolioIdAsync(int portfolioId);

        Task<PortfolioPrinosi> AddAsync(PortfolioPrinosi portfolioPrinosi);

        Task<IEnumerable<PortfolioPrinosi>> GetByPortfolioIdForPeriod(int portfolioId, DateOnly from, DateOnly to);
    }
}
