using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class DailyTurnoverRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private DailyTurnoverRepository _repository = null!;
        private string _databaseName = null!;

        private const int Security1Id = 1;
        private const int Security2Id = 2;
        private const int Security3Id = 3;

        private static readonly DateTime Today = DateTime.Today;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new DailyTurnoverRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var sector = new Sektori
            {
                Id = 1,
                Ime = "Sektor 1"
            };

            var issuer = new Izdavachi
            {
                Id = 1,
                Ime = "Issuer 1",
                Sektor = sector,
                Grad = "Skopje",
                Drzava = "Makedonija"
            };

            var alk = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                Isin = "MKALK101011",
                Izdavach = issuer
            };

            var kmb = new HartiiOdVrednost
            {
                Id = 2,
                Kod = "KMB",
                Isin = "MKKMBS101019",
                Izdavach = issuer
            };

            var tel = new HartiiOdVrednost
            {
                Id = 3,
                Kod = "TEL",
                Isin = "MKMTSK101019",
                Izdavach = issuer
            };

            _context.HartiiOdVrednost.AddRange(alk, kmb, tel);
            _context.SaveChanges();

            AddTurnover(Security1Id, Today.AddDays(-5), 100m, 1000, 1.5m);
            AddTurnover(Security1Id, Today.AddDays(-3), 110m, 1200, 2.0m);
            AddTurnover(Security1Id, Today.AddDays(-1), 115m, 1500, 3.0m);
            AddTurnover(Security2Id, Today.AddDays(-4), 200m, 2000, -1.0m);
            AddTurnover(Security2Id, Today.AddDays(-2), 210m, 2500, 0.5m);
            AddTurnover(Security3Id, Today.AddDays(-3), null, null, null);

            _context.SaveChanges();
        }

        private void AddTurnover(int securityId, DateTime date, decimal? price, int? quantity, decimal? percentageChange)
        {
            _context.Add(new DnevenPromet
            {
                Hvid = securityId,
                Datum = date,
                CenaPoslednaTransakcija = price,
                KolicinaIstrguvaniAkcii = quantity,
                ProcentPromena = percentageChange
            });
        }

        [TestMethod]
        public void GetBySecurityCode_WhenSecurityExists_ReturnsMatchingTurnoverInDescendingDateOrder()
        {
            var result = _repository.GetBySecurityCode("ALK", Today).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(x => x.Hvid == Security1Id));
            Assert.IsTrue(result[0].Datum > result[1].Datum);
            Assert.IsTrue(result[1].Datum > result[2].Datum);
            Assert.IsNotNull(result[0].Hv);
            Assert.AreEqual("ALK", result[0].Hv.Kod);
        }

        [TestMethod]
        public void GetBySecurityCode_WhenDateExcludesAllRecords_ReturnsEmptyCollection()
        {
            var result = _repository.GetBySecurityCode("ALK", Today.AddDays(-10)).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetBySecurityCode_WhenSecurityCodeDoesNotExist_ReturnsEmptyCollection()
        {
            var result = _repository.GetBySecurityCode("Unknown", Today).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetBySecuritiesIds_WhenResolutionIsNull_ReturnsMatchingSecuritiesWithPrices()
        {
            var securitiesIds = new List<int>
            {
                Security1Id,
                Security2Id
            };

            var result = _repository
                .GetBySecuritiesIds(securitiesIds, null, null)
                .ToList();

            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.All(x => securitiesIds.Contains(x.Hvid)));
            Assert.IsTrue(result.All(x => x.CenaPoslednaTransakcija.HasValue));
            Assert.IsTrue(result.SequenceEqual(result.OrderBy(x => x.Datum)));
        }

        [TestMethod]
        public void GetBySecuritiesIds_WhenSecurityIdsAreEmpty_ReturnsEmptyCollection()
        {
            var result = _repository
                .GetBySecuritiesIds(new List<int>(), null, null)
                .ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetBySecuritiesIds_WhenResolutionIsDay_ReturnsOneRecordPerSecurityPerDay()
        {
            var securiitesIds = new List<int>
            {
                Security1Id,
                Security2Id
            };

            var result = _repository
                .GetBySecuritiesIds(securiitesIds, PriceTrendPeriod.Monthly, PriceTrendResolution.Day)
                .ToList();

            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.All(x => x.CenaPoslednaTransakcija.HasValue));
        }

        [TestMethod]
        public void GetBySecuritiesIds_WhenResolutionIsWeek_ExcludesCurrentWeek()
        {
            var securityIds = new List<int>
            {
                Security1Id,
                Security2Id
            };

            var result = _repository
                .GetBySecuritiesIds(securityIds, null, PriceTrendResolution.Week)
                .ToList();

            Assert.IsTrue(result.All(x => x.Datum < Today));
            Assert.IsTrue(result.All(x => securityIds.Contains(x.Hvid)));
        }

        [TestMethod]
        public void GetBySecuritiesIds_WhenResolutionIsMonth_ExcludesCurrentMonth()
        {
            var securitiesIds = new List<int>
            {
                Security1Id,
                Security2Id
            };

            var result = _repository
                .GetBySecuritiesIds(securitiesIds, null, PriceTrendResolution.Month)
                .ToList();

            Assert.IsTrue(result.All(x =>
            x.Datum.Year < Today.Year ||
            x.Datum.Year == Today.Year && x.Datum.Month < Today.Month));
        }

        [TestMethod]
        public void GetBySecuritiesIds_WhenResolutionIsQuarter_ExcludesCurrentQuarter()
        {
            var securitiesIds = new List<int>
            {
                Security1Id,
                Security2Id
            };

            var result = _repository
                .GetBySecuritiesIds(securitiesIds, null, PriceTrendResolution.Quarter)
                .ToList();

            Assert.IsTrue(result.All(x => x.CenaPoslednaTransakcija.HasValue));
            Assert.IsTrue(result.All(x => securitiesIds.Contains(x.Hvid)));
            Assert.IsTrue(result.All(x =>
            {
                int quarter = (x.Datum.Month - 1) / 3 + 1;
                var quarterEnd = new DateTime(
                    x.Datum.Year,
                    quarter * 3,
                    DateTime.DaysInMonth(x.Datum.Year, quarter * 3));

                return quarterEnd < Today;
            }));
        }

        [TestMethod]
        public void GetLiquidity_WhenSecurityIdsAreNull_ReturnsRecordsWithNonNullQuantity()
        {
            var result = _repository
                .GetLiquidity(null, Today.AddDays(-10))
                .ToList();

            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.All(x => x.KolicinaIstrguvaniAkcii.HasValue));
        }

        [TestMethod]
        public void GetLiquidity_WhenSecurityIdsAreProvided_ReturnsOnlyPositiveQuantities()
        {
            var securitiesIds = new List<int>
            {
                Security1Id
            };

            var result = _repository
                .GetLiquidity(securitiesIds, Today.AddDays(-10))
                .ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(x => x.Hvid == Security1Id));
            Assert.IsTrue(result.All(x => x.KolicinaIstrguvaniAkcii > 0));
        }

        [TestMethod]
        public void GetLiquidity_WhenFromDateIsInFuture_ReturnsEmptyCollection()
        {
            var result = _repository
                .GetLiquidity(null, Today.AddYears(1))
                .ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetBySecurityAndDateRange_ReturnsMatchingRecordsInAscendingDateOrder()
        {
            var result = _repository
                .GetBySecurityAndDateRange(Security1Id, Today.AddDays(-4), Today)
                .ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(x => x.Hvid == Security1Id));
            Assert.IsTrue(result[0].Datum < result[1].Datum);
        }

        [TestMethod]
        public void GetBySecurityAndDateRange_WhenNoRecordsMatch_ReturnsEmptyCollection()
        {
            var result = _repository
                .GetBySecurityAndDateRange(Security3Id, Today.AddDays(-10), Today)
                .ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(Security3Id, result[0].Hvid);
        }

        [TestMethod]
        public void GetByDateWithSecurity_ReturnsRecordsForSpecifiedDay()
        {
            var date = Today.AddDays(-3);

            var result = _repository
                .GetByDateWithSecurity(date)
                .ToList();

            Assert.IsTrue(result.All(x => x.Datum >= date && x.Datum < date.AddDays(1)));
            Assert.IsTrue(result.Any(x => x.Hvid == Security1Id));
            Assert.IsTrue(result.Any(x => x.Hvid == Security3Id));
            Assert.IsNotNull(result.First(x => x.Hvid == Security1Id).Hv);
        }

        [TestMethod]
        public void GetDailyTurnoverForTotalMarketCap_ExcludesRecordsWithoutLastTransactionPrice()
        {
            var date = Today.AddDays(-3);

            var result = _repository
                .GetDailyTurnoverForTotalMarketCap(date)
                .ToList();

            Assert.IsTrue(result.All(x =>
            x.Datum >= date &&
            x.Datum < date.AddDays(1) &&
            x.CenaPoslednaTransakcija.HasValue));
        }

        [TestMethod]
        public void GetByDateRange_ReturnsRecordsWithNonNullQuantityWithinRange()
        {
            var result = _repository
                .GetByDateRange(Today.AddDays(-4), Today.AddDays(-2))
                .ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(x => 
            x.Datum >= Today.AddDays(-4) &&
            x.Datum <= Today.AddDays(-2) &&
            x.KolicinaIstrguvaniAkcii.HasValue));
        }

        [TestMethod]
        public void GetLatestPrices_ReturnsLatestPricedRecordForEachSecurity()
        {
            var result = _repository
                .GetLatestPrices(new List<int>
                {
                    Security1Id,
                    Security2Id,
                    Security3Id
                })
                .ToList();

            Assert.AreEqual(2, result.Count);

            var alk = result.Single(x => x.Hvid == Security1Id);
            Assert.AreEqual(Today.AddDays(-1), alk.Datum);
            Assert.AreEqual(115m, alk.CenaPoslednaTransakcija);

            var kmb = result.Single(x => x.Hvid == Security2Id);
            Assert.AreEqual(Today.AddDays(-2), kmb.Datum);
            Assert.AreEqual(210m, kmb.CenaPoslednaTransakcija);
        }

        [TestMethod]
        public void GetLatestPrices_WhenSecurityIdsAreEmpty_ReturnsEmptyCollection()
        {
            var result = _repository
                .GetLatestPrices(new List<int>())
                .ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetLatestPrice_ReturnsLatestPriceOnOrBeforeSpecifiedDate()
        {
            var result = _repository
                .GetLatestPrice(Security1Id, DateOnly.FromDateTime(Today.AddDays(-2)));

            Assert.AreEqual(110m, result);
        }

        [TestMethod]
        public void GetLatestPrice_WhenNoPriceExistsOnOrBeforeDate_ReturnsNull()
        {
            var result = _repository
                .GetLatestPrice(Security1Id, DateOnly.FromDateTime(Today.AddDays(-10)));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetLatestDate_ReturnsMaximumTurnoverDate()
        {
            var result = _repository.GetLatestDate();

            Assert.AreEqual(Today.AddDays(-1), result);
        }

        [TestMethod]
        public void ExistsForDate_WhenRecordsExistOnDate_ReturnsTrue()
        {
            var result = _repository.ExistsForDate(Today.AddDays(-1));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExistsForDate_WhenNoRecordsExistOnDate_ReturnsFalse()
        {
            var result = _repository.ExistsForDate(Today.AddDays(1));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetByDate_ReturnsOnlyRecordsWithPercentageChange()
        {
            var date = Today.AddDays(-3);

            var result = _repository.GetByDate(date).ToList();

            Assert.IsTrue(result.All(x =>
            x.Datum >= date &&
            x.Datum < date.AddDays(1) &&
            x.ProcentPromena.HasValue));

            Assert.IsFalse(result.Any(x => x.Hvid == Security3Id));
        }
    }
}
