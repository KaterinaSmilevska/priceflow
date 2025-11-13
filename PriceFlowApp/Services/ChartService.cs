using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class ChartService : IChartService
    {
        private readonly PriceFlowDbContext _dbContext;
        private readonly ISecuritiesService _securitiesService;

        public ChartService(PriceFlowDbContext dbContext, ISecuritiesService securitiesService)
        {
            _dbContext = dbContext;
            _securitiesService = securitiesService;
        }

        public async Task<IEnumerable<PriceTrend>> GetPriceTrendAsync(int securityId, DateTime startDate, DateTime endDate)
        {
            return await _dbContext.DnevenPromet
                .Where(dp => dp.Hvid == securityId && dp.Datum >= startDate && dp.Datum <= endDate)
                .OrderBy(dp => dp.Datum)
                .Select(dp => new PriceTrend
                {
                    Date = dp.Datum,
                    Price = (decimal)dp.CenaPoslednaTransakcija
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SectorDistribution>> GetSectorDistributionAsync(DateTime date)
        {
            return await _dbContext.HartiiOdVrednost
                .Join(_dbContext.DnevenPromet, hv => hv.Id, dp => dp.Hvid, (hv, dp) => new { hv, dp })
                .Where(x => x.dp.Datum == date)
                .Select( x => new
                {
                    SectorName = x.hv.Izdavach.Sektor.Ime,
                    MarketCap = x.hv.VkupenBrojAkcii * x.dp.CenaPoslednaTransakcija
                })
                .GroupBy(x => x.SectorName)
                .Select(g => new SectorDistribution
                {
                    SectorName = g.Key,
                    MarketCap = (decimal)g.Sum(x => x.MarketCap)

                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Security>> GetSecurities()
        {
            IEnumerable<Security> foundSecurities = await _securitiesService.FindAllAsync();
            return foundSecurities.Select(security => new Security
            {
                Id = security.Id,
                Code = security.Code,
            });
        }


        public DateTime? FindLatestDate()
        {
            return _dbContext.DnevenPromet
                .OrderByDescending(dp => dp.Datum)
                .Select(dp => dp.Datum)
                .FirstOrDefault();
        }
    }
}
