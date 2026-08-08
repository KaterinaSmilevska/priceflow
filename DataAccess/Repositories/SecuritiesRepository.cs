using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class SecuritiesRepository : ISecuritiesRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public SecuritiesRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public HartiiOdVrednost? GetById(int id)
        {
            return _dbContext.HartiiOdVrednost
                .Include(hv => hv.Izdavach)
                .Include(hv => hv.TipHv)
                .FirstOrDefault(hv => hv.Id == id);
        }

        public HartiiOdVrednost? GetByCode(string code)
        {
            return _dbContext.HartiiOdVrednost
                .FirstOrDefault(hv => hv.Kod == code);
        }

        public IEnumerable<HartiiOdVrednost> GetAllByIds(List<int> securitiesIds)
        {
            return _dbContext.HartiiOdVrednost
                .Where(s => securitiesIds.Contains(s.Id))
                .ToList();
        }

        public IEnumerable<HartiiOdVrednost> GetAll()
        {
            return _dbContext.HartiiOdVrednost
                .Include(hv => hv.Izdavach)
                .Include(hv => hv.TipHv)
                .ToList();
        }

        public string? GetSecurityCode(int id)
        {
            HartiiOdVrednost? security = _dbContext.HartiiOdVrednost
                .FirstOrDefault(hv => hv.Id == id);

            return security?.Kod;
        }

        public int? GetTotalNumSharesById(int id)
        {
            HartiiOdVrednost? security = this.GetById(id);

            return security?.VkupenBrojAkcii;
        }

        public int? GetTotalNumSharesBySecurityCode(string securityCode)
        {
            HartiiOdVrednost? security = this.GetByCode(securityCode);

            return security?.VkupenBrojAkcii;
        }

        public HartiiOdVrednost Add(HartiiOdVrednost security)
        {
            _dbContext.HartiiOdVrednost.Add(security);
            _dbContext.SaveChanges();

            return security;
        }

        public HartiiOdVrednost Update(HartiiOdVrednost security)
        {
            _dbContext.HartiiOdVrednost.Update(security);
            _dbContext.SaveChanges();

            return security;
        }

        public HartiiOdVrednost Delete(HartiiOdVrednost security)
        {
            _dbContext.HartiiOdVrednost.Remove(security);
            _dbContext.SaveChanges();

            return security;
        }

        public IEnumerable<HartiiOdVrednost?> SearchByCode(string searchTerm)
        {
            return _dbContext.HartiiOdVrednost
                .Include(hv => hv.TipHv)
                .Include(hv => hv.Izdavach)
                .Where(hv => hv.Kod.Contains(searchTerm))
                .ToList();
        }
    }
}
