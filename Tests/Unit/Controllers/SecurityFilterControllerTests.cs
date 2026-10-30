using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace Tests.Unit.Controllers
{
    [TestClass]
    public class SecurityFilterControllerTests
    {
        private Mock<ISecurityFilterService> _service = null!;
        private SecurityFilterController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _service = new Mock<ISecurityFilterService>();
            _controller = new SecurityFilterController(_service.Object);
        }

        [TestMethod]
        public void GetMostProfitableSecuritiesByDividendYield_WhenSuccessful_ReturnsOkWithSecurities()
        {
            var securities = new List<FilteredSecurity>
            {
                new FilteredSecurity()
            };

            _service
                .Setup(s => s.FindMostProfitableSecuritiesByDividendYield())
                .Returns(securities);

            var result = _controller.GetMostProfitableSecuritiesByDividendYield();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _service.Verify(
                s => s.FindMostProfitableSecuritiesByDividendYield(),
                Times.Once);
        }

        [TestMethod]
        public void GetMostProfitableSecuritiesByDividendYield_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindMostProfitableSecuritiesByDividendYield())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetMostProfitableSecuritiesByDividendYield();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetMostProfitableSecuritiesByDividendPerShare_WhenSuccessful_ReturnsOkWithSecurities()
        {
            var securities = new List<FilteredSecurity>
            {
                new FilteredSecurity()
            };

            _service
                .Setup(s => s.FindMostProfitableSecuritiesByDividendPerShare())
                .Returns(securities);

            var result = _controller.GetMostProfitableSecuritiesByDividendPerShare();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _service.Verify(
                s => s.FindMostProfitableSecuritiesByDividendPerShare(),
                Times.Once);
        }

        [TestMethod]
        public void GetMostProfitableSecuritiesByDividendPerShare_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindMostProfitableSecuritiesByDividendPerShare())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetMostProfitableSecuritiesByDividendPerShare();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetSecuritiesWithBiggestPriceOscillations_WhenSuccessful_ReturnsOkWithSecurities()
        {
            var securities = new List<FilteredSecurity>
            {
                new FilteredSecurity()
            };

            _service
                .Setup(s => s.FindSecuritiesWithBiggestPriceOscillations())
                .Returns(securities);

            var result = _controller.GetSecuritiesWithBiggestPriceOscillations();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _service.Verify(
                s => s.FindSecuritiesWithBiggestPriceOscillations(),
                Times.Once);
        }

        [TestMethod]
        public void GetSecuritiesWithBiggestPriceOscillations_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindSecuritiesWithBiggestPriceOscillations())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetSecuritiesWithBiggestPriceOscillations();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetSecuritiesWithSmallestPriceOscillations_WhenSuccessful_ReturnsOkWithSecurities()
        {
            var securities = new List<FilteredSecurity>
            {
                new FilteredSecurity()
            };

            _service
                .Setup(s => s.FindSecuritiesWithSmallestPriceOscillations())
                .Returns(securities);

            var result = _controller.GetSecuritiesWithSmallestPriceOscillations();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _service.Verify(
                s => s.FindSecuritiesWithSmallestPriceOscillations(),
                Times.Once);
        }

        [TestMethod]
        public void GetSecuritiesWithSmallestPriceOscillations_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindSecuritiesWithSmallestPriceOscillations())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetSecuritiesWithSmallestPriceOscillations();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetLeastLiquidSecuritiesByTradedQuantity_WhenSuccessful_ReturnsOkWithSecurities()
        {
            var securities = new List<FilteredSecurity>
            {
                new FilteredSecurity()
            };

            _service
                .Setup(s => s.FindLeastLiquidSecuritiesByTradedQuantity())
                .Returns(securities);
            
            var result = _controller.GetLeastLiquidSecuritiesByTradedQuantity();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _service.Verify(
                s => s.FindLeastLiquidSecuritiesByTradedQuantity(),
                Times.Once);
        }

        [TestMethod]
        public void GetLeastLiquidSecuritiesByTradedQuantity_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindLeastLiquidSecuritiesByTradedQuantity())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetLeastLiquidSecuritiesByTradedQuantity();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetMostLiquidSecuritiesByTradedQuantity_WhenSuccessful_ReturnsOkWithSecurities()
        {
            var securities = new List<FilteredSecurity>
            {
                new FilteredSecurity()
            };

            _service
                .Setup(s => s.FindMostLiquidSecuritiesByTradedQuantity())
                .Returns(securities);

            var result = _controller.GetMostLiquidSecuritiesByTradedQuantity();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _service.Verify(
                s => s.FindMostLiquidSecuritiesByTradedQuantity(),
                Times.Once);
        }

        [TestMethod]
        public void GetMostLiquidSecuritiesByTradedQuantity_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindMostLiquidSecuritiesByTradedQuantity())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetMostLiquidSecuritiesByTradedQuantity();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetLeastLiquidSecuritiesByNumTradingDays_WhenSuccessful_ReturnsOkWithSecurities()
        {
            var securities = new List<FilteredSecurity>
            {
                new FilteredSecurity()
            };

            _service
                .Setup(s => s.FindLeastLiquidSecuritiesByNumTradingDays())
                .Returns(securities);

            var result = _controller.GetLeastLiquidSecuritiesByNumTradingDays();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _service.Verify(
                s => s.FindLeastLiquidSecuritiesByNumTradingDays(),
                Times.Once);
        }

        [TestMethod]
        public void GetLeastLiquidSecuritiesbyNumTradingDays_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindLeastLiquidSecuritiesByNumTradingDays())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetLeastLiquidSecuritiesByNumTradingDays();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetMostLiquidSecuritiesByNumTradingDays_WhenSuccessful_ReturnsOkWithSecurities()
        {
            var securities = new List<FilteredSecurity>
            {
                new FilteredSecurity()
            };

            _service
                .Setup(s => s.FindMostLiquidSecuritiesByNumTradingDays())
                .Returns(securities);

            var result = _controller.GetMostLiquidSecuritiesByNumTradingDays();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _service.Verify(
                s => s.FindMostLiquidSecuritiesByNumTradingDays(),
                Times.Once);
        }

        [TestMethod]
        public void GetMostLiquidSecuritiesByNumTradingDays_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindMostLiquidSecuritiesByNumTradingDays())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetMostLiquidSecuritiesByNumTradingDays();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetMostProfitableSectorsByDividendYield_WhenSuccessful_ReturnsOkWithSectors()
        {
            var sectors = new List<Sector>
            {
                new Sector()
            };

            _service
                .Setup(s => s.FindMostProfitableSectorsByDividendYield())
                .Returns(sectors);

            var result = _controller.GetMostProfitableSectorsByDividendYield();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(sectors, ok.Value);

            _service.Verify(
                s => s.FindMostProfitableSectorsByDividendYield(),
                Times.Once);
        }

        [TestMethod]
        public void GetMostProfitableSectorsByDividendYield_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindMostProfitableSectorsByDividendYield())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetMostProfitableSectorsByDividendYield();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetMostProfitableSectorsByProfit_WhenSuccessful_ReturnsOkWithSectors()
        {
            var sectors = new List<Sector>
            {
                new Sector()
            };

            _service
                .Setup(s => s.FindMostProfitableSectorsByProfit())
                .Returns(sectors);

            var result = _controller.GetMostProfitableSectorsByProfit();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(sectors, ok.Value);

            _service.Verify(
                s => s.FindMostProfitableSectorsByProfit(),
                Times.Once);
        }

        [TestMethod]
        public void GetMostProfitableSectorsByProfit_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindMostProfitableSectorsByProfit())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetMostProfitableSectorsByProfit();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetSecuritiesValuation_WhenSuccessful_ReturnsOkWithSecurities()
        {
            var securities = new List<FilteredSecurity>
            {
                new FilteredSecurity()
            };

            _service
                .Setup(s => s.FindSecuritiesValuation())
                .Returns(securities);

            var result = _controller.GetSecuritiesValuation();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _service.Verify(
                s => s.FindSecuritiesValuation(),
                Times.Once);
        }

        [TestMethod]
        public void GetSecuritiesValuation_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindSecuritiesValuation())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetSecuritiesValuation();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }
    }
}
