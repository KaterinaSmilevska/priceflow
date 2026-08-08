using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IPortfoliosRepository
    {
        Portfolija? GetById(int id);

        Portfolija? GetByName(string name, int userId);

        IEnumerable<Portfolija> GetByUserId(int userId);

        Portfolija Add(Portfolija portfolio);

        Portfolija Update(Portfolija portfolio);

        Portfolija Delete(Portfolija portfolio);
    }
}
