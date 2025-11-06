using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class BrokersRepository : IBrokersRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public BrokersRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<Brokeri?> GetByIdAsync(int id)
        {
            return await _dbContext.Brokeri
                .FindAsync(id);
        }

        public async Task<Brokeri?> GetByCompanyAsync(string company)
        {
            return await _dbContext.Brokeri
                .FirstOrDefaultAsync(b => b.Kompanija == company);
        }
    }
}
