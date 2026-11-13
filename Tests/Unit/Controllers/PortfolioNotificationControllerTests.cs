using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace Tests.Unit.Controllers
{
    [TestClass]
    public class PortfolioNotificationControllerTests
    {
        private Mock<IPortfoliosNotificationsService> _serviceMock = null!;
        private PortfolioNotificationController _controller = null!;

        private const int PortfolioId = 1;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IPortfoliosNotificationsService>();
            _controller = new PortfolioNotificationController(_serviceMock.Object);
        }

        [TestMethod]
        public void GetByPortfolioId_WhenNotificationExists_ReturnsOkWithNotification()
        {
            var notification = new PortfolioNotification();

            _serviceMock
                .Setup(s => s.FindByPortfolioId(PortfolioId))
                .Returns(notification);

            var result = _controller.GetByPortfolioId(PortfolioId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(notification, ok.Value);

            _serviceMock.Verify(
                s => s.FindByPortfolioId(PortfolioId),
                Times.Once);
        }

        [TestMethod]
        public void GetByPortfolioId_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.FindByPortfolioId(PortfolioId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetByPortfolioId(PortfolioId);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Update_WhenSuccessful_ReturnsOkWithUpdatedNotification()
        {
            var request = new UpdatePortfolioNotification();
            var response = new PortfolioNotification();

            _serviceMock
                .Setup(s => s.Update(request))
                .Returns(response);

            var result = _controller.Update(request);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(response, ok.Value);

            _serviceMock.Verify(
                s => s.Update(request),
                Times.Once);
        }

        [TestMethod]
        public void UpdateNotification_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var request = new UpdatePortfolioNotification();

            _serviceMock
                .Setup(s => s.Update(request))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Update(request);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }
    }
}
