using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class IssuersRepository : IIssuersRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public IssuersRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Izdavachi? GetById(int id)
        {
            return _dbContext.Izdavachi
                .Find(id);
        }

        public IEnumerable<Izdavachi> GetAll()
        {
            return _dbContext.Izdavachi
                .ToList();
        }
    }
}
