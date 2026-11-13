using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class SecurityFilterServiceTests
    {
        private Mock<ISecurityFilterRepository> _repositoryMock = null!;
        private SecurityFilterService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<ISecurityFilterRepository>();
            _service = new SecurityFilterService(_repositoryMock.Object);
        }

        [TestMethod]
        public void FindMostProfitableSecuritiesByDividendYield_ReturnsPositiveLatestYearValues()
        {
            int latestYear = 2025;

            var financialIndicators = new List<FinansiskiPokazateli>
            {
                new FinansiskiPokazateli
                {
                    Godina = latestYear,
                    DividendenPrinos = 5.5m
                },
                new FinansiskiPokazateli
                {
                    Godina = 2025,
                    DividendenPrinos = 0m
                }
               
            };

            var issuers = new List<Izdavachi>
            {
                new Izdavachi
                {
                    Id = 1,
                    Ime = "Alkaloid AD Skopje",
                    FinansiskiPokazateli = { financialIndicators[0] }
                },
                new Izdavachi
                {
                    Id = 2,
                    Ime = "Komercijalna Banka AD Skopje",
                    FinansiskiPokazateli = { financialIndicators[1] }
                }
     
            };

            var securities = new List<HartiiOdVrednost>
            {
                new HartiiOdVrednost
                {
                    Id = 1,
                    Kod = "ALK",
                    Izdavach = issuers[0]
                },
                new HartiiOdVrednost
                {
                    Id = 2,
                    Kod = "KMB",
                    Izdavach = issuers[1]
                }
            };

            _repositoryMock
                .Setup(r => r.GetMostProfitableSecuritiesByDividendYield())
                .Returns(securities);

            _repositoryMock
                .Setup(r => r.GetLatestYear())
                .Returns(latestYear);

            var result = _service.FindMostProfitableSecuritiesByDividendYield().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(5.5m, result[0].Value);

            _repositoryMock.Verify(
                r => r.GetMostProfitableSecuritiesByDividendYield(),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetLatestYear(),
                Times.Once);
        }

        [TestMethod]
        public void FindMostProfitableSecuritiesByDividendPerShare_ReturnsPositiveLatestYearValues()
        {
            int latestYear = 2025;

            var financialIndicators = new List<FinansiskiPokazateli>
            {
                new FinansiskiPokazateli
                {
                    Godina = latestYear,
                    DividendaPoAkcija = 12.5m
                },
                new FinansiskiPokazateli
                {
                    Godina = 2025,
                    DividendaPoAkcija = 0m
                }

            };

            var issuers = new List<Izdavachi>
            {
                new Izdavachi
                {
                    Id = 1,
                    Ime = "Alkaloid AD Skopje",
                    FinansiskiPokazateli = { financialIndicators[0] }
                },
                new Izdavachi
                {
                    Id = 2,
                    Ime = "Komercijalna Banka AD Skopje",
                    FinansiskiPokazateli = { financialIndicators[1] }
                }

            };

            var securities = new List<HartiiOdVrednost>
            {
                new HartiiOdVrednost
                {
                    Id = 1,
                    Kod = "ALK",
                    Izdavach = issuers[0]
                },
                new HartiiOdVrednost
                {
                    Id = 2,
                    Kod = "KMB",
                    Izdavach = issuers[1]
                }
            };

            _repositoryMock
                .Setup(r => r.GetMostProfitableSecuritiesByDividendPerShare())
                .Returns(securities);

            _repositoryMock
                .Setup(r => r.GetLatestYear())
                .Returns(latestYear);

            var result = _service.FindMostProfitableSecuritiesByDividendPerShare().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(12.5m, result[0].Value);

            _repositoryMock.Verify(
                r => r.GetMostProfitableSecuritiesByDividendPerShare(),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetLatestYear(),
                Times.Once);
        }

        [TestMethod]
        public void FindSecuritiesWithBiggestPriceOscillations_ReturnsCorrectOscillation()
        {
            DateTime latestDate = new DateTime(2025, 12, 31);

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Datum = latestDate,
                    MaxCena = 150m,
                    MinCena = 120m
                }
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                DnevenPromet = dailyData
            };

            _repositoryMock
                .Setup(r => r.GetSecuritiesWithBiggestPriceOscillations())
                .Returns(new List<HartiiOdVrednost> { security });

            _repositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            var result = _service.FindSecuritiesWithBiggestPriceOscillations().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(30m, result[0].Value);

            _repositoryMock.Verify(
                r => r.GetSecuritiesWithBiggestPriceOscillations(),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetLatestDate(),
                Times.Once);
        }

        [TestMethod]
        public void FindSecuritiesWithSmallestPriceOscillations_ReturnsCorrectOscillation()
        {
            DateTime latestDate = new DateTime(2025, 12, 31);

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Datum = latestDate,
                    MaxCena = 120.123456m,
                    MinCena = 110m
                }
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                DnevenPromet = dailyData
            };

            _repositoryMock
                .Setup(r => r.GetSecuritiesWithSmallestPriceOscillations())
                .Returns(new List<HartiiOdVrednost> { security });

            _repositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            var result = _service.FindSecuritiesWithSmallestPriceOscillations().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(10.1235m, result[0].Value);

            _repositoryMock.Verify(
                r => r.GetSecuritiesWithSmallestPriceOscillations(),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetLatestDate(),
                Times.Once);
        }

        [TestMethod]
        public void FindLeastLiquidSecuritiesByTradedQuantity_ReturnsPositiveLatestDateQuantity()
        {
            DateTime latestDate = new DateTime(2025, 12, 31);

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Datum = latestDate,
                    KolicinaIstrguvaniAkcii = 250
                }
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                DnevenPromet = dailyData
            };

            _repositoryMock
                .Setup(r => r.GetLeastLiquidSecuritiesByTradedQuantity())
                .Returns(new List<HartiiOdVrednost> { security });

            _repositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            var result = _service.FindLeastLiquidSecuritiesByTradedQuantity().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(250m, result[0].Value);

            _repositoryMock.Verify(
                r => r.GetLeastLiquidSecuritiesByTradedQuantity(),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetLatestDate(),
                Times.Once);
        }

        [TestMethod]
        public void FindMostLiquidSecuritiesByTradedQuantity_ReturnsPositiveLatestDateQuantity()
        {
            DateTime latestDate = new DateTime(2025, 12, 31);

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Datum = latestDate,
                    KolicinaIstrguvaniAkcii = 1500
                }
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                DnevenPromet = dailyData
            };

            _repositoryMock
                .Setup(r => r.GetMostLiquidSecuritiesByTradedQuantity())
                .Returns(new List<HartiiOdVrednost> { security });

            _repositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            var result = _service.FindMostLiquidSecuritiesByTradedQuantity().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(1500m, result[0].Value);

            _repositoryMock.Verify(
                r => r.GetMostLiquidSecuritiesByTradedQuantity(),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetLatestDate(),
                Times.Once);
        }

        [TestMethod]
        public void FindLeastLiquidSecuritiesByNumTradingDays_CountsPositiveTradingDays()
        {
            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    KolicinaIstrguvaniAkcii = 100
                },
                new DnevenPromet
                {
                    KolicinaIstrguvaniAkcii = 0
                },
                new DnevenPromet
                {
                    KolicinaIstrguvaniAkcii = 250
                },
                new DnevenPromet
                {
                    KolicinaIstrguvaniAkcii = 0
                }
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                DnevenPromet = dailyData
            };

            _repositoryMock
                .Setup(r => r.GetLeastLiquidSecuritiesByNumTradingDays())
                .Returns(new List<HartiiOdVrednost> { security });

            var result = _service.FindLeastLiquidSecuritiesByNumTradingDays().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(2m, result[0].Value);

            _repositoryMock.Verify(
                r => r.GetLeastLiquidSecuritiesByNumTradingDays(),
                Times.Once);
        }

        [TestMethod]
        public void FindMostLiquidSecuritiesByNumTradingDays_CountsPositiveTradingDays()
        {
            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    KolicinaIstrguvaniAkcii = 100
                },
                new DnevenPromet
                {
                    KolicinaIstrguvaniAkcii = 200
                },
                new DnevenPromet
                {
                    KolicinaIstrguvaniAkcii = 300
                },
                new DnevenPromet
                {
                    KolicinaIstrguvaniAkcii = 0
                }
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                DnevenPromet = dailyData
            };

            _repositoryMock
                .Setup(r => r.GetMostLiquidSecuritiesByNumTradingDays())
                .Returns(new List<HartiiOdVrednost> { security });

            var result = _service.FindMostLiquidSecuritiesByNumTradingDays().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(3m, result[0].Value);

            _repositoryMock.Verify(
                r => r.GetMostLiquidSecuritiesByNumTradingDays(),
                Times.Once);
        }

        [TestMethod]
        public void FindMostProfitableSectorsByDividendYield_SumsLatestYearDividendYield()
        {
            int latestYear = 2025;

            var financialIndicators = new List<FinansiskiPokazateli>
            {
                new FinansiskiPokazateli
                {
                    Godina = latestYear,
                    DividendenPrinos = 4.5m
                },
                new FinansiskiPokazateli
                {
                    Godina = latestYear,
                    DividendenPrinos = 2.5m
                }
            };

            var issuers = new List<Izdavachi>
            {
                new Izdavachi
                {
                    Id = 1,
                    Ime = "Alkaloid AD Skopje",
                    FinansiskiPokazateli = financialIndicators
                }
            };

            var sector = new Sektori
            {
                Id = 1,
                Ime = "Banking",
                Izdavachi = issuers
            };

            _repositoryMock
                .Setup(r => r.GetMostProfitableSectorsByDividendYield())
                .Returns(new List<Sektori> { sector });

            _repositoryMock
                .Setup(r => r.GetLatestYear())
                .Returns(latestYear);

            var result = _service.FindMostProfitableSectorsByDividendYield().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Banking", result[0].Name);
            Assert.AreEqual(7.0m, result[0].TotalValue);

            _repositoryMock.Verify(
                r => r.GetMostProfitableSectorsByDividendYield(),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetLatestYear(),
                Times.Once);
        }

        [TestMethod]
        public void FindMostProfitableSectorsByProfit_SumsPositiveLatestYearProfit()
        {
            int latestYear = 2025;

            var financialIndicators = new List<FinansiskiPokazateli>
            {
                new FinansiskiPokazateli
                {
                    Godina = latestYear,
                    OperativnaDobivka = 100000m
                },
                new FinansiskiPokazateli
                {
                    Godina = latestYear,
                    OperativnaDobivka = 50000m
                },
                new FinansiskiPokazateli
                {
                    Godina = latestYear,
                    OperativnaDobivka = -20000m
                }
            };

            var issuers = new List<Izdavachi>
            {
                new Izdavachi
                {
                    Id = 1,
                    Ime = "Alkaloid AD Skopje",
                    FinansiskiPokazateli = financialIndicators
                }
            };

            var sector = new Sektori
            {
                Id = 1,
                Ime = "Banking",
                Izdavachi = issuers
            };

            _repositoryMock
                .Setup(r => r.GetMostProfitableSectorsByProfit())
                .Returns(new List<Sektori> { sector });

            _repositoryMock
                .Setup(r => r.GetLatestYear())
                .Returns(latestYear);

            var result = _service.FindMostProfitableSectorsByProfit().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Banking", result[0].Name);
            Assert.AreEqual(150000m, result[0].TotalValue);

            _repositoryMock.Verify(
                r => r.GetMostProfitableSectorsByProfit(),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetLatestYear(),
                Times.Once);
        }

        [TestMethod]
        public void FindSecuritiesValuation_ReturnsPriceToBookValueRatio()
        {
            int latestYear = 2025;
            DateTime latestDate = new DateTime(2025, 12, 31);

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Datum = latestDate,
                    CenaPoslednaTransakcija = 150m
                }
            };

            var financialIndicators = new List<FinansiskiPokazateli>
            {
                new FinansiskiPokazateli
                {
                    Godina = latestYear,
                    KnigovodstvenaVrednostPoAkcija = 30m
                }
            };

            var issuer = new Izdavachi
            {
                Id = 1,
                Ime = "Alkaloid AD Skopje",
                FinansiskiPokazateli = financialIndicators
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                DnevenPromet = dailyData,
                Izdavach = issuer
            };

            _repositoryMock
                .Setup(r => r.GetSecuritiesValuation())
                .Returns(new List<HartiiOdVrednost> { security });

            _repositoryMock
                .Setup(r => r.GetLatestYear())
                .Returns(latestYear);

            _repositoryMock
               .Setup(r => r.GetLatestDate())
               .Returns(latestDate);

            var result = _service.FindSecuritiesValuation().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(5m, result[0].Value);

            _repositoryMock.Verify(
                r => r.GetSecuritiesValuation(),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetLatestYear(),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetLatestDate(),
                Times.Once);
        }
    }
}
