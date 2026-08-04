using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class SectorsRepository : ISectorsRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public SectorsRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Sektori? GetById(int id)
        {
            return _dbContext.Sektori
                .Find(id);
        }

        public IEnumerable<Sektori> GetAll()
        {
            return _dbContext.Sektori
                .ToList();
        }
    }
}
