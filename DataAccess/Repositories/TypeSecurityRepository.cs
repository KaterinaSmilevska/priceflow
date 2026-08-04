using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class TypeSecurityRepository : ITypeSecurityRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public TypeSecurityRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public TipHv? GetById(int id)
        {
            return _dbContext.TipHv
                .Find(id);
        }

        public IEnumerable<TipHv> GetAll()
        {
            return _dbContext.TipHv
                .ToList();
        }
    }
}
