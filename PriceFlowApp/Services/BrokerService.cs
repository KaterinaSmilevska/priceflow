using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class BrokerService : IBrokerService
    {
        private readonly IBrokersRepository _brokerRepository;

        public BrokerService(IBrokersRepository brokerRepository)
        {
            _brokerRepository = brokerRepository;
        }

        public Task<Brokeri?> FindById(int id)
        {
            return _brokerRepository.GetByIdAsync(id);
        }

        public async Task<Brokeri?> FindByCompanyAsync(string company)
        {
            if (string.IsNullOrWhiteSpace(company))
                throw new ArgumentException("Kompanija cannot be null or empty.");

            var broker = await _brokerRepository.GetByCompanyAsync(company);
            if (broker == null)
                throw new ArgumentException($"Broker with Kompanija '{company}' not found.");

            return broker;
        }

        public async Task<IEnumerable<Broker>> FindAllAsync()
        {
            IEnumerable<Brokeri> brokers = await _brokerRepository.GetAllAsync();

            return brokers.Select(b => new Broker
            {
                Id = b.Id,
                Company = b.Kompanija,
                CommissionPercent = b.ProcentProvizija
            });
        }
    }
}
