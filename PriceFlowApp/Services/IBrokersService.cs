using DataAccess.Models;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public interface IBrokersService
    {
        Task<IEnumerable<Broker>> FindAllAsync();

        Task<IEnumerable<BrokerResponse>> GetAllAsync();

        Task<Brokeri?> FindById(int id);

        Task<Brokeri?> FindByCompanyAsync(string company);

        Task<Broker> AddAsync(CreateBrokerRequest request);

        Task<BrokerResponse> UpdateAsync(UpdateBrokerRequest request);

        Task DeleteAsync(int brokerId);
    }
}
