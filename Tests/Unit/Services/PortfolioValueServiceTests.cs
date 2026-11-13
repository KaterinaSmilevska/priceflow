using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class PortfolioValueServiceTests
    {
        private Mock<ITransactionsRepository> _transactionsRepositoryMock = null!;
        private Mock<IDailyTurnoverRepository> _dailyTurnoverRepositoryMock = null!;
        private PortfolioValueService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
            _dailyTurnoverRepositoryMock = new Mock<IDailyTurnoverRepository>();
            _service = new PortfolioValueService(_transactionsRepositoryMock.Object, _dailyTurnoverRepositoryMock.Object);
        }

        [TestMethod]
        public void GetCurrentValue_ValidHoldings_ReturnsPortfolioValues()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Hvid = 1,
                    Hv = security,
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 100
                },
                 new Transakcii
                {
                    Hvid = 1,
                    Hv = security,
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 20
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1, true))
                .Returns(transactions);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestPrices(It.IsAny<IEnumerable<int>>()))
                .Returns(new List<DnevenPromet>
                {
                    new DnevenPromet
                    {
                        Hvid = 1,
                        CenaPoslednaTransakcija = 150m
                    }
                });

            var result = _service.GetCurrentValue(1, true).ToList();

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(80, result[0].TotalQuantity);
            Assert.AreEqual(150m, result[0].LastPrice);
            Assert.AreEqual(12000m, result[0].CurrentValue);
            Assert.IsTrue(result[0].IsReal);
        }

        [TestMethod]
        public void GetCurrentValue_SimulatedTransactions_ReturnsIsRealFalse()
        {
            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Hvid = 1,
                    Hv = new HartiiOdVrednost { Id = 1, Kod = "KMB" },
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 50
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(5, false))
                .Returns(transactions);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestPrices(It.IsAny<IEnumerable<int>>()))
                .Returns(new List<DnevenPromet>
                {
                    new DnevenPromet
                    {
                        Hvid = 1,
                        CenaPoslednaTransakcija = 200m
                    }
                });

            var result = _service.GetCurrentValue(5, false).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.IsFalse(result[0].IsReal);
            Assert.AreEqual(50, result[0].TotalQuantity);
            Assert.AreEqual(10000m, result[0].CurrentValue);

            _transactionsRepositoryMock.Verify(
                r => r.GetByPortfolioId(5, false),
                Times.Once);
        }

        [TestMethod]
        public void GetCurrentValue_NonPositiveHoldings_ExcludesThem()
        {
            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Hvid = 1,
                    Hv = new HartiiOdVrednost { Id = 1, Kod = "ALK" },
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 100
                },
                new Transakcii
                {
                    Hvid = 1,
                    Hv = new HartiiOdVrednost { Id = 1, Kod = "ALK" },
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 100
                },
                new Transakcii
                {
                    Hvid = 2,
                    Hv = new HartiiOdVrednost { Id = 1, Kod = "KMB" },
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 50
                },
                new Transakcii
                {
                    Hvid = 2,
                    Hv = new HartiiOdVrednost { Id = 1, Kod = "KMB" },
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 70
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1, true))
                .Returns(transactions);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestPrices(It.IsAny<IEnumerable<int>>()))
                .Returns(new List<DnevenPromet>());

            var result = _service.GetCurrentValue(1, true).ToList();

            Assert.AreEqual(0, result.Count);

            _dailyTurnoverRepositoryMock.Verify(
                r => r.GetLatestPrices(It.IsAny<IEnumerable<int>>()),
                Times.Once);
        }

        [TestMethod]
        public void GetCurrentValue_SecurityWithoutLatestPrice_ExcludesSecurity()
        {
            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Hvid = 1,
                    Hv = new HartiiOdVrednost { Id = 1, Kod = "ALK" },
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 100
                },
                new Transakcii
                {
                    Hvid = 2,
                    Hv = new HartiiOdVrednost { Id = 1, Kod = "KMB" },
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 50
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1, true))
                .Returns(transactions);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestPrices(It.IsAny<IEnumerable<int>>()))
                .Returns(new List<DnevenPromet>
                {
                    new DnevenPromet
                    {
                        Hvid = 1,
                        CenaPoslednaTransakcija = 100m
                    }
                });

            var result = _service.GetCurrentValue(1, true).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);

            _transactionsRepositoryMock.Verify(
                r => r.GetByPortfolioId(1, true),
                Times.Once);
        }

        [TestMethod]
        public void GetCurrentValue_MultipleSecurities_ReturnsOrderedByCurrentValueDescending()
        {
            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Hvid = 1,
                    Hv = new HartiiOdVrednost { Id = 1, Kod = "ALK" },
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 10
                },
                new Transakcii
                {
                    Hvid = 2,
                    Hv = new HartiiOdVrednost { Id = 1, Kod = "KMB" },
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 100
                }
            };

            _transactionsRepositoryMock
              .Setup(r => r.GetByPortfolioId(1, true))
              .Returns(transactions);

            _dailyTurnoverRepositoryMock
               .Setup(r => r.GetLatestPrices(It.IsAny<IEnumerable<int>>()))
               .Returns(new List<DnevenPromet>
               {
                    new DnevenPromet
                    {
                        Hvid = 1,
                        CenaPoslednaTransakcija = 100m
                    },
                    new DnevenPromet
                    {
                        Hvid = 2,
                        CenaPoslednaTransakcija = 20m
                    }
               });

            var result = _service.GetCurrentValue(1, true).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual("KMB", result[0].SecurityCode);
            Assert.AreEqual(2000m, result[0].CurrentValue);

            Assert.AreEqual("ALK", result[1].SecurityCode);
            Assert.AreEqual(1000m, result[1].CurrentValue);

            _transactionsRepositoryMock.Verify(
                t => t.GetByPortfolioId(1, true),
                Times.Once);
        }

        [TestMethod]
        public void GetCurrentValue_NoTransactions_ReturnsEmpty()
        {
            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1, true))
                .Returns(new List<Transakcii>());

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestPrices(It.IsAny<IEnumerable<int>>()))
                .Returns(new List<DnevenPromet>());

            var result = _service.GetCurrentValue(1, true).ToList();

            Assert.AreEqual(0, result.Count);
        }
    }
}
