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

        public async Task<Ulogi?> GetByIdAsync(int id)
        {
            return await _dbContext.Ulogi
                .FindAsync(id);
        }

        public async Task<Ulogi?> GetByNameAsync(string name)
        {
            return await _dbContext.Ulogi
                .FirstOrDefaultAsync(u => u.Ime == name);
        }

        public async Task<IEnumerable<Ulogi>> GetAllAsync()
        {
            return await _dbContext.Ulogi.ToListAsync();
        }

        public async Task<List<string>> GetNamesAsync()
        {
            return await _dbContext.Ulogi
                .Select(u => u.Ime).ToListAsync();
        }

        public async Task<List<int>> GetIdsByNamesAsync(List<string> names)
        {
            return await _dbContext.Ulogi
               .Where(u => names.Contains(u.Ime))
               .Select(u => u.Id)
               .ToListAsync();
        }

        public async Task<List<string>> GetByUserIdAsync(int id)
        {
            return await _dbContext.KorisniciUlogi
                .Where(ku => ku.KorisnikId == id)
                .Include(ku => ku.Uloga)
                .Select(ku => ku.Uloga.Ime)
                .ToListAsync();
        }

        public async Task<List<string>> GetNamesAsync(IEnumerable<Ulogi> roles)
        {
            List<int> roleIds = roles.Select(r => r.Id).ToList();

            return await _dbContext.Ulogi
                .Where(r => roleIds.Contains(r.Id))
                .Select(r => r.Ime)
                .ToListAsync();
        }
    }
}