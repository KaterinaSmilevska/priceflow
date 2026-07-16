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

        public async Task<IEnumerable<Sektori>> GetMostProfitableSectorsByDividendYieldAsync()
        {
            int latestYear = await this.GetLatestYearAsync();

            return await _dbContext.Sektori
                .Include(s => s.Izdavachi)
                .ThenInclude(i => i.FinansiskiPokazateli)
                .Where(s => s.Izdavachi
                    .Any(i => i.FinansiskiPokazateli
                        .Any(fp => fp.Godina == latestYear && fp.DividendenPrinos != null)))
                .OrderByDescending(s => s.Izdavachi
                    .SelectMany(i => i.FinansiskiPokazateli)
                    .Where(fp => fp.Godina == latestYear && fp.DividendenPrinos != null)
                    .Sum(fp => fp.DividendenPrinos ?? 0))
                .ToListAsync();
        }

        public async Task<IEnumerable<Sektori>> GetMostProfitableSectorsByProfitAsync()
        {

            int latestYear = await this.GetLatestYearAsync();

            return await _dbContext.Sektori
                .Include(s => s.Izdavachi)
                .ThenInclude(i => i.FinansiskiPokazateli)
                .Where(s => s.Izdavachi
                    .Any(i => i.FinansiskiPokazateli
                        .Any(fp => fp.Godina == latestYear && fp.OperativnaDobivka != null)))
                .OrderByDescending(s => s.Izdavachi
                    .SelectMany(i => i.FinansiskiPokazateli)
                    .Where(fp => fp.Godina == latestYear && fp.OperativnaDobivka != null && fp.OperativnaDobivka > 0)
                    .Sum(fp => fp.OperativnaDobivka ?? 0))
                .ToListAsync();
        }

        public async Task<IEnumerable<HartiiOdVrednost>> GetMostProfitableSecuritiesByDividendPerShareAsync()
        {
            int latestYear = await this.GetLatestYearAsync();

            return await _dbContext.HartiiOdVrednost
                .Include(hv => hv.Izdavach)
                .ThenInclude(i => i.FinansiskiPokazateli)
                .Where(h => h.Izdavach.FinansiskiPokazateli
                    .Any(fp => fp.Godina == latestYear && fp.DividendaPoAkcija != null && fp.DividendaPoAkcija != 0))
                .OrderByDescending(h => h.Izdavach.FinansiskiPokazateli
                        .Where(fp => fp.Godina == latestYear && fp.DividendaPoAkcija != null && fp.DividendaPoAkcija != 0)
                        .Select(fp => fp.DividendaPoAkcija)
                        .FirstOrDefault())
                    .ToListAsync();
        }

        public async Task<IEnumerable<HartiiOdVrednost>> GetMostProfitableSecuritiesByDividendYieldAsync()
        {
            int latestYear = await this.GetLatestYearAsync();

            return await _dbContext.HartiiOdVrednost
                 .Include(hv => hv.Izdavach)
                .ThenInclude(i => i.FinansiskiPokazateli)
                .Where(h => h.Izdavach.FinansiskiPokazateli
                    .Any(fp => fp.Godina == latestYear && fp.DividendenPrinos != null && fp.DividendenPrinos != 0))
                .OrderByDescending(h => h.Izdavach.FinansiskiPokazateli
                        .Where(fp => fp.Godina == latestYear && fp.DividendenPrinos != null && fp.DividendenPrinos != 0)
                        .Select(fp => fp.DividendenPrinos)
                        .FirstOrDefault())
                    .ToListAsync();
        }

        public async Task<IEnumerable<HartiiOdVrednost>> GetSecuritiesWithBiggestPriceOscillationsAsync()
        {
            DateTime latestDate = await this.GetLatestDateAsync();

            return await _dbContext.HartiiOdVrednost
                .Include(hv => hv.DnevenPromet)
                .Where(hv => hv.DnevenPromet
                    .Any(dp => dp.Datum == latestDate && dp.MaxCena != null && dp.MinCena != null))
                .OrderByDescending(hv => hv.DnevenPromet
                    .Where(dp => dp.Datum == latestDate && dp.MaxCena != null && dp.MinCena != null
                        && (dp.MaxCena - dp.MinCena) > 0)
                    .Select(dp => (dp.MaxCena ?? 0) - (dp.MinCena ?? 0))
                    .FirstOrDefault())
                .ToListAsync();
        }

        public async Task<IEnumerable<HartiiOdVrednost>> GetSecuritiesWithSmallestPriceOscillationsAsync()
        {
            DateTime latestDate = await this.GetLatestDateAsync();

            return await _dbContext.HartiiOdVrednost
                .Include(hv => hv.DnevenPromet)
                .Where(hv => hv.DnevenPromet
                    .Any(dp => dp.Datum == latestDate && dp.MaxCena != null && dp.MinCena != null))
                .OrderBy(hv => hv.DnevenPromet
                    .Where(dp => dp.Datum == latestDate && dp.MaxCena != null && dp.MinCena != null 
                        && (dp.MaxCena - dp.MinCena) > 0)
                    .Select(dp => (dp.MaxCena ?? 0) - (dp.MinCena ?? 0))
                    .FirstOrDefault())
                .ToListAsync();
        }

        public async Task<IEnumerable<HartiiOdVrednost>> GetLeastLiquidSecuritiesByTradedQuantityAsync()
        {
            DateTime latestDate = await this.GetLatestDateAsync();

            return await _dbContext.HartiiOdVrednost
                .Include(hv => hv.DnevenPromet)
                .Where(hv => hv.DnevenPromet
                    .Any(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii > 0))
                .OrderBy(hv => hv.DnevenPromet
                    .Where(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii > 0)
                    .Select(dp => dp.KolicinaIstrguvaniAkcii)
                    .FirstOrDefault())
                .ToListAsync();
        }

        public async Task<IEnumerable<HartiiOdVrednost>> GetSecuritiesValuationAsync()
        {
            int latestYear = await this.GetLatestYearAsync();

            DateTime latestDate = await this.GetLatestDateAsync();

            return await _dbContext.HartiiOdVrednost
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
                .ToListAsync();
        }

        public async Task<IEnumerable<HartiiOdVrednost>> GetMostLiquidSecuritiesByTradedQuantityAsync()
        {
            {
                DateTime latestDate = await this.GetLatestDateAsync();

                return await _dbContext.HartiiOdVrednost
                    .Include(hv => hv.DnevenPromet)
                    .Where(hv => hv.DnevenPromet
                        .Any(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii > 0))
                    .OrderByDescending(hv => hv.DnevenPromet
                        .Where(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii > 0)
                        .Select(dp => dp.KolicinaIstrguvaniAkcii)
                        .FirstOrDefault())
                    .ToListAsync();
            }
        }

        public async Task<IEnumerable<HartiiOdVrednost>> GetMostLiquidSecuritiesByNumTradingDaysAsync()
        {
            return await _dbContext.HartiiOdVrednost
                .Include(hv => hv.DnevenPromet)
                .Where(hv => hv.DnevenPromet
                    .Any(dp => dp.KolicinaIstrguvaniAkcii > 0))
                .OrderByDescending(hv => hv.DnevenPromet
                    .Count(dp => dp.KolicinaIstrguvaniAkcii > 0))
                .ToListAsync();

        }

        public async Task<IEnumerable<HartiiOdVrednost>> GetLeastLiquidSecuritiesByNumTradingDaysAsync()
        {
            return await _dbContext.HartiiOdVrednost
                .Include(hv => hv.DnevenPromet)
                .Where(hv => hv.DnevenPromet
                    .Any(dp => dp.KolicinaIstrguvaniAkcii > 0))
                .OrderBy(hv => hv.DnevenPromet
                    .Count(dp => dp.KolicinaIstrguvaniAkcii > 0))
                .ToListAsync();

        }

        public async Task<int> GetLatestYearAsync()
        {
            return await _dbContext.FinansiskiPokazateli
                .MaxAsync(fp => fp.Godina) - 1;
        }

        public async Task<DateTime> GetLatestDateAsync()
        {
            return await _dbContext.DnevenPromet
                .MaxAsync(dp => dp.Datum);
        }
    }
}
