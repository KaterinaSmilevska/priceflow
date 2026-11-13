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
    public class ThresholdControllerTests
    {
        private Mock<IThresholdService> _serviceMock = null!;
        private ThresholdController _controller = null!;

        private const int UserId = 1;
        private const int ThresholdId = 10;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IThresholdService>();
            _controller = new ThresholdController(_serviceMock.Object);

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
        public void GetUserThresholds_WhenThresholdsExist_ReturnsOkWithThresholds()
        {
            var thresholds = new List<ThresholdResponse>
            {
                new ThresholdResponse()
            };

            _serviceMock
                .Setup(s => s.GetUserThresholds(UserId))
                .Returns(thresholds);

            var result = _controller.GetUserThresholds();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(thresholds, ok.Value);

            _serviceMock.Verify(
                s => s.GetUserThresholds(UserId),
                Times.Once);
        }

        [TestMethod]
        public void GetUserThresholds_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.GetUserThresholds(UserId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetUserThresholds();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetOwned_WhenSecutiesExist_ReturnsOkWithSecurities()
        {
            var securities = new List<OwnedSecurity>
            {
                new OwnedSecurity()
            };

            _serviceMock
                .Setup(s => s.GetOwnedSecurities(UserId))
                .Returns(securities);

            var result = _controller.GetOwned();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _serviceMock.Verify(
                s => s.GetOwnedSecurities(UserId),
                Times.Once);
        }

        [TestMethod]
        public void GetOwned_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.GetOwnedSecurities(UserId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetOwned();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Add_WhenSuccessful_ReturnsOkWithCreatedThreshold()
        {
            var request = new AddThresholdRequest();
            var threshold = new ThresholdResponse();

            _serviceMock
                .Setup(s => s.Add(UserId, request))
                .Returns(threshold);

            var result = _controller.Add(request);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(threshold, ok.Value);

            _serviceMock.Verify(
                s => s.Add(UserId, request),
                Times.Once);
        }

        [TestMethod]
        public void Add_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var request = new AddThresholdRequest();

            _serviceMock
                .Setup(s => s.Add(UserId, request))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Add(request);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Update_WhenSuccessful_ReturnsOkWithUpdatedThreshold()
        {
            var request = new UpdateThresholdRequest();
            var threshold = new ThresholdResponse();

            _serviceMock
               .Setup(s => s.Update(UserId, ThresholdId, request))
               .Returns(threshold);

            var result = _controller.Update(ThresholdId, request);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(threshold, ok.Value);

            _serviceMock.Verify(
                s => s.Update(UserId, ThresholdId, request),
                Times.Once);
        }

        [TestMethod]
        public void Update_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var request = new UpdateThresholdRequest();

            _serviceMock
                .Setup(s => s.Update(UserId, ThresholdId, request))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Update(ThresholdId, request);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Delete_WhenSuccessful_ReturnsOkWithDeletedThreshold()
        {
            var threshold = new ThresholdResponse();

            _serviceMock
                .Setup(s => s.Delete(UserId, ThresholdId))
                .Returns(threshold);

            var result = _controller.Delete(ThresholdId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(threshold, ok.Value);

            _serviceMock.Verify(
                s => s.Delete(UserId, ThresholdId),
                Times.Once);
        }

        [TestMethod]
        public void Delete_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.Delete(UserId, ThresholdId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Delete(ThresholdId);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }
    }
}
