using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface ISecuritiesRepository
    {
        HartiiOdVrednost? GetById(int id);

        HartiiOdVrednost? GetByCode(string code);

        IEnumerable<HartiiOdVrednost> GetAllByIds(List<int> securitiesIds);

        IEnumerable<HartiiOdVrednost> GetAll();

        string? GetSecurityCode(int id);

        int? GetTotalNumSharesById(int id);

        int? GetTotalNumSharesBySecurityCode(string securityCode);

        HartiiOdVrednost Add(HartiiOdVrednost security);

        HartiiOdVrednost Update(HartiiOdVrednost security);

        HartiiOdVrednost Delete(HartiiOdVrednost security);

        IEnumerable<HartiiOdVrednost?> SearchByCode(string searchTerm);
    }
}
