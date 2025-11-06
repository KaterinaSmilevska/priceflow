using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class UsersRolesRepository : IUsersRolesRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public UsersRolesRepository(PriceFlowDbContext dbContext) => _dbContext = dbContext;

        public async Task AddAsync(KorisniciUlogi userRole)
        {
            _dbContext.KorisniciUlogi.Add(userRole);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveByUserIdAsync(int id)
        {
            _dbContext.KorisniciUlogi.RemoveRange(_dbContext.KorisniciUlogi
               .Where(ku => ku.KorisnikId == id));

            await _dbContext.SaveChangesAsync(); 
        }
    }
}
