using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IBrokersRepository
    {
        Brokeri? GetById(int id);

        Brokeri? GetByCompany(string company);

        IEnumerable<Brokeri> GetAll();

        Brokeri Add(Brokeri broker);

        Brokeri Update(Brokeri broker);

        Brokeri Delete(Brokeri broker);
    }
}
