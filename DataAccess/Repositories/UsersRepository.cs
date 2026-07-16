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

        public async Task<IEnumerable<Korisnici>> GetAllAsync()
        {
            return await _dbContext.Korisnici.ToListAsync();
        }

        public async Task UpdateAsync(Korisnici user)
        {
            _dbContext.Korisnici.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}
