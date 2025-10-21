using DataAccess.Models;

namespace PriceFlowApp.Services
{
    public interface IBrokerService
    {
        Task<Broker> GetBrokerByKompanijaAsync(string kompanija);
    }
}
