using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IIssuersRepository
    {
        Izdavachi? GetById(int id);

        IEnumerable<Izdavachi> GetAll();
    }
}
