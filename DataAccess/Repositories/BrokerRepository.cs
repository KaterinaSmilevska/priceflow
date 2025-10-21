using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class BrokerRepository : IBrokerRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public BrokerRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task<Broker> GetBrokerByKompanijaAsync(string kompanija)
        {
            return await _dbContext.Brokers
                .FirstOrDefaultAsync(b => b.Kompanija == kompanija);
        }
    }
}
