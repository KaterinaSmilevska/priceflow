using DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
