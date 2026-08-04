using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface ITypeSecurityRepository
    {
        TipHv? GetById(int id);

        IEnumerable<TipHv> GetAll();
    }
}
