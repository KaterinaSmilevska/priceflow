using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class SecuritiesService : ISecuritiesService
    {
        private readonly ISecuritiesRepository _securitiesRepository;

        public SecuritiesService(ISecuritiesRepository securitiesRepository) => _securitiesRepository = securitiesRepository;

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
    }
}
