using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class PortfoliosService : IPortfoliosService
    {

        private readonly IPortfoliosRepository _portfolijaRepository;

        public PortfoliosService(IPortfoliosRepository portfolijaRepository) => _portfolijaRepository = portfolijaRepository;

        public async Task DeletePortfolio(int id, int userId)
        {
            var portfolio = await _portfolijaRepository.GetByIdAsync(id);
            if (portfolio == null || portfolio.KorisnikId != userId)
                throw new Exception("Portfolio not found");

            await _portfolijaRepository.DeleteAsync(portfolio);
        }

        public async Task<IEnumerable<Portfolio>> FindUserPortfoliosAsync(int userId)
        {
            IEnumerable<Portfolija?> items = await _portfolijaRepository.GetByUserAsync(userId);

            return items.Select(p => new Portfolio
            {
                Id = p.Id,
                Name = p.Ime,
                Description = p.Opis
            }).ToList();
        }

        public async Task<Portfolio> CreatePortfolio(int userId, CreatePortfolio portfolio)
        {
            Portfolija entity = new Portfolija
            {
                Ime = portfolio.Name,
                Opis = portfolio.Description,
                KorisnikId = userId
            };

            entity = await _portfolijaRepository.CreateAsync(entity);

            return new Portfolio
            {
                Id = entity.Id,
                Name = entity.Ime,
                Description = entity.Opis
            };
        }

        public async Task<Portfolio> UpdatePortfolio(int id, int userId, UpdatePortfolio portfolio)
        {
            var foundPortfolio = await _portfolijaRepository.GetByIdAsync(id);

            if (foundPortfolio == null || foundPortfolio.KorisnikId != userId)
                throw new Exception("Portfolio not found");

            foundPortfolio.Ime = portfolio.Name;
            foundPortfolio.Opis = portfolio.Description;

            var updated = await _portfolijaRepository.UpdateAsync(foundPortfolio);

            return new Portfolio
            {
                Id = updated.Id,
                Name = updated.Ime,
                Description = updated.Opis
            };
        }
    }
}
