using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class MarketOverviewServiceTests
    {
        private Mock<ISecuritiesRepository> _securitiesRepositoryMock = null!;
        private Mock<ITransactionsRepository> _transactionsRepositoryMock = null!;
        private Mock<IDailyTurnoverRepository> _dailyTurnoverRepositoryMock = null!;
        private MarketOverviewService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _securitiesRepositoryMock = new Mock<ISecuritiesRepository>();
            _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
            _dailyTurnoverRepositoryMock = new Mock<IDailyTurnoverRepository>();
            _service = new MarketOverviewService(_securitiesRepositoryMock.Object, _transactionsRepositoryMock.Object,
                _dailyTurnoverRepositoryMock.Object);
        }

        [TestMethod]
        public void GetOverview_WithMarketData_ReturnsCorrectOverview()
        {
            var latestDate = new DateTime(2026, 10, 8);

            var securities = new List<HartiiOdVrednost>
            {
                new HartiiOdVrednost
                {
                    Id = 1,
                    Kod = "ALK",
                    VkupenBrojAkcii = 1000
                },
                new HartiiOdVrednost
                {
                    Id = 2,
                    Kod = "KMB",
                    VkupenBrojAkcii = 2000
                },
                new HartiiOdVrednost
                {
                    Id = 3,
                    Kod = "TEL",
                    VkupenBrojAkcii = 500
                }
            };

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = securities[0],
                    Datum = latestDate,
                    CenaPoslednaTransakcija = 100m,
                    ProcentPromena = 5m,
                    KolicinaIstrguvaniAkcii = 100
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Hv = securities[1],
                    Datum = latestDate,
                    CenaPoslednaTransakcija = 50m,
                    ProcentPromena = -3m,
                    KolicinaIstrguvaniAkcii = 200
                },
                new DnevenPromet
                {
                    Hvid = 3,
                    Hv = securities[2],
                    Datum = latestDate,
                    CenaPoslednaTransakcija = 200m,
                    ProcentPromena = 2m,
                    KolicinaIstrguvaniAkcii = null
                }
            };

            var monthlyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = securities[0],
                    Datum = latestDate,
                    CenaPoslednaTransakcija = 100m,
                    ProcentPromena = 5m,
                    KolicinaIstrguvaniAkcii = 100
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Hv = securities[1],
                    Datum = latestDate,
                    CenaPoslednaTransakcija = 50m,
                    ProcentPromena = -3m,
                    KolicinaIstrguvaniAkcii = 200
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetDailyTurnoverForTotalMarketCap(latestDate))
                .Returns(dailyData);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDateWithSecurity(latestDate))
                .Returns(dailyData);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDateRange(
                    new DateTime(2026, 10, 1), latestDate))
                .Returns(monthlyData);

            _securitiesRepositoryMock
                .Setup(r => r.GetTotalNumSecurities())
                .Returns(15);

            var result = _service.GetOverview();

            Assert.AreEqual(300000m, result.TotalMarketCap);
            Assert.AreEqual(150, result.AverageDailyVolume);
            Assert.AreEqual(150, result.AverageMonthlyVolume);
            Assert.AreEqual("ALK", result.TopGainer);
            Assert.AreEqual(5m, result.TopGainerChange);
            Assert.AreEqual("KMB", result.TopLoser);
            Assert.AreEqual(-3m, result.TopLoserChange);
            Assert.AreEqual(15, result.TotalSecurities);

            _dailyTurnoverRepositoryMock.Verify(
                r => r.GetLatestDate(),
                Times.Once);

            _securitiesRepositoryMock.Verify(
                r => r.GetTotalNumSecurities(),
                Times.Once);
        }

        [TestMethod]
        public void GetOverview_WhenNoGainersOrLosers_ReturnsEmptyCodes()
        {
            var latestDate = new DateTime(2026, 10, 8);

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = security,
                    Datum = latestDate,
                    CenaPoslednaTransakcija = 100m,
                    ProcentPromena = 0m,
                    KolicinaIstrguvaniAkcii = 100
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetDailyTurnoverForTotalMarketCap(latestDate))
                .Returns(dailyData);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDateWithSecurity(latestDate))
                .Returns(dailyData);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDateRange(
                    new DateTime(2026, 10, 1), latestDate))
                .Returns(dailyData);

            _securitiesRepositoryMock
                .Setup(r => r.GetTotalNumSecurities())
                .Returns(1);

            var result = _service.GetOverview();

            Assert.AreEqual("", result.TopGainer);
            Assert.AreEqual(0m, result.TopGainerChange);
            Assert.AreEqual("", result.TopLoser);
            Assert.AreEqual(0m, result.TopLoserChange);
        }

        [TestMethod]
        public void GetTopGainers_ReturnsPositiveChangesInDecendingOrder()
        {
            var date = new DateTime(2026, 10, 8);

            var securities = new List<HartiiOdVrednost>
            {
                new HartiiOdVrednost
                {
                    Id = 1,
                    Kod = "ALK"
                },
                new HartiiOdVrednost
                {
                    Id = 2,
                    Kod = "KMB"
                },
                new HartiiOdVrednost
                {
                    Id = 3,
                    Kod = "TEL"
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(date);

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = securities[0],
                    Datum = date,
                    ProcentPromena = 3m,
                    KolicinaIstrguvaniAkcii = 100
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Hv = securities[1],
                    Datum = date,
                    ProcentPromena = 8m,
                    KolicinaIstrguvaniAkcii = 200
                },
                new DnevenPromet
                {
                    Hvid = 3,
                    Hv = securities[2],
                    Datum = date,
                    ProcentPromena = -2m,
                    KolicinaIstrguvaniAkcii = 300
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDateWithSecurity(date))
                .Returns(dailyData);

            var result = _service.GetTopGainers(2).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("KMB", result[0].SecurityCode);
            Assert.AreEqual(8m, result[0].ChangePercent);
            Assert.AreEqual(200, result[0].Volume);
            Assert.AreEqual("ALK", result[1].SecurityCode);
            Assert.AreEqual(3m, result[1].ChangePercent);
        }

        [TestMethod]
        public void GetTopGainers_ExcludesRowsWithNullVolume()
        {
            var date = new DateTime(2026, 10, 8);

            var securities = new List<HartiiOdVrednost>
            {
                new HartiiOdVrednost
                {
                    Id = 1,
                    Kod = "ALK"
                },
                new HartiiOdVrednost
                {
                    Id = 2,
                    Kod = "KMB"
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(date);

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = securities[0],
                    Datum = date,
                    ProcentPromena = 10m,
                    KolicinaIstrguvaniAkcii = null
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Hv = securities[1],
                    Datum = date,
                    ProcentPromena = 5m,
                    KolicinaIstrguvaniAkcii = 100
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDateWithSecurity(date))
                .Returns(dailyData);

            var result = _service.GetTopGainers(5).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("KMB", result[0].SecurityCode);
        }

        [TestMethod]
        public void GetTopLosers_ReturnsNegativeChangesInAscendingOrder()
        {
            var date = new DateTime(2026, 10, 8);

            var securities = new List<HartiiOdVrednost>
            {
                new HartiiOdVrednost
                {
                    Id = 1,
                    Kod = "ALK"
                },
                new HartiiOdVrednost
                {
                    Id = 2,
                    Kod = "KMB"
                },
                new HartiiOdVrednost
                {
                    Id = 3,
                    Kod = "TEL"
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(date);

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = securities[0],
                    Datum = date,
                    ProcentPromena = -2m,
                    KolicinaIstrguvaniAkcii = 100
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Hv = securities[1],
                    Datum = date,
                    ProcentPromena = -8m,
                    KolicinaIstrguvaniAkcii = 200
                },
                new DnevenPromet
                {
                    Hvid = 3,
                    Hv = securities[2],
                    Datum = date,
                    ProcentPromena = 3m,
                    KolicinaIstrguvaniAkcii = 300
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDateWithSecurity(date))
                .Returns(dailyData);

            var result = _service.GetTopLosers(2).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("KMB", result[0].SecurityCode);
            Assert.AreEqual(-8m, result[0].ChangePercent);
            Assert.AreEqual("ALK", result[1].SecurityCode);
            Assert.AreEqual(-2m, result[1].ChangePercent);
        }

        [TestMethod]
        public void GetMostTrade_ReturnsRowsInDescendingVolumeOrder()
        {
            var date = new DateTime(2026, 10, 8);

            var securities = new List<HartiiOdVrednost>
            {
                new HartiiOdVrednost
                {
                    Id = 1,
                    Kod = "ALK"
                },
                new HartiiOdVrednost
                {
                    Id = 2,
                    Kod = "KMB"
                },
                new HartiiOdVrednost
                {
                    Id = 3,
                    Kod = "TEL"
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(date);

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = securities[0],
                    Datum = date,
                    ProcentPromena = 2m,
                    KolicinaIstrguvaniAkcii = 100
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Hv = securities[1],
                    Datum = date,
                    ProcentPromena = -1m,
                    KolicinaIstrguvaniAkcii = 500
                },
                new DnevenPromet
                {
                    Hvid = 3,
                    Hv = securities[2],
                    Datum = date,
                    ProcentPromena = 4m,
                    KolicinaIstrguvaniAkcii = 250
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = securities[0],
                    Datum = date,
                    ProcentPromena = 0m,
                    KolicinaIstrguvaniAkcii = null
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDateWithSecurity(date))
                .Returns(dailyData);

            var result = _service.GetMostTraded(2).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("KMB", result[0].SecurityCode);
            Assert.AreEqual(500, result[0].Volume);
            Assert.AreEqual("TEL", result[1].SecurityCode);
            Assert.AreEqual(250, result[1].Volume);
        }

        [TestMethod]
        public void GetTopGainers_WhenNoMatchingRows_ReturnsEmptyCollection()
        {
            var date = new DateTime(2026, 10, 8);

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(date);

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = security,
                    Datum = date,
                    ProcentPromena = -2m,
                    KolicinaIstrguvaniAkcii = 100
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDateWithSecurity(date))
                .Returns(dailyData);

            var result = _service.GetTopGainers(5).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FindLiquidity_WhenOnlyOwnedIsTrue_UsesOwnedSecurityIds()
        {
            var ownedIds = new List<int> { 1, 2 };
            var fromDate = DateTime.Today.AddMonths(-3);

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(1))
                .Returns(ownedIds);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLiquidity(
                    It.Is<IEnumerable<int>?>(
                        ids => ids != null && ids.SequenceEqual(ownedIds)),
                    It.IsAny<DateTime>()))
                .Returns(new List<DnevenPromet>());

            var result = _service.FindLiquidity(1, 3, true);

            Assert.IsNotNull(result);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedSecuritiesIds(1),
                Times.Once);

            _dailyTurnoverRepositoryMock.Verify(
                r => r.GetLiquidity(
                    It.Is<IEnumerable<int>?>(
                        ids => ids != null && ids.SequenceEqual(ownedIds)),
                    It.Is<DateTime>(d => d.Date == fromDate.Date)),
                Times.Once);
        }

        [TestMethod]
        public void FindLiquidity_WhenOnlyOwnedIsFalse_PassesNullSecurityIds()
        {
            _dailyTurnoverRepositoryMock
               .Setup(r => r.GetLiquidity(
                   It.Is<IEnumerable<int>?>(
                       ids => ids == null),
                   It.IsAny<DateTime>()))
               .Returns(new List<DnevenPromet>());

            var result = _service.FindLiquidity(1, 6, false);

            Assert.IsNotNull(result);

            _transactionsRepositoryMock.Verify(
               r => r.GetOwnedSecuritiesIds(It.IsAny<int>()),
               Times.Never);

            _dailyTurnoverRepositoryMock.Verify(
                r => r.GetLiquidity(
                    It.Is<IEnumerable<int>?>(
                        ids => ids == null),
                    It.IsAny<DateTime>()),
                Times.Once);
        }

        [TestMethod]
        public void FindLiquidity_GroupsSecurityDataAndCalculatesLiquidity()
        {
            var today = DateTime.Today;

            var securities = new List<HartiiOdVrednost>
            {
                new HartiiOdVrednost
                {
                    Id = 1,
                    Kod = "ALK"
                },
                new HartiiOdVrednost
                {
                    Id = 2,
                    Kod = "KMB"
                }
            };

            var turnover = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = securities[0],
                    Datum = today.AddDays(-2),
                    KolicinaIstrguvaniAkcii = 100
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = securities[0],
                    Datum = today.AddDays(-1),
                    KolicinaIstrguvaniAkcii = 200
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Hv = securities[1],
                    Datum = today.AddDays(-1),
                    KolicinaIstrguvaniAkcii = 600
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLiquidity(
                    It.Is<IEnumerable<int>?>(ids => ids == null),
                    It.IsAny<DateTime>()))
                .Returns(turnover);

            var result = _service.FindLiquidity(1, 3, false);

            var mostTraded = result.MostByTradedQuantity.ToList();
            Assert.AreEqual(2, mostTraded.Count);
            Assert.AreEqual("KMB", mostTraded[0].SecurityCode);
            Assert.AreEqual(600, mostTraded[0].TradedQuantity);

            var alkLiquidity = mostTraded.Single(x => x.SecurityCode == "ALK");
            Assert.AreEqual(2, alkLiquidity.TradingDays);
            Assert.AreEqual(300, alkLiquidity.TradedQuantity);
            Assert.AreEqual(150m, alkLiquidity.AverageDailyVolume);
            Assert.AreEqual(today.AddDays(-1), alkLiquidity.LastTradeDate);

        }
    }
}