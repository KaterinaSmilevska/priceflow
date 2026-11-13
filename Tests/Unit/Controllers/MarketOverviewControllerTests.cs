using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;
using System.Security.Claims;

namespace Tests.Unit.Controllers
{
    [TestClass]
    public class MarketOverviewControllerTests
    {
        private Mock<IMarketOverviewService> _serviceMock = null!;
        private MarketOverviewController _controller = null!;

        private const int UserId = 1;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IMarketOverviewService>();
            _controller = new MarketOverviewController(_serviceMock.Object);

            SetAuthenticatedUser(UserId);
        }

        private void SetAuthenticatedUser(int userId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "TestAuthentication");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };
        }

        [TestMethod]
        public void GetMarketOverview_WhenDataExists_ReturnsOkWithOverview()
        {
            var overview = new MarketOverview();

            _serviceMock
                .Setup(s => s.GetOverview())
                .Returns(overview);

            var result = _controller.GetMarketOverview();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(overview, ok.Value);

            _serviceMock.Verify(
                s => s.GetOverview(),
                Times.Once);
        }

        [TestMethod]
        public void GetOverview_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.GetOverview())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetMarketOverview();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetTopGainers_WhenDataExists_ReturnsOkWithGainers()
        {
            const int count = 5;
            var gainers = new List<SecurityPerformance>();

            _serviceMock
                .Setup(s => s.GetTopGainers(count))
                .Returns(gainers);

            var result = _controller.GetTopGainers(count);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(gainers, ok.Value);

            _serviceMock.Verify(
                s => s.GetTopGainers(count),
                Times.Once);
        }

        [TestMethod]
        public void GetTopGainers_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const int count = 5;

            _serviceMock
               .Setup(s => s.GetTopGainers(count))
               .Throws(new Exception("Unexpected error"));

            var result = _controller.GetTopGainers(count);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetTopLosers_WhenDataExists_ReturnsOKWithLosers()
        {
            const int count = 5;
            var losers = new List<SecurityPerformance>();

            _serviceMock
                .Setup(s => s.GetTopLosers(count))
                .Returns(losers);

            var result = _controller.GetTopLosers(count);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(losers, ok.Value);

            _serviceMock.Verify(
                s => s.GetTopLosers(count),
                Times.Once);
        }

        [TestMethod]
        public void GetTopLosers_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const int count = 5;

            _serviceMock
              .Setup(s => s.GetTopLosers(count))
              .Throws(new Exception("Unexpected error"));

            var result = _controller.GetTopLosers(count);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetMostTraded_WhenDataExists_ReturnsOkWithMostTraded()
        {
            const int count = 5;
            var securities = new List<SecurityPerformance>();

            _serviceMock
                .Setup(s => s.GetMostTraded(count))
                .Returns(securities);

            var result = _controller.GetMostTraded(count);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _serviceMock.Verify(
                s => s.GetMostTraded(count),
                Times.Once);
        }

        [TestMethod]
        public void GetMostTraded_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const int count = 5;

            _serviceMock
                .Setup(s => s.GetMostTraded(count))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetMostTraded(count);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetLiquidity_WhenDataExists_ReturnsOkWithLiquidity()
        {
            const int months = 6;
            const bool onlyOwned = true;
            var liquidity = new LiquidityOverview();

            _serviceMock
                .Setup(s => s.FindLiquidity(UserId, months, onlyOwned))
                .Returns(liquidity);

            var result = _controller.GetLiquidity(months, onlyOwned);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(liquidity, ok.Value);

            _serviceMock.Verify(
                s => s.FindLiquidity(UserId, months, onlyOwned),
                Times.Once);
        }

        [TestMethod]
        public void GetLiquidity_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const int months = 6;
            const bool onlyOwned = true;

            _serviceMock
                .Setup(s => s.FindLiquidity(UserId, months, onlyOwned))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetLiquidity(months, onlyOwned);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetLiquidity_WhenCustomParametersProvided_PassesParametersToService()
        {
            const int months = 12;
            const bool onlyOwned = false;
            var liquidity = new LiquidityOverview();

            _serviceMock
                .Setup(s => s.FindLiquidity(UserId, months, onlyOwned))
                .Returns(liquidity);

            var result = _controller.GetLiquidity(months, onlyOwned);
            var ok = result.Result as ObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(liquidity, ok.Value);

            _serviceMock.Verify(
                s => s.FindLiquidity(UserId, 12, false),
                Times.Once);
        }
    }
}
