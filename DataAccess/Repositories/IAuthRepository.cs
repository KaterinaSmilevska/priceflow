using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IAuthRepository
    {
        Korisnici? GetById(int id);

        Korisnici? GetByUsername(string username);

        Korisnici? GetByVerificationToken(Guid token);

        Korisnici? GetByEmail(string email);

        Korisnici? GetByResetPasswordToken(Guid token);
        
        IEnumerable<Korisnici> GetAll();
        
        Korisnici Add(Korisnici user);

        Korisnici Update(Korisnici user);

        Korisnici Delete(Korisnici user);

        bool UsernameExists(string username);        
    }
}
