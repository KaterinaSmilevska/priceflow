using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class ThresholdRepository: IThresholdRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public ThresholdRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public HvPromenaCena? GetById(int id)
        {
            return _dbContext.HvPromenaCena
                .Include(e => e.Hv)
                .FirstOrDefault(x => x.Id == id);
        }


        public IEnumerable<HvPromenaCena> GetByUserId(int userId)
        {
            return _dbContext.HvPromenaCena
                .Where(e => e.KorisnikId == userId)
                .Include(e => e.Hv)
                .ToList();
        }

        public IEnumerable<HvPromenaCena> GetBySecurityId(int securityId)
        {
            return _dbContext.HvPromenaCena
                .Include(e => e.Hv)
                .Where(e => e.Hvid == securityId)
                .ToList();
        }

        public HvPromenaCena? GetByUserIdAndSecurityId(int userId, int securityId)
        {
            return _dbContext.HvPromenaCena
                .FirstOrDefault(e => e.KorisnikId == userId && e.Hvid == securityId);
        }

        public HvPromenaCena Add(HvPromenaCena entity)
        {
           _dbContext.HvPromenaCena.Add(entity);
           _dbContext.SaveChanges();

           return entity;
        }

        public HvPromenaCena Update(HvPromenaCena entity)
        {
            _dbContext.HvPromenaCena.Update(entity);
            _dbContext.SaveChanges();

            return entity;
        }

        public HvPromenaCena Delete(HvPromenaCena entity)
        {
           _dbContext.HvPromenaCena.Remove(entity);
           _dbContext.SaveChanges();
            
            return entity;
        }
    }
}
