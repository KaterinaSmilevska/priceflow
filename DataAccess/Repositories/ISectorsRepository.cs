using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface ISectorsRepository
    {
        Task<Sektori?> GetByIdAsync(int id);

        Task<IEnumerable<Sektori>> GetAllAsync();
    }
}
