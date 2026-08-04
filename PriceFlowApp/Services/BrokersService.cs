using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;

namespace PriceFlowApp.Services
{
    public class BrokersService : IBrokersService
    {
        private readonly IBrokersRepository _brokersRepository;

        public BrokersService(IBrokersRepository brokersRepository)
        {
            _brokersRepository = brokersRepository;
        }

        public Brokeri? FindById(int id)
        {
            return _brokersRepository
                .GetById(id);
        }

        public Brokeri? FindByCompany(string company)
        {
            if (string.IsNullOrWhiteSpace(company))
                throw new ValidationException("BROKER_COMPANY_REQUIRED", "Company cannot be null or empty.");

            var broker = _brokersRepository.GetByCompany(company);

            if (broker == null)
                throw new NotFoundException("BROKER_NOT_FOUND", $"Broker with company '{company}' not found.");

            return broker;
        }

        public IEnumerable<Broker> FindAll()
        {
            IEnumerable<Brokeri> brokers = _brokersRepository.GetAll();

            return brokers.Select(b => new Broker
            {
                Id = b.Id,
                Company = b.Kompanija,
                CommissionPercent = b.ProcentProvizija
            });
        }

        public IEnumerable<BrokerResponse> GetAll()
        {
            IEnumerable<Brokeri> brokers = _brokersRepository.GetAll();

            return brokers.Select(b => new BrokerResponse
            {
                Id = b.Id,
                Company = b.Kompanija,
                CommissionPercent = b.ProcentProvizija
            });
        }

        public Broker Add(CreateBrokerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Company))
                throw new ValidationException("BROKER_COMPANY_REQUIRED", "Company cannot be null or empty.");

            if (request.CommissionPercent < 0)
                throw new BusinessRuleException("BROKER_IVALID_COMMISSION", "Commission percent cannot be negative.");

            if (_brokersRepository.GetByCompany(request.Company) != null)
                throw new AlreadyExistsException("BROKER_ALREADY_EXISTS", "Broker already exists.");

            var entity = new Brokeri
            {
                Kompanija = request.Company,
                ProcentProvizija = request.CommissionPercent
            };

            var createdBroker = _brokersRepository.Add(entity);

            return new Broker
            {
                Id = createdBroker.Id,
                Company = createdBroker.Kompanija,
                CommissionPercent = createdBroker.ProcentProvizija
            };
        }

        public BrokerResponse Update(UpdateBrokerRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Company))
                throw new ValidationException("BROKER_COMPANY_REQUIRED", "Company cannot be null or empty.");

            if (request.CommissionPercent < 0)
                throw new BusinessRuleException("BROKER_IVALID_COMMISSION", "Commission percent cannot be negative.");

            if (_brokersRepository.GetByCompany(request.Company) != null)
                throw new AlreadyExistsException("BROKER_ALREADY_EXISTS", "Broker already exists.");

            Brokeri? broker = _brokersRepository.GetById(request.Id);

            if (broker == null)
                throw new NotFoundException("BROKER_NOT_FOUND", "Broker not found.");

            broker.Kompanija = request.Company;
            broker.ProcentProvizija = request.CommissionPercent;

            _brokersRepository.Update(broker);

            return new BrokerResponse
            {
                Id = broker.Id,
                Company = broker.Kompanija,
                CommissionPercent = broker.ProcentProvizija
            };
        }

        public Broker Delete(int id)
        {
            Brokeri? existingBroker = _brokersRepository.GetById(id);

            if (existingBroker == null)
                throw new NotFoundException("BROKER_NOT_FOUND", "Broker not found.");

            _brokersRepository.Delete(existingBroker);

            return new Broker
            {
                Id = existingBroker.Id,
                Company = existingBroker.Kompanija,
                CommissionPercent = existingBroker.ProcentProvizija
            };
        }
    }
}
