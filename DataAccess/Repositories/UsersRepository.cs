using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public UsersRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Korisnici Update(Korisnici user)
        {
            _dbContext.Korisnici.Update(user);
            _dbContext.SaveChanges();

            return user;
        }

        public IEnumerable<Korisnici> GetAll()
        {
            return _dbContext.Korisnici
                .ToList();
        }
    }
}
