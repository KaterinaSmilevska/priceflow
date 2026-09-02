using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class BrokersRepository : IBrokersRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public BrokersRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Brokeri? GetById(int id)
        {
            return _dbContext.Brokeri
                .Find(id);
        }

        public Brokeri? GetByCompany(string company)
        {
            return _dbContext.Brokeri
                .FirstOrDefault(b => b.Kompanija == company);
        }

        public IEnumerable<Brokeri> GetAll()
        {
            return _dbContext.Brokeri
                .ToList();
        }

        public Brokeri Add(Brokeri broker)
        {
            _dbContext.Brokeri.Add(broker);
            _dbContext.SaveChanges();

            return broker;
        }

        public Brokeri Update(Brokeri broker)
        {
            _dbContext.Brokeri.Update(broker);
            _dbContext.SaveChanges();

            return broker;
        }

        public Brokeri Delete(Brokeri broker)
        {
            _dbContext.Brokeri.Remove(broker);
            _dbContext.SaveChanges();

            return broker;
        }
    }
}
