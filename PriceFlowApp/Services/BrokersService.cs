using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class BrokersService : IBrokersService
    {
        private readonly IBrokersRepository _brokerRepository;

        public BrokersService(IBrokersRepository brokerRepository)
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

        public async Task<IEnumerable<BrokerResponse>> GetAllAsync()
        {
            IEnumerable<Brokeri> brokers = await _brokerRepository.GetAllAsync();

            return brokers.Select(b => new BrokerResponse
            {
                Id = b.Id,
                Company = b.Kompanija,
                CommissionPercent = b.ProcentProvizija
            });
        }

        public async Task<BrokerResponse> UpdateAsync(UpdateBrokerRequest request)
        {
            if (request.CommissionPercent < 0)
                throw new ArgumentException("Commission percent cannot be negative.");

            Brokeri? broker = await _brokerRepository.GetByIdAsync(request.Id);

            if (broker == null)
                throw new Exception("Broker cannot be found");

            broker.Kompanija = request.Company;
            broker.ProcentProvizija = request.CommissionPercent;

            await _brokerRepository.UpdateAsync(broker);

            return new BrokerResponse
            {
                Id = broker.Id,
                Company = broker.Kompanija,
                CommissionPercent = broker.ProcentProvizija
            };
        }

        public async Task DeleteAsync(int brokerId)
        {
            await _brokerRepository.DeleteAsync(brokerId);
        }

        public async Task<Broker> AddAsync(CreateBrokerRequest request)
        {
            var entity = new Brokeri
            {
                Kompanija = request.Company,
                ProcentProvizija = request.CommissionPercent
            };

            var createdBroker = await _brokerRepository.AddAsync(entity);

            var result = await _brokerRepository.GetByIdAsync(createdBroker.Id);

            return new Broker
            {
                Id = result.Id,
                Company = result.Kompanija,
                CommissionPercent = result.ProcentProvizija
            };
        }
    }
}
