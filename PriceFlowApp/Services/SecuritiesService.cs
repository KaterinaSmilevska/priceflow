using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class SecuritiesService : ISecuritiesService
    {
        private readonly ISecuritiesRepository _securitiesRepository;
        private readonly IDailyTurnoverRepository _dailyTurnoverRepository;

        public SecuritiesService(ISecuritiesRepository securitiesRepository, IDailyTurnoverRepository dailyTurnoverRepository)
        {
            _securitiesRepository = securitiesRepository;
            _dailyTurnoverRepository = dailyTurnoverRepository;

        }

        public async Task<Security> AddAsync(CreateSecurity security)
        {
            var entity = new HartiiOdVrednost
            {
                Isin = security.Isin,
                Kod = security.Code,
                VkupenBrojAkcii = security.TotalNumShares,
                TipHvid = security.TypeSecurityId,
                IzdavachId = security.IssuerId
            };

            var createdSecurity = await _securitiesRepository.AddAsync(entity);
            var full = await _securitiesRepository.GetByIdAsync(createdSecurity.Id);

            return new Security
            {
                Id = full.Id,
                Isin = full.Isin,
                Code = full.Kod,
                TypeSecurityName = full.TipHv.Ime,
                IssuerName = full.Izdavach.Ime,
                TotalNumShares = full.VkupenBrojAkcii
            };
        }

        public async Task DeleteAsync(int id)
        {
            await _securitiesRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Security>> FindAllAsync()
        {
            var foundSecurities = await _securitiesRepository.GetAllAsync();

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

        public async Task<Security?> FindByIdAsync(int id)
        {
            var security = await _securitiesRepository.GetByIdAsync(id);

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

        public async Task<Security?> FindByCodeAsync(string code)
        {
            var security = await _securitiesRepository.GetByCodeAsync(code);

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

        public async Task<Security> UpdateAsync(int id, CreateSecurity security)
        {
            HartiiOdVrednost? existingSecurity = await _securitiesRepository.GetByIdAsync(id);
            if (existingSecurity == null)
                throw new Exception("Security not found");

            existingSecurity.Isin = security.Isin;
            existingSecurity.Kod = security.Code;
            existingSecurity.VkupenBrojAkcii = security.TotalNumShares;
            existingSecurity.TipHvid = security.TypeSecurityId;
            existingSecurity.IzdavachId = security.IssuerId;

            await _securitiesRepository.UpdateAsync(existingSecurity);

            var full = await _securitiesRepository.GetByIdAsync(id);

            return new Security
            {
                Id = full.Id,
                Isin = full.Isin,
                Code = full.Kod,
                TypeSecurityName = full.TipHv.Ime,
                IssuerName = full.Izdavach.Ime,
                TotalNumShares = full.VkupenBrojAkcii
            };
        }

        public async Task<string?> FindSecurityCode(int id)
        {
            return await _securitiesRepository.GetSecurityCode(id);
        }

        public async Task<int?> FindTotalNumShares(int id)
        {
            return await _securitiesRepository.GetTotalNumShares(id);
        }

        public async Task<int?> FindTotalNumSharesAsync(string securityCode)
        {
            return await _securitiesRepository.GetTotalNumSharesAsync(securityCode);
        }

        public async Task<SecurityDailyPrices?> GetLatestPricesAsync(string securityCode, DateTime date)
        {
           IEnumerable<DnevenPromet?> dailyTurnover = await _dailyTurnoverRepository.GetBySecurityCode(securityCode, date);

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

        public async Task<IEnumerable<Security>> SearchByCodeAsync(string searchTerm)
        {
            IEnumerable<HartiiOdVrednost> securities = await _securitiesRepository.SearchByCodeAsync(searchTerm);

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
    }
}
