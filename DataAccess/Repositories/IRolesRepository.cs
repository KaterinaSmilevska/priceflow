using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IRolesRepository
    {
        Ulogi? GetById(int id);

        List<string> GetByUserId(int id);

        Ulogi? GetByName(string name);

        IEnumerable<Ulogi> GetAll();

        List<string> GetNames();

        List<int> GetIdsByNames(List<string> names);

        List<string> GetNames(IEnumerable<Ulogi> roles);
    }
}
