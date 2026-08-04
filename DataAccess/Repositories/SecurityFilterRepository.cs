using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories
{
    public class SecurityFilterRepository : ISecurityFilterRepository
    {
        private readonly PriceFlowDbContext _dbContext;

        public SecurityFilterRepository(PriceFlowDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<HartiiOdVrednost?> GetMostProfitableSecuritiesByDividendYield()
        {
            int latestYear = this.GetLatestYear();

            return _dbContext.HartiiOdVrednost
                 .Include(hv => hv.Izdavach)
                .ThenInclude(i => i.FinansiskiPokazateli)
                .Where(h => h.Izdavach.FinansiskiPokazateli
                    .Any(fp => fp.Godina == latestYear && fp.DividendenPrinos != null && fp.DividendenPrinos != 0))
                .OrderByDescending(h => h.Izdavach.FinansiskiPokazateli
                        .Where(fp => fp.Godina == latestYear && fp.DividendenPrinos != null && fp.DividendenPrinos != 0)
                        .Select(fp => fp.DividendenPrinos)
                        .FirstOrDefault())
                    .ToList();
        }

        public IEnumerable<HartiiOdVrednost?> GetMostProfitableSecuritiesByDividendPerShare()
        {
            int latestYear = this.GetLatestYear();

            return _dbContext.HartiiOdVrednost
                .Include(hv => hv.Izdavach)
                .ThenInclude(i => i.FinansiskiPokazateli)
                .Where(h => h.Izdavach.FinansiskiPokazateli
                    .Any(fp => fp.Godina == latestYear && fp.DividendaPoAkcija != null && fp.DividendaPoAkcija != 0))
                .OrderByDescending(h => h.Izdavach.FinansiskiPokazateli
                        .Where(fp => fp.Godina == latestYear && fp.DividendaPoAkcija != null && fp.DividendaPoAkcija != 0)
                        .Select(fp => fp.DividendaPoAkcija)
                        .FirstOrDefault())
                    .ToList();
        }

        public IEnumerable<HartiiOdVrednost?> GetSecuritiesWithBiggestPriceOscillations()
        {
            DateTime latestDate = this.GetLatestDate();

            return _dbContext.HartiiOdVrednost
                .Include(hv => hv.DnevenPromet)
                .Where(hv => hv.DnevenPromet
                    .Any(dp => dp.Datum == latestDate && dp.MaxCena != null && dp.MinCena != null))
                .OrderByDescending(hv => hv.DnevenPromet
                    .Where(dp => dp.Datum == latestDate && dp.MaxCena != null && dp.MinCena != null
                        && (dp.MaxCena - dp.MinCena) > 0)
                    .Select(dp => (dp.MaxCena ?? 0) - (dp.MinCena ?? 0))
                    .FirstOrDefault())
                .ToList();
        }

        public IEnumerable<HartiiOdVrednost?> GetSecuritiesWithSmallestPriceOscillations()
        {
            DateTime latestDate = this.GetLatestDate();

            return _dbContext.HartiiOdVrednost
                .Include(hv => hv.DnevenPromet)
                .Where(hv => hv.DnevenPromet
                    .Any(dp => dp.Datum == latestDate && dp.MaxCena != null && dp.MinCena != null))
                .OrderBy(hv => hv.DnevenPromet
                    .Where(dp => dp.Datum == latestDate && dp.MaxCena != null && dp.MinCena != null 
                        && (dp.MaxCena - dp.MinCena) > 0)
                    .Select(dp => (dp.MaxCena ?? 0) - (dp.MinCena ?? 0))
                    .FirstOrDefault())
                .ToList();
        }

        public IEnumerable<HartiiOdVrednost?> GetLeastLiquidSecuritiesByTradedQuantity()
        {
            DateTime latestDate = this.GetLatestDate();

            return _dbContext.HartiiOdVrednost
                .Include(hv => hv.DnevenPromet)
                .Where(hv => hv.DnevenPromet
                    .Any(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii > 0))
                .OrderBy(hv => hv.DnevenPromet
                    .Where(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii > 0)
                    .Select(dp => dp.KolicinaIstrguvaniAkcii)
                    .FirstOrDefault())
                .ToList();
        }

        public IEnumerable<HartiiOdVrednost?> GetMostLiquidSecuritiesByTradedQuantity()
        {
            {
                DateTime latestDate = this.GetLatestDate();

                return _dbContext.HartiiOdVrednost
                    .Include(hv => hv.DnevenPromet)
                    .Where(hv => hv.DnevenPromet
                        .Any(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii > 0))
                    .OrderByDescending(hv => hv.DnevenPromet
                        .Where(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii > 0)
                        .Select(dp => dp.KolicinaIstrguvaniAkcii)
                        .FirstOrDefault())
                    .ToList();
            }
        }

        public IEnumerable<HartiiOdVrednost?> GetLeastLiquidSecuritiesByNumTradingDays()
        {
            return _dbContext.HartiiOdVrednost
                .Include(hv => hv.DnevenPromet)
                .Where(hv => hv.DnevenPromet
                    .Any(dp => dp.KolicinaIstrguvaniAkcii > 0))
                .OrderBy(hv => hv.DnevenPromet
                    .Count(dp => dp.KolicinaIstrguvaniAkcii > 0))
                .ToList();
        }

        public IEnumerable<HartiiOdVrednost?> GetMostLiquidSecuritiesByNumTradingDays()
        {
            return _dbContext.HartiiOdVrednost
                .Include(hv => hv.DnevenPromet)
                .Where(hv => hv.DnevenPromet
                    .Any(dp => dp.KolicinaIstrguvaniAkcii > 0))
                .OrderByDescending(hv => hv.DnevenPromet
                    .Count(dp => dp.KolicinaIstrguvaniAkcii > 0))
                .ToList();
        }

        public IEnumerable<Sektori?> GetMostProfitableSectorsByDividendYield()
        {
            int latestYear = this.GetLatestYear();

            return _dbContext.Sektori
                .Include(s => s.Izdavachi)
                .ThenInclude(i => i.FinansiskiPokazateli)
                .Where(s => s.Izdavachi
                    .Any(i => i.FinansiskiPokazateli
                        .Any(fp => fp.Godina == latestYear && fp.DividendenPrinos != null)))
                .OrderByDescending(s => s.Izdavachi
                    .SelectMany(i => i.FinansiskiPokazateli)
                    .Where(fp => fp.Godina == latestYear && fp.DividendenPrinos != null)
                    .Sum(fp => fp.DividendenPrinos ?? 0))
                .ToList();
        }

        public IEnumerable<Sektori?> GetMostProfitableSectorsByProfit()
        {
            int latestYear = this.GetLatestYear();

            return _dbContext.Sektori
                .Include(s => s.Izdavachi)
                .ThenInclude(i => i.FinansiskiPokazateli)
                .Where(s => s.Izdavachi
                    .Any(i => i.FinansiskiPokazateli
                        .Any(fp => fp.Godina == latestYear && fp.OperativnaDobivka != null)))
                .OrderByDescending(s => s.Izdavachi
                    .SelectMany(i => i.FinansiskiPokazateli)
                    .Where(fp => fp.Godina == latestYear && fp.OperativnaDobivka != null && fp.OperativnaDobivka > 0)
                    .Sum(fp => fp.OperativnaDobivka ?? 0))
                .ToList();
        }

        public IEnumerable<HartiiOdVrednost?> GetSecuritiesValuation()
        {
            int latestYear = this.GetLatestYear();

            DateTime latestDate = this.GetLatestDate();

            return _dbContext.HartiiOdVrednost
                .Include(hv => hv.DnevenPromet)
                .Include(hv => hv.Izdavach)
                .ThenInclude(hv => hv.FinansiskiPokazateli)
                .Where(hv => hv.DnevenPromet
                    .Any(dp => dp.Datum == latestDate && dp.CenaPoslednaTransakcija != null) &&
                    hv.Izdavach.FinansiskiPokazateli
                    .Any(fp => fp.Godina == latestYear && fp.KnigovodstvenaVrednostPoAkcija != null
                        && fp.KnigovodstvenaVrednostPoAkcija > 0))
                .OrderByDescending(hv =>
                (
                    hv.DnevenPromet
                    .Where(dp => dp.Datum == latestDate && dp.CenaPoslednaTransakcija != null)
                    .Select(dp => dp.CenaPoslednaTransakcija ?? 0)
                    .FirstOrDefault()

                    /

                    hv.Izdavach.FinansiskiPokazateli
                    .Where(fp => fp.Godina == latestYear && fp.KnigovodstvenaVrednostPoAkcija != null)
                    .Select(fp => fp.KnigovodstvenaVrednostPoAkcija ?? 1)
                    .FirstOrDefault()
                ))
                .ToList();
        }

        public DateTime GetLatestDate()
        {
            return _dbContext.DnevenPromet
                .Max(dp => dp.Datum);
        }

        public int GetLatestYear()
        {
            return _dbContext.FinansiskiPokazateli
                .Max(fp => fp.Godina);
        }
    }
}
