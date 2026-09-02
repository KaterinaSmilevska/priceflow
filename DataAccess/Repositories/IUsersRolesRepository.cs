using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IUsersRolesRepository
    {
        KorisniciUlogi Add(KorisniciUlogi userRole);

        int RemoveByUserId(int id);
    }
}
