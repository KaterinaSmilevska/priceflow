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
    public class PriceChangeNotificationControllerTests
    {
        private Mock<IPriceChangeNotificationService> _service = null!;
        private PriceChangeNotificationController _controller = null!;

        private const int UserId = 1;

        [TestInitialize]
        public void Setup()
        {
            _service = new Mock<IPriceChangeNotificationService>();
            _controller = new PriceChangeNotificationController(_service.Object);

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
        public void GetByUserId_WhenNotificationsExist_ReturnsOkWithNotifications()
        {
            var notifications = new List<PriceChangeNotificationResponse>();

            _service
                .Setup(s => s.GenerateNotifications());

            _service
                .Setup(s => s.GetUserNotifications(UserId))
                .Returns(notifications);

            var result = _controller.GetByUserId();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(notifications, ok.Value);

            _service.Verify(
                s => s.GenerateNotifications(),
                Times.Once);

            _service.Verify(
                s => s.GetUserNotifications(UserId),
                Times.Once);
        }

        [TestMethod]
        public void GetByUserId_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.GenerateNotifications())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetByUserId();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetUnreadCount_WhenSuccessful_ReturnsOkWithCount()
        {
            const int unreadCount = 5;

            _service
                .Setup(s => s.GetUnreadNotificationCount(UserId))
                .Returns(unreadCount);

            var result = _controller.GetUnreadCount();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreEqual(unreadCount, ok.Value);
        }

        [TestMethod]
        public void GetUnreadCount_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.GetUnreadNotificationCount(UserId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetUnreadCount();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void MarkAsRead_WhenSuccessful_ReturnsOk()
        {
            const int notificationId = 10;

            _service
                .Setup(s => s.MarkNotificationAsRead(UserId, notificationId));

            var result = _controller.MarkAsRead(notificationId);

            Assert.IsInstanceOfType(result, typeof(OkResult));

            var ok = (OkResult)result;
            Assert.AreEqual(200, ok.StatusCode);

            _service.Verify(
                s => s.MarkNotificationAsRead(UserId, notificationId),
                Times.Once);
        }

        [TestMethod]
        public void MarkAsRead_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const int notificationId = 10;

            _service
                .Setup(s => s.MarkNotificationAsRead(UserId, notificationId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.MarkAsRead(notificationId);
            var error = result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }
    }
}
