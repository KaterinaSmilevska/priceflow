using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class PortfoliosServiceTests
    {
        private Mock<IPortfoliosRepository> _portfoliosRepositoryMock = null!;
        private Mock<ITransactionsRepository> _transactionsRepositoryMock = null!;
        private Mock<IDailyTurnoverRepository> _dailyTurnoverRepositoryMock = null!;
        private Mock<IAuthRepository> _authRepositoryMock = null!;
        private Mock<IPortfolioReturnsService> _portfolioReturnsServiceMock = null!;
        private PortfoliosService _portfoliosService = null!;

        [TestInitialize]
        public void Setup()
        {
            _portfoliosRepositoryMock = new Mock<IPortfoliosRepository>();
            _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
            _dailyTurnoverRepositoryMock = new Mock<IDailyTurnoverRepository>();
            _authRepositoryMock = new Mock<IAuthRepository>();
            _portfolioReturnsServiceMock = new Mock<IPortfolioReturnsService>();
            _portfoliosService = new PortfoliosService(_portfoliosRepositoryMock.Object, _transactionsRepositoryMock.Object,
                _dailyTurnoverRepositoryMock.Object, _portfolioReturnsServiceMock.Object, _authRepositoryMock.Object);
        }

        [TestMethod]
        public void FindById_ExistingPortfolio_ReturnsMappedPortfolio()
        {
            var portfolio = new Portfolija
            {
                Id = 1,
                Ime = "My Portfolio",
                Opis = "Long-term investments"
            };
            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(portfolio);

            var result = _portfoliosService.FindById(1);

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("My Portfolio", result.Name);
            Assert.AreEqual("Long-term investments", result.Description);

            _portfoliosRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);
        }

        [TestMethod]
        public void FindById_PortfolioNotFound_ThrowsNotFoundException()
        {
            _portfoliosRepositoryMock
                .Setup(r => r.GetById(100))
                .Returns((Portfolija?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _portfoliosService.FindById(1000));

            Assert.AreEqual("PORTFOLIO_NOT_FOUND", exception.Code);
        }

        [TestMethod]
        public void FindUserPortfolios_ExistingUser_ReturnsMappedPortfolios()
        {
            _authRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns(new Korisnici { Id = 10 });

            var portfolios = new List<Portfolija>
            {
                new Portfolija
                {
                    Id = 1,
                    Ime = "Portfolio A",
                },
                new Portfolija
                {
                    Id = 2,
                    Ime = "Portfolio B"
                }
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetByUserId(10))
                .Returns(portfolios);

            var result = _portfoliosService.FindUserPortfolios(10).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual("Portfolio A", result[0].Name);
            Assert.AreEqual("Portfolio B", result[1].Name);

            _portfoliosRepositoryMock.Verify(
                r => r.GetByUserId(10),
                Times.Once);
        }

        [TestMethod]
        public void FindUserPortfolios_UserNotFound_ThrowsNotFoundException()
        {
            _authRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((Korisnici?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _portfoliosService.FindUserPortfolios(1000));

            Assert.AreEqual("USER_NOT_FOUND", exception.Code);

            _portfoliosRepositoryMock.Verify(
                r => r.GetByUserId(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_ValidRequest_AddsAndReturnsPortfolio()
        {
            var request = new AddPortfolioRequest
            {
                Name = "New Portfolio",
                Description = "My investments"
            };

            var portfolio = new Portfolija
            {
                Id = 5,
                Ime = "New Portfolio",
                Opis = "My investments",
                KorisnikId = 10
            };

            _authRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns(new Korisnici { Id = 10 });

            _portfoliosRepositoryMock
                .Setup(r => r.GetByName("New Portfolio", 10))
                .Returns((Portfolija?)null);

            _portfoliosRepositoryMock
                .Setup(r => r.Add(It.IsAny<Portfolija>()))
                .Returns(portfolio);

            var result = _portfoliosService.Add(10, request);

            Assert.AreEqual(5, result.Id);
            Assert.AreEqual("New Portfolio", result.Name);
            Assert.AreEqual("My investments", result.Description);

            _portfoliosRepositoryMock.Verify(
                r => r.Add(It.Is<Portfolija>(p =>
                p.Ime == "New Portfolio" &&
                p.Opis == "My investments" &&
                p.KorisnikId == 10)),
                Times.Once);
        }

        [TestMethod]
        public void Add_DuplicateName_ThrowsAlreadyExistsException()
        {
            var request = new AddPortfolioRequest
            {
                Name = "Existing Portfolio"
            };

            var existingPortfolio = new Portfolija
            {
                Id = 1,
                Ime = "Existing Portfolio",
                KorisnikId = 10
            };

            _authRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns(new Korisnici { Id = 10 });

            _portfoliosRepositoryMock
                .Setup(r => r.GetByName("Existing Portfolio", 10))
                .Returns(existingPortfolio);

            var exception = Assert.ThrowsException<AlreadyExistsException>(
                () => _portfoliosService.Add(10, request));

            Assert.AreEqual("NAME_ALREADY_EXISTS", exception.Code);

            _portfoliosRepositoryMock.Verify(
                r => r.Add(It.IsAny<Portfolija>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_ValidRequest_UpdatesPortfolio()
        {
            var existingPortfolio = new Portfolija
            {
                Id = 1,
                Ime = "Old Name",
                Opis = "Old Description",
                KorisnikId = 10
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetByName("Updated Name", 10))
                .Returns((Portfolija?)null);

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(existingPortfolio);

            _portfoliosRepositoryMock
                .Setup(r => r.Update(It.IsAny<Portfolija>()))
                .Returns(existingPortfolio);

            var request = new UpdatePortfolio
            {
                Name = "Updated Name",
                Description = "Updated Description"
            };

            var result = _portfoliosService.Update(1, 10, request);

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Updated Name", result.Name);
            Assert.AreEqual("Updated Description", result.Description);

            _portfoliosRepositoryMock.Verify(
                r => r.Update(It.Is<Portfolija>(p =>
                p.Ime == "Updated Name" &&
                p.Opis == "Updated Description")),
                Times.Once);
        }

        [TestMethod]
        public void Update_DuplicateName_ThrowsAlreadyExistsException()
        {
            var existingPortfolio = new Portfolija
            {
                Id = 2,
                Ime = "Taken Name",
                KorisnikId = 10
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetByName("Taken Name", 10))
                .Returns(existingPortfolio);

            var request = new UpdatePortfolio
            {
                Name = "Taken Name",
                Description = "Description"
            };

            var exception = Assert.ThrowsException<AlreadyExistsException>(
                () => _portfoliosService.Update(1, 10, request));

            Assert.AreEqual("NAME_ALREADY_EXISTS", exception.Code);

            _portfoliosRepositoryMock.Verify(
                r => r.Update(It.IsAny<Portfolija>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_SameNameAsCurrentPortfolio_AllowsUpdate()
        {
            var existingPortfolio = new Portfolija
            {
                Id = 1,
                Ime = "My Portfolio",
                Opis = "Old",
                KorisnikId = 10
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetByName("My Portfolio", 10))
                .Returns(existingPortfolio);

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(existingPortfolio);

            _portfoliosRepositoryMock
                .Setup(r => r.Update(It.IsAny<Portfolija>()))
                .Returns(existingPortfolio);

            var request = new UpdatePortfolio
            {
                Name = "My Portfolio",
                Description = "Updated"
            };

            var result = _portfoliosService.Update(1, 10, request);

            Assert.AreEqual("My Portfolio", result.Name);
            Assert.AreEqual("Updated", result.Description);

            _portfoliosRepositoryMock.Verify(
                r => r.Update(It.IsAny<Portfolija>()),
                Times.Once);
        }

        [TestMethod]
        public void Update_OwnedPortfolio_DeletesAndReturnsPortfolio()
        {
            var portfolio = new Portfolija
            {
                Id = 1,
                Ime = "My Portfolio",
                KorisnikId = 10
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(portfolio);

            _portfoliosRepositoryMock
                .Setup(r => r.Delete(portfolio))
                .Returns(portfolio);

            var result = _portfoliosService.Delete(1, 10);

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("My Portfolio", result.Name);

            _portfoliosRepositoryMock.Verify(
                r => r.Delete(portfolio),
                Times.Once);
        }

        [TestMethod]
        public void Delete_PortfolioOwnedByAnotherUser_ThrowsUnauthorizedException()
        {
            var portfolio = new Portfolija
            {
                Id = 1,
                Ime = "Another User's Portfolio",
                KorisnikId = 2
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(portfolio);

            var exception = Assert.ThrowsException<UnauthorizedException>(
                () => _portfoliosService.Delete(1, 10));

            Assert.AreEqual("ACCESS_DENIED", exception.Code);

            _portfoliosRepositoryMock.Verify(
                r => r.Delete(It.IsAny<Portfolija>()),
                Times.Never);
        }

        [TestMethod]
        public void GeneratePerformanceSummary_PortfolioNotFound_ThrowsNotFoundException()
        {
            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((Portfolija?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _portfoliosService.GeneratePerformanceSummary(1000, new DateOnly(2026, 1, 1), new DateOnly(2026, 3, 21)));

            Assert.AreEqual("PORTFOLIO_NOT_FOUND", exception.Code);

            _transactionsRepositoryMock.Verify(
                t => t.GetByPortfolioIdUntilDate(
                    It.IsAny<int>(),
                    It.IsAny<DateOnly>()),
                Times.Never);
        }

        [TestMethod]
        public void GeneratePerformanceSummary_ValidPortfolio_CalculatesCorrectSummary()
        {
            var from = new DateOnly(2026, 1, 1);
            var to = new DateOnly(2026, 1, 31);

            var portfolio = new Portfolija
            {
                Id = 1,
                Ime = "My Portfolio"
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(portfolio);

            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Hvid = 10,
                    Datum = new DateOnly(2025, 12, 15),
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 100,
                    Iznos = 10000m,
                    BrokerskaProvizija = 0.5m,
                    BerzanskaProvizija = 0.2m,
                    Cdhvprovizija = 0.1m
                },
                new Transakcii
                {
                    Hvid = 10,
                    Datum = new DateOnly(2026, 1, 10),
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 50,
                    Iznos = 6000m,
                    BrokerskaProvizija = 0.5m,
                    BerzanskaProvizija = 0.2m,
                    Cdhvprovizija = 0.1m
                },
                new Transakcii
                {
                    Hvid = 10,
                    Datum = new DateOnly(2026, 1, 20),
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 20,
                    Iznos = 3000m,
                    BrokerskaProvizija = 0.5m,
                    BerzanskaProvizija = 0.2m,
                    Cdhvprovizija = 0.1m
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioIdUntilDate(1, to))
                .Returns(transactions);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestPrice(10, from))
                .Returns(100m);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestPrice(10, to))
                .Returns(120m);

            _portfolioReturnsServiceMock
                .Setup(r => r.CalculateSummaryForPeriod(1, from, to))
                .Returns(new PortfolioReturnsSummary
                {
                    TotalDividends = 500m,
                    TotalTaxes = 50m
                });

            var result = _portfoliosService.GeneratePerformanceSummary(1, from, to);

            Assert.AreEqual("My Portfolio", result.PortfolioName);
            Assert.AreEqual(from, result.FromDate);
            Assert.AreEqual(to, result.ToDate);

            Assert.AreEqual(10000m, result.StartingValue);
            Assert.AreEqual(15600m, result.EndingValue);
            Assert.AreEqual(500m, result.Dividends);
            Assert.AreEqual(72m, result.Commissions);
            Assert.AreEqual(6028m, result.AbsoluteReturn);

            _transactionsRepositoryMock.Verify(
                r => r.GetByPortfolioIdUntilDate(1, to),
                Times.Once);

            _portfolioReturnsServiceMock.Verify(
                r => r.CalculateSummaryForPeriod(1, from, to),
                Times.Once);

            _dailyTurnoverRepositoryMock.Verify(
                r => r.GetLatestPrice(10, from),
                Times.Once);

            _dailyTurnoverRepositoryMock.Verify(
                r => r.GetLatestPrice(10, to),
                Times.Once);
        }

        [TestMethod]
        public void GeneratePerfomanceSummary_NoTransactions_ReturnsDividendsWithoutHoldings()
        {
            var from = new DateOnly(2026, 1, 1);
            var to = new DateOnly(2026, 1, 31);

            var portfolio = new Portfolija
            {
                Id = 1,
                Ime = "Empty Portfolio"
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(portfolio);

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioIdUntilDate(1, to))
                .Returns(new List<Transakcii>());

            _portfolioReturnsServiceMock
                .Setup(r => r.CalculateSummaryForPeriod(1, from, to))
                .Returns(new PortfolioReturnsSummary
                {
                    TotalDividends = 250m,
                    TotalTaxes = 25m
                });

            var result = _portfoliosService.GeneratePerformanceSummary(1, from, to);

            Assert.AreEqual("Empty Portfolio", result.PortfolioName);

            Assert.AreEqual(0m, result.StartingValue);
            Assert.AreEqual(0m, result.EndingValue);
            Assert.AreEqual(250m, result.Dividends);
            Assert.AreEqual(0m, result.Commissions);
            Assert.AreEqual(250m, result.AbsoluteReturn);


            _dailyTurnoverRepositoryMock.Verify(
                r => r.GetLatestPrice(
                    It.IsAny<int>(),
                    It.IsAny<DateOnly>()),
                Times.Never);
        }
    }
}
