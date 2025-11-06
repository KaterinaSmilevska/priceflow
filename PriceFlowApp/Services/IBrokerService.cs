using DataAccess.Models;

namespace PriceFlowApp.Services
{
    public interface IBrokerService
    {
        Task<Brokeri?> FindById(int id);

        Task<Brokeri?> FindByCompanyAsync(string company);
    }
}
