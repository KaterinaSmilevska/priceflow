using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace Tests.Unit.Controllers
{
    [TestClass]
    public class ChartControllerTests
    {
        private Mock<IChartService> _serviceMock = null!;
        private ChartController _controller = null!;

        private const int SecurityId = 1;
        private const int PortfolioId = 1;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IChartService>();
            _controller = new ChartController(_serviceMock.Object);
        }

        [TestMethod]
        public void GetPriceTrend_WhenDataExists_ReturnsOkWithPriceTrend()
        {
            var startDate = new DateTime(2026, 1, 1);
            var endDate = new DateTime(2026, 1, 31);
            var data = new List<PriceTrend>();

            _serviceMock
                .Setup(s => s.GetPriceTrend(SecurityId, startDate, endDate))
                .Returns(data);

            var result = _controller.GetPriceTrend(SecurityId, startDate, endDate);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(data, ok.Value);

            _serviceMock.Verify(
                s => s.GetPriceTrend(SecurityId, startDate, endDate),
                Times.Once);
        }

        [TestMethod]
        public void GetPriceTrend_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var startDate = new DateTime(2026, 1, 1);
            var endDate = new DateTime(2026, 1, 31);
            var data = new List<PriceTrend>();

            _serviceMock
                .Setup(s => s.GetPriceTrend(SecurityId, startDate, endDate))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetPriceTrend(SecurityId, startDate, endDate);
            var error = result.Result as ObjectResult;
            
            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetSectorDistribution_WhenDataExists_ReturnsOkWithDistribution()
        {
            var date = new DateTime(2026, 1, 31);
            var data = new List<SectorDistribution>();

            _serviceMock
                .Setup(s => s.GetSectorDistribution(date))
                .Returns(data);

            var result = _controller.GetSectorDistribution(date);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(data, ok.Value);

            _serviceMock.Verify(
                s => s.GetSectorDistribution(date),
                Times.Once);
        }

        [TestMethod]
        public void GetSectorDistribution_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var date = new DateTime(2026, 1, 31);

            _serviceMock
               .Setup(s => s.GetSectorDistribution(date))
               .Throws(new Exception("Unexpected error"));

            var result = _controller.GetSectorDistribution(date);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetMonthlyIncome_WhenDataExists_ReturnsOkWithMonthlyIncome()
        {
            const bool isReal = true;
            var data = new List<MonthlyIncome>();

            _serviceMock
                .Setup(s => s.GetMonthlyIncome(PortfolioId, isReal))
                .Returns(data);

            var result = _controller.GetMonthlyIncome(PortfolioId, isReal);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(data, ok.Value);

            _serviceMock.Verify(
                s => s.GetMonthlyIncome(PortfolioId, isReal),
                Times.Once);
        }

        [TestMethod]
        public void GetMonthlyIncome_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const bool isReal = false;

            _serviceMock
                .Setup(s => s.GetMonthlyIncome(PortfolioId, isReal))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetMonthlyIncome(PortfolioId, isReal);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetSecurityAllocation_WhenDataExists_ReturnsOkWithAllocation()
        {
            const bool isReal = true;
            var data = new List<SecurityAllocation>();

            _serviceMock
                .Setup(s => s.GetAllocation(PortfolioId, isReal))
                .Returns(data);

            var result = _controller.GetSecurityAllocation(PortfolioId, isReal);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(data, ok.Value);

            _serviceMock.Verify(
                s => s.GetAllocation(PortfolioId, isReal),
                Times.Once);
        }

        [TestMethod]
        public void GetSecurityAllocation_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const bool isReal = false;

            _serviceMock
                .Setup(s => s.GetAllocation(PortfolioId, isReal))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetSecurityAllocation(PortfolioId, isReal);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetSecurities_WhenDataExists_ReturnsOkWithSecurities()
        {
            var data = new List<Security>();

            _serviceMock
                .Setup(s => s.GetSecurities())
                .Returns(data);

            var result = _controller.GetSecurities();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(data, ok.Value);

            _serviceMock.Verify(
                s => s.GetSecurities(),
                Times.Once);
        }

        [TestMethod]
        public void GetSecurities_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.GetSecurities())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetSecurities();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void FindLatestDate_WhenDateExists_ReturnsOkWithLatestDate()
        {
            var latestDate = new DateTime(2026, 10, 9);
            
            _serviceMock
                .Setup(s => s.FindLatestDate())
                .Returns(latestDate);

            var result = _controller.GetLatestDate();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreEqual(latestDate, ok.Value);

            _serviceMock.Verify(
                s => s.FindLatestDate(),
                Times.Once);
        }

        [TestMethod]
        public void GetLatestDate_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.FindLatestDate())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetLatestDate();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }
    }
}
