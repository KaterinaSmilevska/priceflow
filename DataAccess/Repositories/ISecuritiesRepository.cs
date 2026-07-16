using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface ISecuritiesRepository
    {
        Task<IEnumerable<HartiiOdVrednost>> GetAllAsync();

        Task<HartiiOdVrednost?> GetByIdAsync(int id);

        Task<IEnumerable<HartiiOdVrednost>> GetAllByIds(List<int> securitiesIds);

        Task<HartiiOdVrednost?> GetByCodeAsync(string code);

        Task<string?> GetSecurityCode(int id);

        Task<int?> GetTotalNumShares(int id);

        Task<int?> GetTotalNumSharesAsync(string securityCode);

        Task<HartiiOdVrednost> AddAsync(HartiiOdVrednost security);

        Task DeleteAsync(int id);

        Task UpdateAsync(HartiiOdVrednost security);

        Task<IEnumerable<HartiiOdVrednost>> SearchByCodeAsync(string searchTerm);
    }
}
