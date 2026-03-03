using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using PriceFlowApp.DTOs;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace PriceFlowApp.Services
{
    public class SecurityFilterService : ISecurityFilterService
    {
        private readonly ISecurityFilterRepository _securityFilterRepository;

        public SecurityFilterService(ISecurityFilterRepository securityFilterRepository)
        {
            _securityFilterRepository = securityFilterRepository;
        }

        public async Task<IEnumerable<FilteredSecurity>> FindLeastLiquidSecuritiesByNumTradingDaysAsync()
        {
            IEnumerable<HartiiOdVrednost> securities = await _securityFilterRepository.GetLeastLiquidSecuritiesByNumTradingDaysAsync();

            return securities.Select(s =>
            {
                int numTradingDays = s.DnevenPromet
                .Count(dp => dp.KolicinaIstrguvaniAkcii > 0);

                return new FilteredSecurity
                {
                    SecurityId = s.Id,
                    SecurityCode = s.Kod,
                    Value = numTradingDays
                };
            })
            .Where(x => x.Value > 0)
            .ToList();
        }

        public async Task<IEnumerable<FilteredSecurity>> FindLeastLiquidSecuritiesByTradedQuantityAsync()
        {
            IEnumerable<HartiiOdVrednost> securities = await _securityFilterRepository.GetLeastLiquidSecuritiesByTradedQuantityAsync();

            DateTime latestDate = await _securityFilterRepository.GetLatestDateAsync();

            return securities.Select(s =>
            {
                int? tradedQuantity = s.DnevenPromet
                    .Where(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii > 0)
                    .Select(dp => dp.KolicinaIstrguvaniAkcii)
                    .FirstOrDefault();

                return new FilteredSecurity
                {
                    SecurityId = s.Id,
                    SecurityCode = s.Kod,
                    Value = tradedQuantity
                };
            })
            .Where(x => x.Value > 0)
            .ToList();
        }

        public async Task<IEnumerable<FilteredSecurity>> FindMostLiquidSecuritiesByNumTradingDaysAsync()
        {
            IEnumerable<HartiiOdVrednost> securities = await _securityFilterRepository.GetMostLiquidSecuritiesByNumTradingDaysAsync();

            return securities.Select(s =>
            {
                int numTradingDays = s.DnevenPromet
                .Count(dp => dp.KolicinaIstrguvaniAkcii > 0);

                return new FilteredSecurity
                {
                    SecurityId = s.Id,
                    SecurityCode = s.Kod,
                    Value = numTradingDays
                };
            })
            .Where(x => x.Value > 0)
            .ToList();
        }

        public async Task<IEnumerable<FilteredSecurity>> FindMostLiquidSecuritiesByTradedQuantityAsync()
        {
            IEnumerable<HartiiOdVrednost> securities = await _securityFilterRepository.GetMostLiquidSecuritiesByTradedQuantityAsync();

            DateTime latestDate = await _securityFilterRepository.GetLatestDateAsync();

            return securities.Select(s =>
            {
                int? tradedQuantity = s.DnevenPromet
                    .Where(dp => dp.Datum == latestDate && dp.KolicinaIstrguvaniAkcii > 0)
                    .Select(dp => dp.KolicinaIstrguvaniAkcii)
                    .FirstOrDefault();

                return new FilteredSecurity
                {
                    SecurityId = s.Id,
                    SecurityCode = s.Kod,
                    Value = tradedQuantity
                };
            })
            .Where(x => x.Value > 0)
            .ToList();
        }

        public async Task<IEnumerable<Sector>> FindMostProfitableSectorsByDividendYieldAsync()
        {
            IEnumerable<Sektori> sectors = await _securityFilterRepository.GetMostProfitableSectorsByDividendYieldAsync();

            int latestYear = await _securityFilterRepository.GetLatestYearAsync();
            return sectors.Select(s =>
            {
                decimal dividendYield = s.Izdavachi
                .SelectMany(i => i.FinansiskiPokazateli)
                .Where(fp => fp.Godina == latestYear && fp.DividendenPrinos != null)
                .Sum(fp => fp.DividendenPrinos ?? 0);

                return new Sector
                {
                    SectorId = s.Id,
                    SectorName = s.Ime,
                    TotalValue = dividendYield
                };
            })
            .Where(x => x.TotalValue > 0)
            .ToList();
        }

        public async Task<IEnumerable<Sector>> FindMostProfitableSectorsByProfitAsync()
        {
            IEnumerable<Sektori> sectors = await _securityFilterRepository.GetMostProfitableSectorsByProfitAsync();

            int latestYear = await _securityFilterRepository.GetLatestYearAsync();
            return sectors.Select(s =>
            {
                decimal profit = s.Izdavachi
                .SelectMany(i => i.FinansiskiPokazateli)
                .Where(fp => fp.Godina == latestYear && fp.OperativnaDobivka != null && fp.OperativnaDobivka > 0)
                .Sum(fp => fp.OperativnaDobivka ?? 0);

                return new Sector
                {
                    SectorId = s.Id,
                    SectorName = s.Ime,
                    TotalValue = profit
                };
            })
            .Where(x => x.TotalValue > 0)
            .ToList();
        }

        public async Task<IEnumerable<FilteredSecurity>> FindMostProfitableSecuritiesByDividendPerShareAsync()
        {
            IEnumerable<HartiiOdVrednost> securities = await _securityFilterRepository.GetMostProfitableSecuritiesByDividendPerShareAsync();

            int latestYear = await _securityFilterRepository.GetLatestYearAsync();

            return securities.Select(s =>
            {
                decimal? dividendPerShare = s.Izdavach.FinansiskiPokazateli
                     .Where(fp => fp.Godina == latestYear && fp.DividendaPoAkcija != null && fp.DividendaPoAkcija != 0)
                     .Select(fp => fp.DividendaPoAkcija)
                     .FirstOrDefault();

                return new FilteredSecurity
                {
                    SecurityId = s.Id,
                    SecurityCode = s.Kod,
                    Value = dividendPerShare
                };
            })
            .Where(x => x.Value > 0)
            .ToList();
        }

        public async Task<IEnumerable<FilteredSecurity>> FindMostProfitableSecuritiesByDividendYieldAsync()
        {
            IEnumerable<HartiiOdVrednost> securities = await _securityFilterRepository.GetMostProfitableSecuritiesByDividendYieldAsync();

            int latestYear = await _securityFilterRepository.GetLatestYearAsync();

            return securities.Select(s =>
            {
                decimal? dividendYield = s.Izdavach.FinansiskiPokazateli
                    .Where(fp => fp.Godina == latestYear && fp.DividendenPrinos != null && fp.DividendenPrinos != 0)
                    .Select(fp => fp.DividendenPrinos)
                    .FirstOrDefault();

                return new FilteredSecurity
                {
                    SecurityId = s.Id,
                    SecurityCode = s.Kod,
                    Value = dividendYield
                };
            })
            .Where(x => x.Value > 0)
            .ToList();
        }

        public async Task<IEnumerable<FilteredSecurity>> FindSecuritiesValuationAsync()
        {
            IEnumerable<HartiiOdVrednost> securities = await _securityFilterRepository.GetSecuritiesValuationAsync();

            int latestYear = await _securityFilterRepository.GetLatestYearAsync();

            DateTime latestDate = await _securityFilterRepository.GetLatestDateAsync();

            return securities.Select(s =>
            {
                decimal valuation = s.DnevenPromet
                    .Where(dp => dp.Datum == latestDate && dp.CenaPoslednaTransakcija != null)
                    .Select(dp => dp.CenaPoslednaTransakcija ?? 0)
                    .FirstOrDefault()

                    /

                    s.Izdavach.FinansiskiPokazateli
                    .Where(fp => fp.Godina == latestYear && fp.KnigovodstvenaVrednostPoAkcija != null)
                    .Select(fp => fp.KnigovodstvenaVrednostPoAkcija ?? 1)
                    .FirstOrDefault();

                return new FilteredSecurity()
                {
                    SecurityId = s.Id,
                    SecurityCode = s.Kod,
                    Value = valuation
                };
            })
            .Where(x => x.Value > 0)
            .ToList();
        }

        public async Task<IEnumerable<FilteredSecurity>> FindSecuritiesWithBiggestPriceOscillationsAsync()
        {
            IEnumerable<HartiiOdVrednost> securities = await _securityFilterRepository.GetSecuritiesWithBiggestPriceOscillationsAsync();

            DateTime latestDate = await _securityFilterRepository.GetLatestDateAsync();

            return securities.Select(s =>
            {
                decimal priceOscillation = s.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.MaxCena != null && dp.MinCena != null 
                    && (dp.MaxCena - dp.MinCena) > 0)
                .Select(dp => (dp.MaxCena ?? 0) - (dp.MinCena ?? 0))
                .FirstOrDefault();


                return new FilteredSecurity
                {
                    SecurityId = s.Id,
                    SecurityCode = s.Kod,
                    Value = priceOscillation
                };
            })
            .Where(x => x.Value > 0)
            .ToList();
        }

        public async Task<IEnumerable<FilteredSecurity>> FindSecuritiesWithSmallestPriceOscillationsAsync()
        {
            IEnumerable<HartiiOdVrednost> securities = await _securityFilterRepository.GetSecuritiesWithSmallestPriceOscillationsAsync();

            DateTime latestDate = await _securityFilterRepository.GetLatestDateAsync();

            return securities.Select(s =>
            {
                decimal priceOscillation = s.DnevenPromet
                .Where(dp => dp.Datum == latestDate && dp.MaxCena != null && dp.MinCena != null
                    && (dp.MaxCena - dp.MinCena) > 0)
                .Select(dp => (dp.MaxCena ?? 0) - (dp.MinCena ?? 0))
                .FirstOrDefault();


                return new FilteredSecurity
                {
                    SecurityId = s.Id,
                    SecurityCode = s.Kod,
                    Value = Math.Round(priceOscillation, 4)
                };
            })
            .Where(x => x.Value > 0)
            .ToList();
        }
    }
}
