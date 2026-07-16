using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface ITypeSecurityRepository
    {
        Task<TipHv?> GetByIdAsync(int id);

        Task<IEnumerable<TipHv>> GetAllAsync();
    }
}
