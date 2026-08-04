using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IUsersRepository
    {
        Korisnici Update(Korisnici user);

        IEnumerable<Korisnici> GetAll();
    }
}
