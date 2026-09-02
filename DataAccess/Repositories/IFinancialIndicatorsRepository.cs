using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IFinancialIndicatorsRepository
    {
        IEnumerable<FinansiskiPokazateli> GetAll();
    }
}
