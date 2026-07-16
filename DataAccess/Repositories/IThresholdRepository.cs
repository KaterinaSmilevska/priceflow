using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IThresholdRepository
    {
        Task<IEnumerable<HvPromenaCena>> GetByUserAsync(int userId);

        Task<HvPromenaCena?> GetByIdAsync(int id);

        Task<HvPromenaCena?> GetByUserandSecurityCodeAsync(int userId, int securityId);

        Task<HvPromenaCena> AddAsync(HvPromenaCena entity);

        Task<HvPromenaCena> UpdateAsync(HvPromenaCena entity);

        Task DeleteAsync(HvPromenaCena entity);
    }
}
