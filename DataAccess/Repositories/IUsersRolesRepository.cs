using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IUsersRolesRepository
    {
        Task AddAsync(KorisniciUlogi userRole);

        Task RemoveByUserIdAsync(int id);
    }
}
