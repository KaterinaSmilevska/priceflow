using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IBrokerService
    {
        Task<IEnumerable<Broker>> FindAllAsync();

        Task<Brokeri?> FindById(int id);

        Task<Brokeri?> FindByCompanyAsync(string company);
    }
}
