using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Helpers;

namespace PriceFlowApp.Services
{
    public class BrokersService : IBrokersService
    {
        private readonly IBrokersRepository _brokersRepository;

        public BrokersService(IBrokersRepository brokersRepository)
        {
            _brokersRepository = brokersRepository;
        }

        public BrokerResponse FindById(int id)
        {
            Brokeri broker = GetBrokerById(id);

            return MapToBroker(broker);
        }

        public BrokerResponse FindByCompany(string company)
        {
            ValidationHelper.ValidateRequiredField(company, "Company", "COMPANY_VALIDATION_REQUIRED");

            Brokeri broker = GetBrokerByCompany(company);

            return MapToBroker(broker);
        }

        public IEnumerable<BrokerResponse> FindAll()
        {
            IEnumerable<Brokeri> brokers = _brokersRepository.GetAll();

            return brokers.
                Select(MapToBroker)
                .ToList();
        }

        public BrokerResponse Add(AddBrokerRequest request)
        {
            ValidationHelper.ValidateRequiredField(request.Company, "Company", "COMPANY_VALIDATION_REQUIRED");
            ValidateCompanyAvailability(request.Company);
            ValidateCommissionPercent(request.CommissionPercent);

            Brokeri broker = new Brokeri
            {
                Kompanija = request.Company,
                ProcentProvizija = request.CommissionPercent
            };

            Brokeri addedBroker = _brokersRepository.Add(broker);

            return MapToBroker(addedBroker);
        }

        public BrokerResponse Update(int id, UpdateBrokerRequest request)
        {
            ValidationHelper.ValidateRequiredField(request.Company, "Company", "COMPANY_VALIDATION_REQUIRED");
            ValidateCompanyAvailability(request.Company, id);
            ValidateCommissionPercent(request.CommissionPercent);

            Brokeri existingBroker = GetBrokerById(id);

            existingBroker.Kompanija = request.Company;
            existingBroker.ProcentProvizija = request.CommissionPercent;

            Brokeri updatedBroker = _brokersRepository.Update(existingBroker);

            return new BrokerResponse
            {
                Id = updatedBroker.Id,
                Company = updatedBroker.Kompanija,
                CommissionPercent = updatedBroker.ProcentProvizija
            };
        }

        public BrokerResponse Delete(int id)
        {
            Brokeri existingBroker = GetBrokerById(id);

            Brokeri deletedBroker = _brokersRepository.Delete(existingBroker);

            return MapToBroker(deletedBroker);
        }

        private Brokeri GetBrokerById(int brokerId)
        {
            Brokeri? broker = _brokersRepository.GetById(brokerId);
            if (broker == null)
                throw new NotFoundException("BROKER_NOT_FOUND", "Broker not found.");

            return broker;
        }

        private Brokeri GetBrokerByCompany(string company)
        {
            Brokeri? broker = _brokersRepository.GetByCompany(company);
            if(broker == null)
                throw new NotFoundException("BROKER_NOT_FOUND", $"Broker with company '{company}' not found.");

            return broker;
        }

        private void ValidateCompanyAvailability(string company, int? brokerId = null)
        {
            Brokeri? existingBroker = _brokersRepository.GetByCompany(company);
            if (existingBroker != null && existingBroker.Id != brokerId)
                throw new AlreadyExistsException("COMPANY_ALREADY_EXISTS", "Company already exists.");
        }

        private void ValidateCommissionPercent(decimal commissionPercent)
        {
            if (commissionPercent < 0)
                throw new ValidationException("INVALID_COMMISSION_PERCENT", "Commission percent cannot be negative.");
        }

        private BrokerResponse MapToBroker(Brokeri broker)
        {
            return new BrokerResponse
            {
                Id = broker.Id,
                Company = broker.Kompanija,
                CommissionPercent = broker.ProcentProvizija
            };
        }
    }
}
