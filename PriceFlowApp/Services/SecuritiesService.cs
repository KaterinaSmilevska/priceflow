using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;

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

        public Security? FindById(int id)
        {
            HartiiOdVrednost security = GetSecurity(id);

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

        public Security? FindByCode(string code)
        {
            var security = _securitiesRepository.GetByCode(code);

            if (security == null)
                return null;

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


        public IEnumerable<Security> FindAll()
        {
            var foundSecurities = _securitiesRepository.GetAll();

            return foundSecurities.Select(hv => new Security
            {
                Id = hv.Id,
                Isin = hv.Isin,
                Code = hv.Kod,
                TypeSecurityName = hv.TipHv.Ime,
                IssuerName = hv.Izdavach.Ime,
                TotalNumShares = hv.VkupenBrojAkcii
            });
        }

        public string? FindSecurityCode(int id)
        {
            HartiiOdVrednost security = GetSecurity(id);

            return _securitiesRepository.GetSecurityCode(id);
        }

        public int? FindTotalNumShares(int id)
        {
            HartiiOdVrednost security = GetSecurity(id);

            return _securitiesRepository.GetTotalNumSharesById(id);
        }

        public int? FindTotalNumShares(string securityCode)
        {
            return _securitiesRepository.GetTotalNumSharesBySecurityCode(securityCode);
        }

        public Security Add(CreateSecurity security)
        {
            if (string.IsNullOrWhiteSpace(security.Isin))
                throw new ValidationException("SECURITY_ISIN_REQUIRED", "ISIN cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(security.Code))
                throw new ValidationException("SECURITY_CODE_REQUIRED", "Code cannot be null or empty.");

            TipHv typeSecurity = GetTypeSecurity(security.TypeSecurityId);
            Izdavachi issuer = GetIssuer(security.IssuerId);

            var entity = new HartiiOdVrednost
            {
                Isin = security.Isin,
                Kod = security.Code,
                VkupenBrojAkcii = security.TotalNumShares,
                TipHvid = security.TypeSecurityId,
                IzdavachId = security.IssuerId
            };

            var createdSecurity = _securitiesRepository.Add(entity);

            return new Security
            {
                Id = createdSecurity.Id,
                Isin = createdSecurity.Isin,
                Code = createdSecurity.Kod,
                TypeSecurityName = createdSecurity.TipHv.Ime,
                IssuerName = createdSecurity.Izdavach.Ime,
                TotalNumShares = createdSecurity.VkupenBrojAkcii
            };
        }


        public Security Update(UpdateSecurity security)
        {
            HartiiOdVrednost? existingSecurity = GetSecurity(security.Id);
            TipHv typeSecurity = GetTypeSecurity(security.TypeSecurityId);
            Izdavachi issuer = GetIssuer(security.IssuerId);

            if (string.IsNullOrWhiteSpace(security.Isin))
                throw new ValidationException("SECURITY_ISIN_REQUIRED", "ISIN cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(security.Code))
                throw new ValidationException("SECURITY_CODE_REQUIRED", "Code cannot be null or empty.");

            existingSecurity.Isin = security.Isin;
            existingSecurity.Kod = security.Code;
            existingSecurity.VkupenBrojAkcii = security.TotalNumShares;
            existingSecurity.TipHvid = typeSecurity.Id;
            existingSecurity.IzdavachId = issuer.Id;

            HartiiOdVrednost updatedSecurity =_securitiesRepository.Update(existingSecurity);

            return new Security
            {
                Id = updatedSecurity.Id,
                Isin = updatedSecurity.Isin,
                Code = updatedSecurity.Kod,
                TypeSecurityName = updatedSecurity.TipHv.Ime,
                IssuerName = updatedSecurity.Izdavach.Ime,
                TotalNumShares = updatedSecurity.VkupenBrojAkcii
            };
        }

        public Security Delete(int id)
        {
            HartiiOdVrednost existingSecurity = GetSecurity(id);

            _securitiesRepository.Delete(existingSecurity);

            return new Security
            {
                Id = existingSecurity.Id,
                Isin = existingSecurity.Isin,
                Code = existingSecurity.Kod,
                TypeSecurityName = existingSecurity.TipHv.Ime,
                IssuerName = existingSecurity.Izdavach.Ime,
                TotalNumShares = existingSecurity.VkupenBrojAkcii
            };
        }

        public SecurityDailyPrices? GetLatestPrices(string securityCode, DateTime date)
        {
           IEnumerable<DnevenPromet?> dailyTurnover = _dailyTurnoverRepository.GetBySecurityCode(securityCode, date);

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

            return securities.Select(s => new Security
            {
                Id = s.Id,
                Isin = s.Isin,
                Code = s.Kod,
                TypeSecurityName = s.TipHv.Ime,
                IssuerName = s.Izdavach.Ime,
                TotalNumShares = s.VkupenBrojAkcii
            })
            .ToList();
        }

        private HartiiOdVrednost GetSecurity(int securityId)
        {
            var security = _securitiesRepository.GetById(securityId);
            if (security == null)
                throw new NotFoundException("SECURITY_NOT_FOUND", "Security not found.");

            return security;
        }

        private TipHv GetTypeSecurity(int typeSecurityId)
        {
            var typeSecurity = _typeSecurityRepository.GetById(typeSecurityId);
            if (typeSecurity == null)
                throw new NotFoundException("TYPESECURITY_NOT_FOUND", "Type security not found.");

            return typeSecurity;
        }

        private Izdavachi GetIssuer(int issuerId)
        {
            var issuer = _issuersRepository.GetById(issuerId);
            if (issuer == null)
                throw new NotFoundException("ISSUER_NOT_FOUND", "Issuer not found.");

            return issuer;
        }
    }
}
