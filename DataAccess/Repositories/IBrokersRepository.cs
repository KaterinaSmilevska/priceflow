using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IBrokersRepository
    {
        Task<IEnumerable<Brokeri>> GetAllAsync();

        Task<Brokeri?> GetByIdAsync(int id);

        Task<Brokeri?> GetByCompanyAsync(string company);
    }
}
