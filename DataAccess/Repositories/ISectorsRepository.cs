using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface ISectorsRepository
    {
        Sektori? GetById(int id);

        IEnumerable<Sektori> GetAll();
    }
}
