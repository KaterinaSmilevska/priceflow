using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
