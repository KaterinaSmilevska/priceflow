using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class TransactionsRepository : ITransactionsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public TransactionsRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<Transakcii> AddAsync(Transakcii transaction)
        {
            await _dbContext.AddAsync(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        }

        public async Task DeleteAsync(Transakcii transaction)
        {
            _dbContext.Transakcii.Remove(transaction);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Transakcii?> GetByIdAsync(int id)
        {
            return await _dbContext.Transakcii.FindAsync(id);
        }

        public async Task<List<Transakcii>> GetByPortfolioIdAsync(int portfolioId)
        {
            return await _dbContext.Transakcii
                .Include(t => t.Hv)
                .Where(t => t.PortfolioId == portfolioId)
                .OrderByDescending(t => t.Datum)
                .ToListAsync();
        }

        public async Task<Transakcii> UpdateAsync(Transakcii transaction)
        {
            _dbContext.Transakcii.Update(transaction);
            await _dbContext.SaveChangesAsync();
            return transaction;
        }
    }
}
