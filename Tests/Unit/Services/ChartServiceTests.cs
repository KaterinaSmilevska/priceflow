using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class ChartServiceTests
    {
        private Mock<IDailyTurnoverRepository> _dailyTurnoverRepositoryMock = null!;
        private Mock<IPortfoliosRepository> _portfoliosRepositoryMock = null!;
        private Mock<ITransactionsRepository> _transactionsRepositoryMock = null!;
        private Mock<ISecuritiesRepository> _securitiesRepositoryMock = null!;
        private ChartService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _dailyTurnoverRepositoryMock = new Mock<IDailyTurnoverRepository>();
            _portfoliosRepositoryMock = new Mock<IPortfoliosRepository>();
            _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
            _securitiesRepositoryMock = new Mock<ISecuritiesRepository>();
            _service = new ChartService(_dailyTurnoverRepositoryMock.Object, _transactionsRepositoryMock.Object,
                _securitiesRepositoryMock.Object, _portfoliosRepositoryMock.Object);
        }

        [TestMethod]
        public void GetPriceTrend_WithValidSecurityAndDates_ReturnsMappedPrices()
        {
            var startDate = new DateTime(2026, 10, 1);
            var endDate = new DateTime(2026, 10, 3);

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(security);

            var turnover = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = security,
                    Datum = startDate,
                    CenaPoslednaTransakcija = 100m
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = security,
                    Datum = startDate.AddDays(1),
                    CenaPoslednaTransakcija = 105.5m
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetBySecurityAndDateRange(1, startDate, endDate))
                .Returns(turnover);

            var result = _service.GetPriceTrend(1, startDate, endDate).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(startDate, result[0].Date);
            Assert.AreEqual(100m, result[0].Price);
            Assert.AreEqual(startDate.AddDays(1), result[1].Date);
            Assert.AreEqual(105.5m, result[1].Price);

            _dailyTurnoverRepositoryMock.Verify(
                r => r.GetBySecurityAndDateRange(1, startDate, endDate),
                Times.Once);
        }

        [TestMethod]
        public void GetPriceTrend_WhenStartDateIsAfterEndDate_ThrowsValidationException()
        {
            var startDate = new DateTime(2026, 10, 10);
            var endDate = new DateTime(2026, 10, 1);

            var exception = Assert.ThrowsException<ValidationException>(
                () => _service.GetPriceTrend(100, startDate, endDate));

            Assert.AreEqual("INVALID_DATE_RANGE", exception.Code);

            _securitiesRepositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public void GetPriceTrend_WhenSecurityDoesNotExist_ThrowsNotFoundException()
        {
            var startDate = new DateTime(2026, 10, 1);
            var endDate = new DateTime(2026, 10, 10);

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.GetPriceTrend(100, startDate, endDate));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);

            _dailyTurnoverRepositoryMock.Verify(
                r => r.GetBySecurityAndDateRange(
                    It.IsAny<int>(),
                    It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()),
                Times.Never);
        }

        [TestMethod]
        public void GetPriceTrend_WhenNoPricesExist_ReturnsEmptyCollection()
        {
            var startDate = new DateTime(2026, 10, 1);
            var endDate = new DateTime(2026, 10, 10);

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(security);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetBySecurityAndDateRange(1, startDate, endDate))
                .Returns(new List<DnevenPromet>());

            var result = _service.GetPriceTrend(1, startDate, endDate).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetSectorDistribution_GroupsBySectorAndCalculatesMarketCap()
        {
            var date = new DateTime(2026, 10, 8);

            var techSector = new Sektori
            {
                Id = 1,
                Ime = "Technology"
            };

            var financeSector = new Sektori
            {
                Id = 2,
                Ime = "Finance"
            };

            var techIssuer = new Izdavachi
            {
                Id = 1,
                Sektor = techSector
            };

            var financeIssuer = new Izdavachi
            {
                Id = 2,
                Sektor = financeSector
            };

            var alk = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                VkupenBrojAkcii = 1000,
                Izdavach = techIssuer
            };

            var kmb = new HartiiOdVrednost
            {
                Id = 2,
                Kod = "KMB",
                VkupenBrojAkcii = 500,
                Izdavach = financeIssuer
            };

            var tel = new HartiiOdVrednost
            {
                Id = 3,
                Kod = "TEL",
                VkupenBrojAkcii = 2000,
                Izdavach = techIssuer
            };

            var turnover = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = alk,
                    Datum = date,
                    CenaPoslednaTransakcija = 100m
                },
                new DnevenPromet
                {
                    Hvid= 2,
                    Hv = kmb,
                    Datum = date,
                    CenaPoslednaTransakcija = 200m
                },
                new DnevenPromet
                {
                    Hvid = 3,
                    Hv = tel,
                    Datum = date,
                    CenaPoslednaTransakcija = 50m
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDateWithSecurity(date))
                .Returns(turnover);

            var result = _service.GetSectorDistribution(date).ToList();

            Assert.AreEqual(2, result.Count);

            var technology = result.Single(x => x.SectorName == "Technology");
            Assert.AreEqual(200000m, technology.MarketCap);

            var finance = result.Single(x => x.SectorName == "Finance");
            Assert.AreEqual(100000m, finance.MarketCap);
        }

        [TestMethod]
        public void GetSectorDistribution_WhenPriceIsNull_TreatsPriceAsZero()
        {
            var date = new DateTime(2026, 10, 8);

            var sector = new Sektori
            {
                Id = 1,
                Ime = "Technology"
            };

            var issuer = new Izdavachi
            {
                Id = 1,
                Sektor = sector
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                Izdavach = issuer,
                VkupenBrojAkcii = 1000
            };

            var turnover = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Hv = security,
                    Datum = date,
                    CenaPoslednaTransakcija = null
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDateWithSecurity(date))
                .Returns(turnover);

            var result = _service.GetSectorDistribution(date).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(0m, result[0].MarketCap);
        }

        [TestMethod]
        public void GetMonthlyIncome_FiltersByTransactionTypeAndPortfolioType()
        {
            var portfolio = new Portfolija
            {
                Id = 1
            };

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

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(portfolio);

            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Id = 1,
                    Hv = securities[0],
                    Datum = new DateOnly(2026, 1, 5),
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 5,
                    Iznos = 500m,
                    Realna = true
                },
                new Transakcii
                {
                    Id = 2,
                    Hv = securities[1],
                    Datum = new DateOnly(2026, 1, 20),
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 2,
                    Iznos = 300m,
                    Realna = true
                },
                new Transakcii
                {
                    Id = 3,
                    Hv = securities[0],
                    Datum = new DateOnly(2026, 2, 5),
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 1,
                    Iznos = 200m,
                    Realna = true
                },
                new Transakcii
                {
                    Id = 4,
                    Hv = securities[2],
                    Datum = new DateOnly(2026, 1, 10),
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 1,
                    Iznos = 100m,
                    Realna = false
                },
                new Transakcii
                {
                    Id = 5,
                    Hv = securities[0],
                    Datum = new DateOnly(2026, 3, 1),
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 1,
                    Iznos = 100m,
                    Realna = true
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1))
                .Returns(transactions);

            var result = _service.GetMonthlyIncome(1, true).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(2026, result[0].Year);
            Assert.AreEqual(1, result[0].Month);
            Assert.AreEqual(800m, result[0].Income);

            Assert.AreEqual(2026, result[1].Year);
            Assert.AreEqual(2, result[1].Month);
            Assert.AreEqual(200m, result[1].Income);
        }

        [TestMethod]
        public void GetMonthlyIncome_WhenPortfolioDoesNotExist_ThrowsNotFoundException()
        {
            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((Portfolija?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.GetMonthlyIncome(1000, true));

            Assert.AreEqual("PORTFOLIO_NOT_FOUND", exception.Code);
        }

        [TestMethod]
        public void GetMonthlyIncome_WhenNoMatchingTransactions_ReturnsEmptyCollection()
        {
            var portfolio = new Portfolija
            {
                Id = 1
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(portfolio);

            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Id = 1,
                    Hv = security,
                    Datum = new DateOnly(2026, 1, 5),
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 5,
                    Iznos = 500m,
                    Realna = true
                }
            };


            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1))
                .Returns(transactions);

            var result = _service.GetMonthlyIncome(1, true).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetAllocation_SumsBuysAndSellsAndExcludesZeroQuantity()
        {
            var portfolio = new Portfolija
            {
                Id = 1
            };

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

            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Id = 1,
                    Hv = securities[0],
                    Datum = new DateOnly(2026, 1, 1),
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 10,
                    Iznos = 1000m,
                    Realna = true
                },
                new Transakcii
                {
                    Id = 2,
                    Hv = securities[0],
                    Datum = new DateOnly(2026, 1, 2),
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 3,
                    Iznos = 300m,
                    Realna = true
                },
                new Transakcii
                {
                    Id = 3,
                    Hv = securities[1],
                    Datum = new DateOnly(2026, 1, 3),
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 5,
                    Iznos = 500m,
                    Realna = true
                },
                new Transakcii
                {
                    Id = 4,
                    Hv = securities[1],
                    Datum = new DateOnly(2026, 1, 4),
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 5,
                    Iznos = 600m,
                    Realna = true
                },
                new Transakcii
                {
                    Id = 5,
                    Hv = securities[2],
                    Datum = new DateOnly(2026, 1, 5),
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 4,
                    Iznos = 400m,
                    Realna = false
                }
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(portfolio);

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1))
                .Returns(transactions);

            var result = _service.GetAllocation(1, true).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(7, result[0].Quantity);
        }

        [TestMethod]
        public void GetAllocation_WhenPortfolioDoesNotExist_ThrowsNotFoundException()
        {
            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((Portfolija?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.GetAllocation(1000, true));

            Assert.AreEqual("PORTFOLIO_NOT_FOUND", exception.Code);
        }

        [TestMethod]
        public void GetSecurities_ReturnsMappedSecurities()
        {
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

            _securitiesRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(securities);

            var result = _service.GetSecurities().ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("ALK", result[0].Code);
            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("KMB", result[1].Code);
        }

        [TestMethod]
        public void GetSecurities_WhenRepositoryReturnsEmptyCollection_ReturnsEmptyCollection()
        {
            _securitiesRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<HartiiOdVrednost>());

            var result = _service.GetSecurities().ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FindLatestDate_ReturnsRepositoryDate()
        {
            var expectedDate = new DateTime(2026, 10, 8);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(expectedDate);

            var result = _service.FindLatestDate();

            Assert.AreEqual(expectedDate, result);

            _dailyTurnoverRepositoryMock.Verify(
                r => r.GetLatestDate(),
                Times.Once);
        }
    }
}
