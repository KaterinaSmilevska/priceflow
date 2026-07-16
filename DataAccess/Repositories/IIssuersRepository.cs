using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IIssuersRepository
    {
        Task<Izdavachi?> GetByIdAsync(int id);

        Task<IEnumerable<Izdavachi>> GetAllAsync();
    }
}
