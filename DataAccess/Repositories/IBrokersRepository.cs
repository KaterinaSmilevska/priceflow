using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IBrokersRepository
    {
        Task<IEnumerable<Brokeri>> GetAllAsync();

        Task<Brokeri?> GetByIdAsync(int id);

        Task<Brokeri?> GetByCompanyAsync(string company);

        Task<Brokeri> AddAsync(Brokeri broker);

        Task UpdateAsync(Brokeri broker);

        Task DeleteAsync(int id);
    }
}
