using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class PortfoliosRepository : IPortfoliosRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public PortfoliosRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Portfolija? GetById(int id)
        {
            return _dbContext.Portfolija
                .Find(id);
        }

        public IEnumerable<Portfolija> GetByUserId(int userId)
        {
            return _dbContext.Portfolija
                 .Where(p => p.KorisnikId == userId)
                 .Include(p => p.Transakcii)
                 .Include(p => p.PortfolioPrinosi)
                 .ToList();
        }

        public Portfolija Add(Portfolija portfolio)
        {
            _dbContext.Portfolija.Add(portfolio);
            _dbContext.SaveChanges();

            return portfolio;
        }

        public Portfolija Update(Portfolija portfolio)
        {
            //var foundPortfolio = await _dbContext.Portfolija
            //    .FirstOrDefaultAsync(p => p.Id == portfolio.Id && p.KorisnikId == portfolio.KorisnikId);

            //if (foundPortfolio == null)
            //    return null;

            //foundPortfolio.Ime = portfolio.Ime;
            //foundPortfolio.Opis = portfolio.Opis;

            _dbContext.Portfolija.Update(portfolio);
            _dbContext.SaveChanges();

            return portfolio;
        }

        public Portfolija Delete(Portfolija portfolio)
        {
            _dbContext.Portfolija.Remove(portfolio);
            _dbContext.SaveChanges();

            return portfolio;
        }
    }
}
