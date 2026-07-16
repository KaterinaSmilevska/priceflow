using DataAccess.Models;

namespace DataAccess.Repositories
{
    public interface IFinancialIndicatorsRepository
    {
        Task<IEnumerable<FinansiskiPokazateli>> GetAllAsync();
    }
}
