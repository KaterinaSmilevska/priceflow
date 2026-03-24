using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class AuthRepository: IAuthRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public AuthRepository(PriceFlowDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Korisnici?> GetByIdAsync(int id)
        {
            return await _dbContext.Korisnici
                .FindAsync(id);
        }

        public async Task<Korisnici?> GetByUsernameAsync(string username)
        {
            return await _dbContext.Korisnici
                .Include(k => k.KorisniciUlogi)
                .ThenInclude(ku => ku.Uloga)
                .FirstOrDefaultAsync(k => k.Username == username);
        }

        public async Task<IEnumerable<Korisnici>> GetAllAsync()
        {
            return await _dbContext.Korisnici
                .Include(k => k.KorisniciUlogi)
                .ThenInclude(ku => ku.Uloga)
                .ToListAsync();
        }

        public async Task AddAsync(Korisnici user)
        {
            _dbContext.Korisnici.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Korisnici user)
        {
            _dbContext.Korisnici.Update(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Korisnici user)
        {
            //if (user == null) return;

            //List<Portfolija> portfolios = await _dbContext.Portfolija
            //    .Where(p => p.KorisnikId == user.Id)
            //    .Include(p => p.Transakcii)
            //    .Include(p => p.PortfolioPrinosi)
            //    .ToListAsync();

            //foreach(Portfolija portfolio in portfolios)
            //{
            //    _dbContext.Transakcii.RemoveRange(portfolio.Transakcii);
            //    _dbContext.PortfolioPrinosi.RemoveRange(portfolio.PortfolioPrinosi);
            //}

            //_dbContext.Portfolija.RemoveRange(portfolios);

            //await _usersRolesRepository.RemoveByUserIdAsync(user.Id);

            _dbContext.Korisnici.Remove(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbContext.Korisnici
                .AnyAsync(k => k.Username == username);
        }

        public async Task<Korisnici?> GetByVerificationTokenAsync(Guid token)
        {
            return await _dbContext.Korisnici
                .FirstOrDefaultAsync(k => k.EmailVerificationToken == token);
        }

        public async Task<Korisnici?> GetByEmailAsync(string email)
        {
            return await _dbContext.Korisnici
                .FirstOrDefaultAsync(k => k.Email == email);
        }

        public async Task<Korisnici?> GetByResetPasswordTokenAsync(Guid token)
        {
            return await _dbContext.Korisnici
                .FirstOrDefaultAsync(k => k.ResetPasswordToken == token);
        }
    }
}
