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
            await _portfolijaRepository.DeleteAsync(id, userId);
        }

        public async Task<PortfolioDetails?> FindPortfolioAsync(int id, int userId)
        {
            Portfolija? portfolio = await _portfolijaRepository.GetByIdAsync(id, userId);
            if (portfolio == null)
                return null;

            return new PortfolioDetails
            {
                Id = portfolio.Id,
                Name = portfolio.Ime,
                Description = portfolio.Opis,
                Transactions = portfolio.Transakcii.Select(t => new Transaction
                {
                    Id = t.Id,
                    HVCode = t.Hv.Kod,
                    SharesQuantity = t.KolicinaAkcii,
                    SharesUnitPrice = t.EdinecnaCenaAkcija,
                    Amount = t.Iznos,
                    TypeTransaction = t.TipTransakcija,
                    Date = t.Datum

                }).ToList(),

                Returns = portfolio.PortfolioPrinosi.Select(pp => new PortfolioReturn
                {
                    Date = pp.Datum,
                    NetAmount = pp.NetoIznos,
                    Tax = pp.Danok,
                    HVCode = pp.Hv.Kod,
                }).ToList()
            };
        }

        public async Task<IEnumerable<PortfolioList>> FindUserPortfoliosAsync(int userId)
        {
            IEnumerable<Portfolija?> items = await _portfolijaRepository.GetByUserAsync(userId);

            return items.Select(p => new PortfolioList
            {
                Id = p.Id,
                Name = p.Ime,
                Description = p.Opis,
                TotalTransactions = p.Transakcii.Count,
                TotalValue = p.Transakcii.Sum(t => t.Iznos)
            });
        }

        public async Task<PortfolioList?> CreatePortfolio(int userId, CreatePortfolio portfolio)
        {
            Portfolija entity = new Portfolija
            {
                Ime = portfolio.Name,
                Opis = portfolio.Description,
                KorisnikId = userId
            };

            entity = await _portfolijaRepository.CreateAsync(entity);

            return new PortfolioList
            {
                Id = entity.Id,
                Name = entity.Ime,
                Description = entity.Opis,
                TotalTransactions = 0,
                TotalValue = 0
            };
        }

        public async Task<PortfolioList?> UpdatePortfolio(int id, int userId, UpdatePortfolio portfolio)
        {
            var updated = await _portfolijaRepository.UpdateAsync(new Portfolija
            {
                Id = id,
                KorisnikId = userId,
                Ime = portfolio.Name,
                Opis = portfolio.Description
            });

            if (updated == null)
                return null;

            return new PortfolioList
            {
                Id = updated.Id,
                Name = updated.Ime,
                Description = updated.Opis,
                TotalTransactions = updated.Transakcii.Count,
                TotalValue = updated.Transakcii.Sum(t => t.Iznos)
            };
        }
    }
}
