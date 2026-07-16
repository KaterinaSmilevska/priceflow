using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IPortfoliosRepository
    {
        Task<Portfolija?> GetByIdAsync(int id);

        Task<IEnumerable<Portfolija>> GetByUserAsync(int userId);

        Task<Portfolija> CreateAsync(Portfolija portfolio);

        Task<Portfolija> UpdateAsync(Portfolija portfolio);

        Task DeleteAsync(Portfolija portfolio);
    }
}
