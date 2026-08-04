using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class RolesRepository: IRolesRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public RolesRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Ulogi? GetById(int id)
        {
            return _dbContext.Ulogi
                .Find(id);
        }

        public List<string> GetByUserId(int id)
        {
            return _dbContext.KorisniciUlogi
                .Where(ku => ku.KorisnikId == id)
                .Include(ku => ku.Uloga)
                .Select(ku => ku.Uloga.Ime)
                .ToList();
        }

        public Ulogi? GetByName(string name)
        {
            return _dbContext.Ulogi
                .FirstOrDefault(u => u.Ime == name);
        }

        public IEnumerable<Ulogi> GetAll()
        {
            return _dbContext.Ulogi
                .ToList();
        }

        public List<string> GetNames()
        {
            return _dbContext.Ulogi
                .Select(u => u.Ime)
                .ToList();
        }

        public List<int> GetIdsByNames(List<string> names)
        {
            return _dbContext.Ulogi
               .Where(u => names.Contains(u.Ime))
               .Select(u => u.Id)
               .ToList();
        }

        public List<string> GetNames(IEnumerable<Ulogi> roles)
        {
            List<int> roleIds = roles.Select(r => r.Id).ToList();

            return _dbContext.Ulogi
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Ime)
                .ToList();
        }
    }
}