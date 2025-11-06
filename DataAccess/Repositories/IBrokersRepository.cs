using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IBrokersRepository
    {
        Task<Brokeri?> GetByIdAsync(int id);

        Task<Brokeri?> GetByCompanyAsync(string company);
    }
}
