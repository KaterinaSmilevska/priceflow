using DataAccess.Enums;
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
    public class PortfoliosControllerTests
    {
        private Mock<IPortfoliosService> _portfoliosServiceMock = null!;
        private Mock<ITransactionsService> _transactionsServiceMock = null!;
        private Mock<IPortfolioReportExportService> _portfolioReportExportServiceMock = null!;
        private Mock<ISecurityPriceTrendReportService> _securityPriceTrendReportServiceMock = null!;
        private PortfoliosController _controller = null!;

        private const int UserId = 1;
        private const int PortfolioId = 10;

        [TestInitialize]
        public void Setup()
        {
            _portfoliosServiceMock = new Mock<IPortfoliosService>();
            _transactionsServiceMock = new Mock<ITransactionsService>();
            _portfolioReportExportServiceMock = new Mock<IPortfolioReportExportService>();
            _securityPriceTrendReportServiceMock = new Mock<ISecurityPriceTrendReportService>();
            _controller = new PortfoliosController(_portfoliosServiceMock.Object, _transactionsServiceMock.Object,
                _portfolioReportExportServiceMock.Object, _securityPriceTrendReportServiceMock.Object);

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
        public void GetById_WhenPortfolioExists_ReturnsOkWithPortfolio()
        {
            var portfolio = new Portfolio();

            _portfoliosServiceMock
                .Setup(s => s.FindById(PortfolioId))
                .Returns(portfolio);

            var result = _controller.GetById(PortfolioId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(portfolio, ok.Value);

            _portfoliosServiceMock.Verify(
                s => s.FindById(PortfolioId),
                Times.Once);
        }

        [TestMethod]
        public void GetById_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _portfoliosServiceMock
                .Setup(s => s.FindById(PortfolioId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetById(PortfolioId);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetAll_WhenPortfoliosExist_ReturnsOkWithPortfolios()
        {
            var portfolios = new List<Portfolio>();

            _portfoliosServiceMock
                .Setup(s => s.FindUserPortfolios(UserId))
                .Returns(portfolios);

            var result = _controller.GetAll();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(portfolios, ok.Value);

            _portfoliosServiceMock.Verify(
                s => s.FindUserPortfolios(UserId),
                Times.Once);
        }

        [TestMethod]
        public void GetAll_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _portfoliosServiceMock
               .Setup(s => s.FindUserPortfolios(UserId))
               .Throws(new Exception("Unexpected error"));

            var result = _controller.GetAll();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Add_WhenSuccessful_ReturnsOkWithAddedPortfolio()
        {
            var request = new AddPortfolioRequest();
            var portfolio = new Portfolio();

            _portfoliosServiceMock
                .Setup(s => s.Add(UserId, request))
                .Returns(portfolio);

            var result = _controller.Add(request);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(portfolio, ok.Value);

            _portfoliosServiceMock.Verify(
                s => s.Add(UserId, request),
                Times.Once);
        }

        [TestMethod]
        public void Add_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var request = new AddPortfolioRequest();

            _portfoliosServiceMock
             .Setup(s => s.Add(UserId, request))
             .Throws(new Exception("Unexpected error"));

            var result = _controller.Add(request);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Update_WhenIdsMatch_ReturnsOkWithUpdatedPortfolio()
        {
            var request = new UpdatePortfolio
            {
                Id = PortfolioId
            };
            var portfolio = new Portfolio();

            _portfoliosServiceMock
                .Setup(s => s.Update(PortfolioId, UserId, request))
                .Returns(portfolio);

            var result = _controller.Update(PortfolioId, request);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(portfolio, ok.Value);

            _portfoliosServiceMock.Verify(
                s => s.Update(PortfolioId,UserId, request), 
                Times.Once);
        }

        [TestMethod]
        public void Update_WhenIdsDoNotMatch_ReturnsBadRequest()
        {
            var request = new UpdatePortfolio
            {
                Id = 20
            };

            var result = _controller.Update(PortfolioId, request);
            var badRequest = result.Result as BadRequestObjectResult;

            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);

            _portfoliosServiceMock.Verify(
                s => s.Update(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<UpdatePortfolio>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var request = new UpdatePortfolio
            {
                Id = PortfolioId
            };

            _portfoliosServiceMock
                .Setup(s => s.Update(PortfolioId, UserId, request))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Update(PortfolioId, request);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Delete_WhenPortfolioExists_ReturnsOkWithDeletedPortfolio()
        {
            var portfolio = new Portfolio();

            _portfoliosServiceMock
                .Setup(s => s.Delete(PortfolioId, UserId))
                .Returns(portfolio);

            var result = _controller.Delete(PortfolioId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(portfolio, ok.Value);

            _portfoliosServiceMock.Verify(
                s => s.Delete(PortfolioId, UserId),
                Times.Once);
        }

        [TestMethod]
        public void Delete_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _portfoliosServiceMock
                .Setup(s => s.Delete(PortfolioId, UserId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Delete(PortfolioId);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrend_WhenDataExists_ReturnsOkWithTrends()
        {
            PriceTrendPeriod? period = null;
            PriceTrendResolution? resolution = null;
            var trends = new List<OwnedSecuritiesPriceTrend>();

            _transactionsServiceMock
                .Setup(s => s.FindPriceTrend(UserId, period, resolution))
                .Returns(trends);

            var result = _controller.GetSecuritiesPriceTrend(period, resolution);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(trends, ok.Value);

            _transactionsServiceMock.Verify(
                s => s.FindPriceTrend(UserId, period, resolution),
                Times.Once);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrend_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            PriceTrendPeriod? period = null;
            PriceTrendResolution? resolution = null;

            _transactionsServiceMock
                .Setup(s => s.FindPriceTrend(UserId, period, resolution))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetSecuritiesPriceTrend(period, resolution);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetPerformanceSummary_WhenFormatIsOmitted_ReturnsOkWithSummary()
        {
            var from = new DateOnly(2026, 1, 1);
            var to = new DateOnly(2026, 10, 1);
            var summary = new PortfolioPerformanceSummary();

            _portfoliosServiceMock
                .Setup(s => s.GeneratePerformanceSummary(PortfolioId, from, to))
                .Returns(summary);

            var result = _controller.GetPerformanceSummary(PortfolioId, from, to);
            var ok = result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreSame(summary, ok.Value);

            _portfoliosServiceMock.Verify(
                s => s.GeneratePerformanceSummary(PortfolioId, from, to),
                Times.Once);
        }

        [TestMethod]
        public void GeneratePerformanceSummary_WhenFormatIsCsv_ReturnsCsvFile()
        {
            var from = new DateOnly(2026, 1, 1);
            var to = new DateOnly(2026, 10, 1);
            var summary = new PortfolioPerformanceSummary();
            byte[] bytes = { 1, 2, 3 };

            _portfoliosServiceMock
                .Setup(s => s.GeneratePerformanceSummary(PortfolioId, from, to))
                .Returns(summary);

            _portfolioReportExportServiceMock
                .Setup(s => s.ExportToCsv(summary))
                .Returns(bytes);

            var result = _controller.GetPerformanceSummary(PortfolioId, from, to, "csv");
            var file = result as FileContentResult;

            Assert.IsNotNull(file);
            Assert.AreEqual("text/csv", file.ContentType);
            Assert.AreEqual("portfolio_performance_summary.csv", file.FileDownloadName);
            CollectionAssert.AreEqual(bytes, file.FileContents);

            _portfolioReportExportServiceMock.Verify(
                s => s.ExportToCsv(summary),
                Times.Once);
        }

        [TestMethod]
        public void GeneratePerformanceSummary_WhenFormatIsExcel_ReturnsExcelFile()
        {
            var from = new DateOnly(2026, 1, 1);
            var to = new DateOnly(2026, 10, 1);
            var summary = new PortfolioPerformanceSummary();
            byte[] bytes = { 4, 5, 6 };

            _portfoliosServiceMock
                .Setup(s => s.GeneratePerformanceSummary(PortfolioId, from, to))
                .Returns(summary);

            _portfolioReportExportServiceMock
                .Setup(s => s.ExportToExcel(summary))
                .Returns(bytes);

            var result = _controller.GetPerformanceSummary(PortfolioId, from, to, "excel");
            var file = result as FileContentResult;

            Assert.IsNotNull(file);
            Assert.AreEqual("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", file.ContentType);
            Assert.AreEqual("portfolio_performance_summary.xlsx", file.FileDownloadName);
            CollectionAssert.AreEqual(bytes, file.FileContents);

            _portfolioReportExportServiceMock.Verify(
                s => s.ExportToExcel(summary),
                Times.Once);
        }

        [TestMethod]
        public void GetPerformanceSummary_WhenFormatIsPdf_ReturnsPdfFile()
        {
            var from = new DateOnly(2026, 1, 1);
            var to = new DateOnly(2026, 10, 1);
            var summary = new PortfolioPerformanceSummary();
            byte[] bytes = { 7, 8, 9 };

            _portfoliosServiceMock
                .Setup(s => s.GeneratePerformanceSummary(PortfolioId, from, to))
                .Returns(summary);

            _portfolioReportExportServiceMock
                .Setup(s => s.ExportToPDF(summary))
                .Returns(bytes);

            var result = _controller.GetPerformanceSummary(PortfolioId, from, to, "pdf");
            var file = result as FileContentResult;

            Assert.IsNotNull(file);
            Assert.AreEqual("application/pdf", file.ContentType);
            Assert.AreEqual("portfolio_performance_summary.pdf", file.FileDownloadName);
            CollectionAssert.AreEqual(bytes, file.FileContents);

            _portfolioReportExportServiceMock.Verify(
                s => s.ExportToPDF(summary),
                Times.Once);
        }

        [TestMethod]
        public void GetPerformanceSummary_WhenFormatIsUnsupported_ReturnsBadRequest()
        {
            var from = new DateOnly(2026, 1, 1);
            var to = new DateOnly(2026, 10, 1);
            var summary = new PortfolioPerformanceSummary();
            byte[] bytes = { 4, 5, 6 };

            _portfoliosServiceMock
                .Setup(s => s.GeneratePerformanceSummary(PortfolioId, from, to))
                .Returns(summary);

            var result = _controller.GetPerformanceSummary(PortfolioId, from, to, "json");
            var badRequest = result as BadRequestObjectResult;

            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);

            _portfolioReportExportServiceMock.Verify(
                s => s.ExportToCsv(It.IsAny<PortfolioPerformanceSummary>()),
                Times.Never);

            _portfolioReportExportServiceMock.Verify(
               s => s.ExportToExcel(It.IsAny<PortfolioPerformanceSummary>()),
               Times.Never);

            _portfolioReportExportServiceMock.Verify(
               s => s.ExportToPDF(It.IsAny<PortfolioPerformanceSummary>()),
               Times.Never);
        }

        [TestMethod]
        public void GetPerformanceSummary_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var from = new DateOnly(2026, 1, 1);
            var to = new DateOnly(2026, 10, 1);

            _portfoliosServiceMock
                .Setup(s => s.GeneratePerformanceSummary(PortfolioId, from, to))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetPerformanceSummary(PortfolioId, from, to);
            var error = result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WhenDataExists_ReturnsOkWithReports()
        {
            PriceTrendPeriod? period = null;
            PriceTrendResolution? resolution = null;
            string? securityCode = "ALK";
            var reports = new List<SecurityPriceTrendReport>();

            _transactionsServiceMock
                .Setup(s => s.GetSecuritiesPriceTrendReport(UserId, period, resolution, securityCode))
                .Returns(reports);

            var result = _controller.GetSecuritiesPriceTrendReport(period, resolution, securityCode);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(reports, ok.Value);

            _transactionsServiceMock.Verify(
                s => s.GetSecuritiesPriceTrendReport(UserId, period, resolution, securityCode),
                Times.Once);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            PriceTrendPeriod? period = null;
            PriceTrendResolution? resolution = null;
            string? securityCode = "ALK";

            _transactionsServiceMock
                .Setup(s => s.GetSecuritiesPriceTrendReport(UserId, period, resolution, securityCode))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetSecuritiesPriceTrendReport(period, resolution, securityCode);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GenerateSecuritiesPriceTrendReport_WhenSuccessful_ReturnsPdfFile()
        {
            PriceTrendPeriod? period = null;
            PriceTrendResolution? resolution = null;
            string? securityCode = "ALK";
            var reports = new List<SecurityPriceTrendReport>();
            byte[] pdf = { 10, 11, 12 };

            _transactionsServiceMock
                .Setup(s => s.GetSecuritiesPriceTrendReport(UserId, period, resolution, securityCode))
                .Returns(reports);

            _securityPriceTrendReportServiceMock
                .Setup(s => s.GenerateSecurityPriceTrendReport(reports))
                .Returns(pdf);

            var result = _controller.GenerateSecuritiesPriceTrendReport(period, resolution, securityCode);

            var file = result as FileContentResult;

            Assert.IsNotNull(file);
            Assert.AreEqual("application/pdf", file.ContentType);
            Assert.AreEqual("SecuritiesPriceTrendReport.pdf", file.FileDownloadName);
            CollectionAssert.AreEqual(pdf, file.FileContents);

            _securityPriceTrendReportServiceMock.Verify(
                s => s.GenerateSecurityPriceTrendReport(reports),
                Times.Once);
        }

        [TestMethod]
        public void GenerateSecuritiesPriceTrendReport_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            PriceTrendPeriod? period = null;
            PriceTrendResolution? resolution = null;
            string? securityCode = "ALK";

            _transactionsServiceMock
                .Setup(s => s.GetSecuritiesPriceTrendReport(UserId, period, resolution, securityCode))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GenerateSecuritiesPriceTrendReport(period, resolution, securityCode);
            var error = result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }
    }
}