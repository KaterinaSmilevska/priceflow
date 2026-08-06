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

        public Korisnici? GetById(int id)
        {
            return _dbContext.Korisnici
                .Find(id);
        }

        public Korisnici? GetByUsername(string username)
        {
            return _dbContext.Korisnici
                .Include(k => k.KorisniciUlogi)
                .ThenInclude(ku => ku.Uloga)
                .FirstOrDefault(k => k.Username == username);
        }

        public Korisnici? GetByVerificationToken(Guid token)
        {
            return _dbContext.Korisnici
                .FirstOrDefault(k => k.EmailVerificationToken == token);
        }

        public Korisnici? GetByEmail(string email)
        {
            return _dbContext.Korisnici
                .FirstOrDefault(k => k.Email == email);
        }

        public Korisnici? GetByResetPasswordToken(Guid token)
        {
            return _dbContext.Korisnici
                .FirstOrDefault(k => k.ResetPasswordToken == token);
        }

        public IEnumerable<Korisnici> GetAll()
        {
            return _dbContext.Korisnici
                .Include(k => k.KorisniciUlogi)
                .ThenInclude(ku => ku.Uloga)
                .ToList();
        }

        public Korisnici Add(Korisnici user)
        {
            _dbContext.Korisnici.Add(user);
            _dbContext.SaveChanges();

            return user;
        }

        public Korisnici Update(Korisnici user)
        {
            _dbContext.Korisnici.Update(user);
            _dbContext.SaveChanges();

            return user;
        }

        public Korisnici Delete(Korisnici user)
        {
            _dbContext.Korisnici.Remove(user);
            _dbContext.SaveChanges();

            return user;
        }

        public bool UsernameExists(string username)
        {
            return _dbContext.Korisnici
                .Any(k => k.Username == username);
        }
    }
}
