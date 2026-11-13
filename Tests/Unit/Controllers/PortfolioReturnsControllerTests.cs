using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace Tests.Unit.Controllers
{
    [TestClass]
    public class PortfolioReturnsControllerTests
    {
        private Mock<IPortfolioReturnsService> _serviceMock = null!;
        private PortfolioReturnsController _controller = null!;

        private const int PortfolioId = 1;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IPortfolioReturnsService>();
            _controller = new PortfolioReturnsController(_serviceMock.Object);
        }

        [TestMethod]
        public void GetByPortfolioId_WhenReturnsExist_ReturnsOkWithReturns()
        {
            var returns = new List<PortfolioReturns>();

            _serviceMock
                .Setup(s => s.FindByPortfolioId(PortfolioId))
                .Returns(returns);

            var result = _controller.GetByPortfolioId(PortfolioId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(returns, ok.Value);

            _serviceMock.Verify(
                s => s.FindByPortfolioId(PortfolioId),
                Times.Once);
        }

        [TestMethod]
        public void GetByPortfolioId_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.FindByPortfolioId(PortfolioId))
                .Throws(new Exception("Unexpected exception"));

            var result = _controller.GetByPortfolioId(PortfolioId);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Add_WhenSuccessful_ReturnsOkWithAddedReturn()
        {
            var portfolioReturn = new PortfolioReturns();

            _serviceMock
                .Setup(s => s.Add(portfolioReturn))
                .Returns(portfolioReturn);

            var result = _controller.Add(portfolioReturn);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(portfolioReturn, ok.Value);

            _serviceMock.Verify(
                s => s.Add(portfolioReturn),
                Times.Once);
        }

        [TestMethod]
        public void Add_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var portfolioReturn = new PortfolioReturns();

            _serviceMock
              .Setup(s => s.Add(portfolioReturn))
              .Throws(new Exception("Unexpected exception"));

            var result = _controller.Add(portfolioReturn);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetSummary_WhenSummaryExists_ReturnsOkWithSummary()
        {
            var summary = new PortfolioReturnsSummary();

            _serviceMock
                .Setup(s => s.CalculateSummary(PortfolioId))
                .Returns(summary);

            var result = _controller.GetSummary(PortfolioId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(summary, ok.Value);

            _serviceMock.Verify(
                s => s.CalculateSummary(PortfolioId),
                Times.Once);
        }

        [TestMethod]
        public void GetSummary_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.CalculateSummary(PortfolioId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetSummary(PortfolioId);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }
    }
}
