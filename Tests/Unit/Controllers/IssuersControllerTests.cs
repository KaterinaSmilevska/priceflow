using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;
namespace Tests.Unit.Controllers
{
    [TestClass]
    public class IssuersControllerTests
    {
        private Mock<IIssuersService> _serviceMock = null!;
        private IssuersController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IIssuersService>();
            _controller = new IssuersController(_serviceMock.Object);
        }

        [TestMethod]
        public void GetAll_WhenIssuersExist_ReturnsOkWithIssuers()
        {
            var issuers = new List<Issuer>();

            _serviceMock
                .Setup(s => s.FindAll())
                .Returns(issuers);

            var result = _controller.GetAll();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(issuers, ok.Value);

            _serviceMock.Verify(
                s => s.FindAll(),
                Times.Once);
        }

        [TestMethod]
        public void GetAll_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.FindAll())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetAll();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }
    }
}
