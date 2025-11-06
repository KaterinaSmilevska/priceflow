using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class IssuersRepository : IIssuersRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public IssuersRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<IEnumerable<Izdavachi>> GetAllAsync()
        {
            return await _dbContext.Izdavachi.ToListAsync();
        }

        public async Task<Izdavachi?> GetByIdAsync(int id)
        {
            return await _dbContext.Izdavachi
                .FindAsync(id);
        }
    }
}
