using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class FinancialIndicatorsRepository : IFinancialIndicatorsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public FinancialIndicatorsRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<FinansiskiPokazateli>> GetAllAsync()
        {
            return await _dbContext.FinansiskiPokazateli.ToListAsync();
        }
    }
}
