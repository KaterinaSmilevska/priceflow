using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IRolesRepository
    {
        Task<Ulogi?> GetByIdAsync(int id);

        Task<Ulogi?> GetByNameAsync(string name);

        Task<IEnumerable<Ulogi>> GetAllAsync();

        Task<List<string>> GetNamesAsync();

        Task<List<int>> GetIdsByNamesAsync(List<string> names);

        Task<List<string>> GetByUserIdAsync(int id);

        Task<List<string>> GetNamesAsync(IEnumerable<Ulogi> roles);
    }
}
