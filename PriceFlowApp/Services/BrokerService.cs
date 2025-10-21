using DataAccess.Models;
using DataAccess.Repositories;

namespace PriceFlowApp.Services
{
    public class BrokerService : IBrokerService
    {
        private readonly IBrokerRepository _brokerRepository;

        public BrokerService(IBrokerRepository brokerRepository)
        {
            _brokerRepository = brokerRepository;
        }

        public async Task<Broker> GetBrokerByKompanijaAsync(string kompanija)
        {
            if (string.IsNullOrWhiteSpace(kompanija))
                throw new ArgumentException("Kompanija cannot be null or empty.");

            var broker = await _brokerRepository.GetBrokerByKompanijaAsync(kompanija);
            if (broker == null)
                throw new ArgumentException($"Broker with Kompanija '{kompanija}' not found.");

            return broker;
        }
    }
}
