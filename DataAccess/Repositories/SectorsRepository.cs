using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class SectorsRepository : ISectorsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public SectorsRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<Sektori>> GetAllAsync()
        {
            return await _dbContext.Sektori.ToListAsync();
        }

        public async Task<Sektori?> GetByIdAsync(int id)
        {
            return await _dbContext.Sektori
                .FindAsync(id);
        }
    }
}
