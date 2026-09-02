using DataAccess.Models;
using DataAccess.Repositories;
using PriceFlowApp.DTOs;

namespace PriceFlowApp.Services
{
    public class SecurityFilterService : ISecurityFilterService
    {
        private readonly ISecurityFilterRepository _securityFilterRepository;

        public SecurityFilterService(ISecurityFilterRepository securityFilterRepository)
        {
            _securityFilterRepository = securityFilterRepository;
        }

        public IEnumerable<FilteredSecurity> FindMostProfitableSecuritiesByDividendYield()
        {
            IEnumerable<HartiiOdVrednost> securities = _securityFilterRepository.GetMostProfitableSecuritiesByDividendYield();

            int latestYear = _securityFilterRepository.GetLatestYear();

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


        public IEnumerable<FilteredSecurity> FindMostProfitableSecuritiesByDividendPerShare()
        {
            IEnumerable<HartiiOdVrednost> securities = _securityFilterRepository.GetMostProfitableSecuritiesByDividendPerShare();

            int latestYear = _securityFilterRepository.GetLatestYear();

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


        public IEnumerable<FilteredSecurity> FindSecuritiesWithBiggestPriceOscillations()
        {
            IEnumerable<HartiiOdVrednost> securities = _securityFilterRepository.GetSecuritiesWithBiggestPriceOscillations();

            DateTime latestDate = _securityFilterRepository.GetLatestDate();

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

        public IEnumerable<FilteredSecurity> FindSecuritiesWithSmallestPriceOscillations()
        {
            IEnumerable<HartiiOdVrednost> securities = _securityFilterRepository.GetSecuritiesWithSmallestPriceOscillations();

            DateTime latestDate = _securityFilterRepository.GetLatestDate();

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

        public IEnumerable<FilteredSecurity> FindLeastLiquidSecuritiesByTradedQuantity()
        {
            IEnumerable<HartiiOdVrednost> securities = _securityFilterRepository.GetLeastLiquidSecuritiesByTradedQuantity();

            DateTime latestDate = _securityFilterRepository.GetLatestDate();

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

        public IEnumerable<FilteredSecurity> FindMostLiquidSecuritiesByTradedQuantity()
        {
            IEnumerable<HartiiOdVrednost> securities = _securityFilterRepository.GetMostLiquidSecuritiesByTradedQuantity();

            DateTime latestDate = _securityFilterRepository.GetLatestDate();

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

        public IEnumerable<FilteredSecurity> FindLeastLiquidSecuritiesByNumTradingDays()
        {
            IEnumerable<HartiiOdVrednost> securities = _securityFilterRepository.GetLeastLiquidSecuritiesByNumTradingDays();

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

        public IEnumerable<FilteredSecurity> FindMostLiquidSecuritiesByNumTradingDays()
        {
            IEnumerable<HartiiOdVrednost> securities = _securityFilterRepository.GetMostLiquidSecuritiesByNumTradingDays();

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

        public IEnumerable<Sector> FindMostProfitableSectorsByDividendYield()
        {
            IEnumerable<Sektori> sectors = _securityFilterRepository.GetMostProfitableSectorsByDividendYield();

            int latestYear = _securityFilterRepository.GetLatestYear();

            return sectors.Select(s =>
            {
                decimal dividendYield = s.Izdavachi
                .SelectMany(i => i.FinansiskiPokazateli)
                .Where(fp => fp.Godina == latestYear && fp.DividendenPrinos != null)
                .Sum(fp => fp.DividendenPrinos ?? 0);

                return new Sector
                {
                    Id = s.Id,
                    Name = s.Ime,
                    TotalValue = dividendYield
                };
            })
            .Where(x => x.TotalValue > 0)
            .ToList();
        }

        public IEnumerable<Sector> FindMostProfitableSectorsByProfit()
        {
            IEnumerable<Sektori> sectors = _securityFilterRepository.GetMostProfitableSectorsByProfit();

            int latestYear = _securityFilterRepository.GetLatestYear();

            return sectors.Select(s =>
            {
                decimal profit = s.Izdavachi
                .SelectMany(i => i.FinansiskiPokazateli)
                .Where(fp => fp.Godina == latestYear && fp.OperativnaDobivka != null && fp.OperativnaDobivka > 0)
                .Sum(fp => fp.OperativnaDobivka ?? 0);

                return new Sector
                {
                    Id = s.Id,
                    Name = s.Ime,
                    TotalValue = profit
                };
            })
            .Where(x => x.TotalValue > 0)
            .ToList();
        }

        public IEnumerable<FilteredSecurity> FindSecuritiesValuation()
        {
            IEnumerable<HartiiOdVrednost> securities = _securityFilterRepository.GetSecuritiesValuation();

            int latestYear = _securityFilterRepository.GetLatestYear();

            DateTime latestDate = _securityFilterRepository.GetLatestDate();

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
    }
}
