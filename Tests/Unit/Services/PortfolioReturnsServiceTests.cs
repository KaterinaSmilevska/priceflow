using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class PortfolioReturnsServiceTests
    {
        private Mock<IPortfolioReturnsRepository> _portfolioReturnsRepositoryMock = null!;
        private Mock<IPortfoliosRepository> _portfoliosRepositoryMock = null!;
        private Mock<ISecuritiesRepository> _securitiesRepositoryMock = null!;
        private PortfolioReturnsService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _portfolioReturnsRepositoryMock = new Mock<IPortfolioReturnsRepository>();
            _portfoliosRepositoryMock = new Mock<IPortfoliosRepository>();
            _securitiesRepositoryMock = new Mock<ISecuritiesRepository>();
            _service = new PortfolioReturnsService(_portfolioReturnsRepositoryMock.Object, 
                _portfoliosRepositoryMock.Object, _securitiesRepositoryMock.Object);
        }

        [TestMethod]
        public void FindByPortfolioId_ValidPortfolio_ReturnsMappedPortfolioReturns()
        {
            var portfolio = new Portfolija
            {
                Id = 1
            };

            var portfolioReturns = new List<PortfolioPrinosi>
            {
                new PortfolioPrinosi
                {
                    Datum = new DateOnly(2026, 2, 15),
                    NetoIznos = 1000m,
                    Danok = 100m,
                    PortfolioId = 1,
                    Hvid = 10
                },
                 new PortfolioPrinosi
                {
                    Datum = new DateOnly(2026, 2, 15),
                    NetoIznos = 1500m,
                    Danok = 150m,
                    PortfolioId = 1,
                    Hvid = 1
                },
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(portfolio);

            _portfolioReturnsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1))
                .Returns(portfolioReturns);

            var result = _service.FindByPortfolioId(1).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(new DateOnly(2026, 2, 15), result[0].Date);
            Assert.AreEqual(1000m, result[0].NetAmount);
            Assert.AreEqual(100m, result[0].Tax);
            Assert.AreEqual(1, result[0].PortfolioId);
            Assert.AreEqual(10, result[0].SecurityId);


            Assert.AreEqual(new DateOnly(2026, 2, 15), result[1].Date);
            Assert.AreEqual(1500m, result[1].NetAmount);
            Assert.AreEqual(150m, result[1].Tax);
            Assert.AreEqual(1, result[1].PortfolioId);
            Assert.AreEqual(1, result[1].SecurityId);

            _portfolioReturnsRepositoryMock.Verify(
                r => r.GetByPortfolioId(1),
                Times.Once);
        }

        [TestMethod]
        public void FindByPortfolioId_PortfolioNotFound_ThrowsNotFoundException()
        {
            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((Portfolija?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.FindByPortfolioId(1000));

            Assert.AreEqual("PORTFOLIO_NOT_FOUND", exception.Code);

            _portfolioReturnsRepositoryMock.Verify(
                r => r.GetByPortfolioId(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_ValidRequest_CreatesAndReturnsPortfolioReturns()
        {
            var portfolio = new Portfolija
            {
                Id = 1
            };

            var security = new HartiiOdVrednost
            {
                Id = 10
            };

            var request = new PortfolioReturns
            {
                PortfolioId = 1,
                SecurityId = 10,
                Date = new DateOnly(2026, 3, 1),
                NetAmount = 2000m,
                Tax = 200m
            };

            var createdReturn = new PortfolioPrinosi
            {
                Datum = request.Date,
                NetoIznos = request.NetAmount,
                Danok = request.Tax,
                PortfolioId = 1,
                Hvid = 10
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(portfolio);

            _securitiesRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns(security);

            _portfolioReturnsRepositoryMock
                .Setup(r => r.Add(It.IsAny<PortfolioPrinosi>()))
                .Returns(createdReturn);

            var result = _service.Add(request);

            Assert.AreEqual(request.Date, result.Date);
            Assert.AreEqual(result.NetAmount, result.NetAmount);
            Assert.AreEqual(request.Tax, result.Tax);
            Assert.AreEqual(1, result.PortfolioId);
            Assert.AreEqual(10, result.SecurityId);

            _portfolioReturnsRepositoryMock.Verify(
                r => r.Add(It.Is<PortfolioPrinosi>(x =>
                x.Datum == request.Date &&
                x.NetoIznos == request.NetAmount &&
                x.Danok == request.Tax &&
                x.PortfolioId == 1 &&
                x.Hvid == 10)),
                Times.Once);
        }

        [TestMethod]
        public void Add_PortfolioNotFound_ThrowsNotFoundException()
        {
            var request = new PortfolioReturns
            {
                PortfolioId = 1000,
                SecurityId = 10,
                Date = new DateOnly(2026, 3, 1),
                NetAmount = 1000m,
                Tax = 100m
            };

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((Portfolija?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.Add(request));

            Assert.AreEqual("PORTFOLIO_NOT_FOUND", exception.Code);

            _securitiesRepositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);

            _portfolioReturnsRepositoryMock.Verify(
                r => r.Add(It.IsAny<PortfolioPrinosi>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_SecurityNotFound_ThrowsNotFoundException()
        {
            var request = new PortfolioReturns
            {
                PortfolioId = 1,
                SecurityId = 1000,
                Date = new DateOnly(2026, 3, 1),
                NetAmount = 1000m,
                Tax = 100m
            };

            _portfoliosRepositoryMock
              .Setup(r => r.GetById(1))
              .Returns(new Portfolija { Id = 1 });

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.Add(request));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);

            _portfolioReturnsRepositoryMock.Verify(
                r => r.Add(It.IsAny<PortfolioPrinosi>()),
                Times.Never);
        }

        [TestMethod]
        public void CalculateSummary_ValidPortfolio_ReturnsTotals()
        {
            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(new Portfolija { Id = 1 });

            _portfolioReturnsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1))
                .Returns(new List<PortfolioPrinosi>
                {
                    new PortfolioPrinosi
                    {
                        NetoIznos = 1000m,
                        Danok = 100m
                    },
                    new PortfolioPrinosi
                    {
                        NetoIznos = 1500m,
                        Danok = 150m
                    },
                    new PortfolioPrinosi
                    {
                        NetoIznos = 500m,
                        Danok = 50m
                    }
                });

            var result = _service.CalculateSummary(1);

            Assert.AreEqual(3000m, result.TotalDividends);
            Assert.AreEqual(300m, result.TotalTaxes);

            _portfolioReturnsRepositoryMock.Verify(
                r => r.GetByPortfolioId(1),
                Times.Once);
        }

        [TestMethod]
        public void CalculateSummary_PortfolioNotFound_ThrowsNotFoundException()
        {
            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((Portfolija?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.CalculateSummary(1000));

            Assert.AreEqual("PORTFOLIO_NOT_FOUND", exception.Code);

            _portfolioReturnsRepositoryMock.Verify(
                r => r.GetByPortfolioId(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public void CalculateSummaryForPeriod_ValidDates_ReturnsTotals()
        {
            var fromDate = new DateOnly(2026, 1, 1);
            var toDate = new DateOnly(2026, 3, 31);

            _portfoliosRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(new Portfolija { Id = 1 });

            _portfolioReturnsRepositoryMock
                .Setup(r => r.GetByPortfolioIdForPeriod(1, fromDate, toDate))
                .Returns(new List<PortfolioPrinosi>
                {
                    new PortfolioPrinosi
                    {
                        NetoIznos = 1000m,
                        Danok = 100m
                    },
                    new PortfolioPrinosi
                    {
                        NetoIznos = 2000m,
                        Danok = 200m
                    },
                });

            var result = _service.CalculateSummaryForPeriod(1, fromDate, toDate);

            Assert.AreEqual(3000m, result.TotalDividends);
            Assert.AreEqual(300m, result.TotalTaxes);

            _portfolioReturnsRepositoryMock.Verify(
                r => r.GetByPortfolioIdForPeriod(1, fromDate, toDate),
                Times.Once);
        }

        [TestMethod]
        public void CalculateSummaryForPeriod_InvalidDateRange_ThrowsValidationException()
        {
            var fromDate = new DateOnly(2026, 4, 1);
            var toDate = new DateOnly(2026, 3, 1);

            _portfoliosRepositoryMock
           .Setup(r => r.GetById(1))
           .Returns(new Portfolija { Id = 1 });

            var exception = Assert.ThrowsException<ValidationException>(
                () => _service.CalculateSummaryForPeriod(1, fromDate, toDate));

            Assert.AreEqual("INVALID_DATE_RANGE", exception.Code);

            _portfolioReturnsRepositoryMock.Verify(
                r => r.GetByPortfolioIdForPeriod(
                    It.IsAny<int>(),
                    It.IsAny<DateOnly>(),
                    It.IsAny<DateOnly>()),
                Times.Never);
        }
    }
}
