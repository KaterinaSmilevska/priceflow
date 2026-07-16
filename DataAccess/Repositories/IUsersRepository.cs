using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IUsersRepository
    {
        Task<IEnumerable<Korisnici>> GetAllAsync();

        Task UpdateAsync(Korisnici user);
    }
}
