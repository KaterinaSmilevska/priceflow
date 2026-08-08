using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Helpers;

namespace PriceFlowApp.Services
{
    public class SecuritiesService : ISecuritiesService
    {
        private readonly ISecuritiesRepository _securitiesRepository;
        private readonly IDailyTurnoverRepository _dailyTurnoverRepository;
        private readonly IIssuersRepository _issuersRepository;
        private readonly ITypeSecurityRepository _typeSecurityRepository;

        public SecuritiesService(ISecuritiesRepository securitiesRepository, IDailyTurnoverRepository dailyTurnoverRepository, 
            IIssuersRepository issuersRepository, ITypeSecurityRepository typeSecurityRepository)
        {
            _securitiesRepository = securitiesRepository;
            _dailyTurnoverRepository = dailyTurnoverRepository;
            _issuersRepository = issuersRepository;
            _typeSecurityRepository = typeSecurityRepository;
        }

        public Security FindById(int id)
        {
            HartiiOdVrednost security = GetSecurityById(id);

            return MapToSecurity(security);
        }

        public Security FindByCode(string code)
        {
            HartiiOdVrednost security = GetSecurityByCode(code);

            return MapToSecurity(security);
        }


        public IEnumerable<Security> FindAll()
        {
            IEnumerable<HartiiOdVrednost> securities = _securitiesRepository.GetAll();

            return securities
                .Select(MapToSecurity)
                .ToList();
        }

        public string? FindSecurityCode(int id)
        {
            HartiiOdVrednost security = GetSecurityById(id);

            return _securitiesRepository.GetSecurityCode(id);
        }

        public int? FindTotalNumShares(int id)
        {
            HartiiOdVrednost security = GetSecurityById(id);

            return _securitiesRepository.GetTotalNumSharesById(id);
        }

        public int? FindTotalNumShares(string securityCode)
        {
            return _securitiesRepository.GetTotalNumSharesBySecurityCode(securityCode);
        }

        public Security Add(AddSecurityRequest request)
        {
            ValidationHelper.ValidateRequiredField(request.Isin, "ISIN", "ISIN_VALIDATION_REQUIRED");
            ValidationHelper.ValidateRequiredField(request.Code, "Code", "CODE_VALIDATION_REQUIRED");
            ValidateCodeAvailability(request.Code);

            TipHv typeSecurity = GetTypeSecurityById(request.TypeSecurityId);
            Izdavachi issuer = GetIssuerById(request.IssuerId);

            HartiiOdVrednost security = new HartiiOdVrednost
            {
                Isin = request.Isin,
                Kod = request.Code,
                VkupenBrojAkcii = request.TotalNumShares,
                TipHvid = request.TypeSecurityId,
                IzdavachId = request.IssuerId
            };

            HartiiOdVrednost addedSecurity = _securitiesRepository.Add(security);

            return MapToSecurity(addedSecurity);
        }

        public Security Update(int id, UpdateSecurity security)
        {
            HartiiOdVrednost? existingSecurity = GetSecurityById(id);
            TipHv typeSecurity = GetTypeSecurityById(security.TypeSecurityId);
            Izdavachi issuer = GetIssuerById(security.IssuerId);

            ValidationHelper.ValidateRequiredField(security.Isin, "ISIN", "ISIN_VALIDATION_REQUIRED");
            ValidationHelper.ValidateRequiredField(security.Code, "Code", "CODE_VALIDATION_REQUIRED");

            ValidateCodeAvailability(security.Code, id);

            existingSecurity.Isin = security.Isin;
            existingSecurity.Kod = security.Code;
            existingSecurity.VkupenBrojAkcii = security.TotalNumShares;
            existingSecurity.TipHvid = typeSecurity.Id;
            existingSecurity.IzdavachId = issuer.Id;

            HartiiOdVrednost updatedSecurity =_securitiesRepository.Update(existingSecurity);

            return MapToSecurity(updatedSecurity);
        }

        public Security Delete(int id)
        {
            HartiiOdVrednost existingSecurity = GetSecurityById(id);

            HartiiOdVrednost deletedSecurity = _securitiesRepository.Delete(existingSecurity);

            return MapToSecurity(deletedSecurity);
        }

        public SecurityDailyPrices GetLatestPrices(string securityCode, DateTime date)
        {
           IEnumerable<DnevenPromet> dailyTurnover = _dailyTurnoverRepository.GetBySecurityCode(securityCode, date);

            if (!dailyTurnover.Any())
                return null;

            decimal? minPrice = dailyTurnover
                .Where(dailyTurnover => dailyTurnover.MinCena.HasValue)
                .Select(d => d.MinCena)
                .FirstOrDefault();

            decimal? maxPrice = dailyTurnover
                .Where(dailyTurnover => dailyTurnover.MaxCena.HasValue)
                .Select(d => d.MaxCena)
                .FirstOrDefault();

            decimal? averagePrice = dailyTurnover
                .Where(dailyTurnover => dailyTurnover.ProsecnaCena.HasValue)
                .Select(d => d.ProsecnaCena)
                .FirstOrDefault();

            return new SecurityDailyPrices
            {
                SecurityCode = securityCode,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                AveragePrice = averagePrice
            };
        }

        public IEnumerable<Security> SearchByCode(string searchTerm)
        {
            IEnumerable<HartiiOdVrednost?> securities = _securitiesRepository.SearchByCode(searchTerm);

            return securities
                .Select(MapToSecurity)
                .ToList();
        }

        private HartiiOdVrednost GetSecurityById(int securityId)
        {
            HartiiOdVrednost? security = _securitiesRepository.GetById(securityId);
            if (security == null)
                throw new NotFoundException("SECURITY_NOT_FOUND", "Security not found.");

            return security;
        }

        private HartiiOdVrednost GetSecurityByCode(string code)
        {
            HartiiOdVrednost? security = _securitiesRepository.GetByCode(code);
            if (security == null)
                throw new NotFoundException("SECURITY_NOT_FOUND", "Security not found.");

            return security;
        }

        private TipHv GetTypeSecurityById(int typeSecurityId)
        {
            var typeSecurity = _typeSecurityRepository.GetById(typeSecurityId);
            if (typeSecurity == null)
                throw new NotFoundException("TYPESECURITY_NOT_FOUND", "Type request not found.");

            return typeSecurity;
        }

        private Izdavachi GetIssuerById(int issuerId)
        {
            var issuer = _issuersRepository.GetById(issuerId);
            if (issuer == null)
                throw new NotFoundException("ISSUER_NOT_FOUND", "Issuer not found.");

            return issuer;
        }

        private void ValidateCodeAvailability(string code, int? securityId = null)
        {
            HartiiOdVrednost? existingSecurity = _securitiesRepository.GetByCode(code);
            if (existingSecurity != null && existingSecurity.Id != securityId)
                throw new AlreadyExistsException("CODE_ALREADY_EXISTS", "Code already exists.");
        }

        private Security MapToSecurity(HartiiOdVrednost security)
        {
            return new Security
            {
                Id = security.Id,
                Isin = security.Isin,
                Code = security.Kod,
                TypeSecurityName = security.TipHv.Ime,
                IssuerName = security.Izdavach.Ime,
                TotalNumShares = security.VkupenBrojAkcii
            };
        }
    }
}
