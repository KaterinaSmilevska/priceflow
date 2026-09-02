using DataAccess.Models;

namespace DataAccess.Repositories
{
    public class UsersRolesRepository : IUsersRolesRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public UsersRolesRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        } 

        public KorisniciUlogi Add(KorisniciUlogi userRole)
        {
            _dbContext.KorisniciUlogi.Add(userRole);
             _dbContext.SaveChanges();

            return userRole;
        }

        public int RemoveByUserId(int id)
        {
            _dbContext.KorisniciUlogi.RemoveRange(_dbContext.KorisniciUlogi
               .Where(ku => ku.KorisnikId == id));
             _dbContext.SaveChanges();

            return id;
        }
    }
}
