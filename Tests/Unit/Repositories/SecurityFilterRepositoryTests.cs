using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class SecurityFilterRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private SecurityFilterRepository _repository = null!;
        private string _databaseName = null!;

        private readonly DateTime _latestDate = new DateTime(2026, 10, 20);
        private const int LatestYear = 2025;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new SecurityFilterRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var sector1 = new Sektori
            {
                Id = 1,
                Ime = "Pharmaceuticals",
                DateModified = _latestDate
            };

            var sector2 = new Sektori
            {
                Id = 2,
                Ime = "Banking",
                DateModified = _latestDate
            };

            var issuer1 = new Izdavachi
            {
                Id = 1,
                Ime = "Alkaloid AD Skopje",
                Grad = "Skopje",
                Drzava = "Makedonija",
                SektorId = 1,
                Sektor = sector1,
                DateModified = _latestDate
            };

            var issuer2 = new Izdavachi
            {
                Id = 2,
                Ime = "Komercijalna banka AD Skopje",
                Grad = "Skopje",
                Drzava = "Makedonija",
                SektorId = 2,
                Sektor = sector2,
                DateModified = _latestDate
            };

            var issuer3 = new Izdavachi
            {
                Id = 3,
                Ime = "Stopanska banka AD Skopje",
                Grad = "Skopje",
                Drzava = "Makedonija",
                SektorId = 2,
                Sektor = sector2,
                DateModified = _latestDate
            };

            var securityType = new TipHv
            {
                Id = 1,
                Ime = "Shares",
                DateModified = _latestDate
            };

            _context.Sektori.AddRange(sector1, sector2);
            _context.Izdavachi.AddRange(issuer1, issuer2, issuer3);
            _context.TipHv.Add(securityType);

            var security1 = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                Isin = "MKALK101011",
                VkupenBrojAkcii = 100000,
                TipHvid = 1,
                IzdavachId = 1,
                Izdavach = issuer1,
                TipHv = securityType,
                DateModified = _latestDate
            };

            var security2 = new HartiiOdVrednost
            {
                Id = 2,
                Kod = "KMB",
                Isin = "MKKMBS101019",
                VkupenBrojAkcii = 200000,
                TipHvid = 1,
                IzdavachId = 2,
                Izdavach = issuer2,
                TipHv = securityType,
                DateModified = _latestDate
            };

            var security3 = new HartiiOdVrednost
            {
                Id = 3,
                Kod = "STB",
                Isin = "MKSTBS101014",
                VkupenBrojAkcii = 300000,
                TipHvid = 1,
                IzdavachId = 3,
                Izdavach = issuer3,
                TipHv = securityType,
                DateModified = _latestDate
            };

            _context.HartiiOdVrednost.AddRange(security1, security2, security3);

            _context.FinansiskiPokazateli.AddRange(
                new FinansiskiPokazateli
                {
                    Id = 1,
                    IzdavachId = 1,
                    Izdavach = issuer1,
                    Godina = LatestYear,
                    DividendenPrinos = 5m,
                    DividendaPoAkcija = 2m,
                    OperativnaDobivka = 1000m,
                    KnigovodstvenaVrednostPoAkcija = 10m,
                    DateModified = _latestDate
                },
                new FinansiskiPokazateli
                {
                    Id = 2,
                    IzdavachId = 2,
                    Izdavach = issuer2,
                    Godina = LatestYear,
                    DividendenPrinos = 2m,
                    DividendaPoAkcija = 4m,
                    OperativnaDobivka = 2000m,
                    KnigovodstvenaVrednostPoAkcija = 20m,
                    DateModified = _latestDate
                },
                new FinansiskiPokazateli
                {
                    Id = 3,
                    IzdavachId = 3,
                    Izdavach = issuer3,
                    Godina = LatestYear,
                    DividendenPrinos = null,
                    DividendaPoAkcija = null,
                    OperativnaDobivka = null,
                    KnigovodstvenaVrednostPoAkcija = null,
                    DateModified = _latestDate
                },
                new FinansiskiPokazateli
                {
                    Id = 4,
                    IzdavachId = 1,
                    Izdavach = issuer1,
                    Godina = LatestYear - 1,
                    DividendenPrinos = 9m,
                    DividendaPoAkcija = 8m,
                    OperativnaDobivka = 0000m,
                    KnigovodstvenaVrednostPoAkcija = 1m,
                    DateModified = _latestDate.AddYears(-1)
                });

            _context.DnevenPromet.AddRange(
                CreateTurnover(1, 1, _latestDate, 15m, 10m, 100, 15m),
                CreateTurnover(2, 2, _latestDate, 12m, 10m, 300, 12m),
                CreateTurnover(3, 3, _latestDate, 18m, 10m, 50, 18m),
                CreateTurnover(4, 1, _latestDate.AddDays(-1), 13m, 10m, 80, 13m),
                CreateTurnover(5, 2, _latestDate.AddDays(-1), 11m, 10m, 150, 11m),
                CreateTurnover(6, 3, _latestDate.AddDays(-1), 17m, 12m, 40, 12m),
                CreateTurnover(7, 3, _latestDate.AddDays(-2), 16m, 12m, 30, 16m)
            );

            _context.SaveChanges();
        }

        private DnevenPromet CreateTurnover(int id, int securityId, DateTime date, decimal? maxPrice, decimal? minPrice, 
            int? quantity, decimal? lastPrice)
        {
            return new DnevenPromet
            {
                Id = id,
                Hvid = securityId,
                Datum = date,
                MaxCena = maxPrice,
                MinCena = minPrice,
                KolicinaIstrguvaniAkcii = quantity,
                CenaPoslednaTransakcija = lastPrice,
                DateModified = date
            };
        }

        [TestMethod]
        public void GetMostProfitableSecuritiesByDividendYield_ReturnsSecuritiesOrderedDescending()
        {
            var result = _repository.GetMostProfitableSecuritiesByDividendYield().ToList();

            CollectionAssert.AreEqual(
                new[] { "ALK", "KMB" },
                result.Select(s => s.Kod).ToArray());
        }

        [TestMethod]
        public void GetMostProfitableSecuritiesByDividendPerShare_ReturnsSecuritiesOrderedDescending()
        {
            var result = _repository.GetMostProfitableSecuritiesByDividendPerShare().ToList();

            CollectionAssert.AreEqual(
               new[] { "KMB", "ALK" },
               result.Select(s => s.Kod).ToArray());
        }

        [TestMethod]
        public void GetSecurititesWithBiggestPriceOscillations_ReturnsSecuritiesOrderedDescending()
        {
            var result = _repository.GetSecuritiesWithBiggestPriceOscillations().ToList();

            CollectionAssert.AreEqual(
               new[] { "STB", "ALK", "KMB" },
               result.Select(s => s.Kod).ToArray());
        }

        [TestMethod]
        public void GetSecurititesWithSmallestPriceOscillations_ReturnsSecuritiesOrderedAscending()
        {
            var result = _repository.GetSecuritiesWithSmallestPriceOscillations().ToList();

            CollectionAssert.AreEqual(
               new[] { "KMB", "ALK", "STB" },
               result.Select(s => s.Kod).ToArray());
        }

        [TestMethod]
        public void GetLeastLiquidSecuritiesByTradedQuantity_ReturnsSecuritiesOrderedAscending()
        {
            var result = _repository.GetLeastLiquidSecuritiesByTradedQuantity().ToList();
            
            CollectionAssert.AreEqual(
                new[] { "STB", "ALK", "KMB" },
                result.Select(s => s.Kod).ToArray());
        }

        [TestMethod]
        public void GetMostLiquidSecuritiesByTradedQuantity_ReturnsSecuritiesOrderedDescending()
        {
            var result = _repository.GetMostLiquidSecuritiesByTradedQuantity().ToList();

            CollectionAssert.AreEqual(
                new[] { "KMB", "ALK", "STB" },
                result.Select(s => s.Kod).ToArray());
        }

        [TestMethod]
        public void GetLeastLiquidSecuritiesByNumTradingDays_ReturnsSecuritiesOrderedAscending()
        {
            var result = _repository.GetLeastLiquidSecuritiesByNumTradingDays().ToList();

            CollectionAssert.AreEqual(
                new[] { "ALK", "KMB", "STB" },
                result.Select(s => s.Kod).ToArray());
        }

        [TestMethod]
        public void GetMostLiquidSecuritiesByNumTradingDays_ReturnsSecuritiesOrderedDescending()
        {
            var result = _repository.GetMostLiquidSecuritiesByNumTradingDays().ToList();

            CollectionAssert.AreEqual(
                new[] { "STB", "ALK", "KMB" },
                result.Select(s => s.Kod).ToArray());
        }

        [TestMethod]
        public void GetMostProfitableSectorsByDividendYield_ReturnsSectorsOrderedDescending()
        {
            var result = _repository.GetMostProfitableSectorsByDividendYield().ToList();

            CollectionAssert.AreEqual(
                new[] { "Pharmaceuticals", "Banking" },
                result.Select(s => s.Ime).ToArray());
        }

        [TestMethod]
        public void GetMostProfitableSectorsByProfit_ReturnsSectorsOrderedDescending()
        {
            var result = _repository.GetMostProfitableSectorsByProfit().ToList();

            CollectionAssert.AreEqual(
                new[] { "Banking", "Pharmaceuticals" },
                result.Select(s => s.Ime).ToArray());
        }

        [TestMethod]
        public void GetSecuritiesValuation_ReturnsSecuritiesOrderedByPriceToBookRatioDescending()
        {
            var result = _repository.GetSecuritiesValuation().ToList();

            CollectionAssert.AreEqual(
                new[] { "ALK", "KMB" },
                result.Select(s => s.Kod).ToArray());
        }

        [TestMethod]
        public void GetLatestDate_ReturnsMostRecentTradingDate()
        {
            var result = _repository.GetLatestDate();

            Assert.AreEqual(_latestDate, result);
        }

        [TestMethod]
        public void GetLatestYear_ReturnsMostRecentFinancialYear()
        {
            var result = _repository.GetLatestYear();

            Assert.AreEqual(LatestYear, result);
        }

    }
}
