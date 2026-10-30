using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace Tests.Unit.Controllers
{
    [TestClass]
    public class RolesControllerTests
    {
        private Mock<IRolesService> _service = null!;
        private RolesController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _service = new Mock<IRolesService>();
            _controller = new RolesController(_service.Object);
        }

        [TestMethod]
        public void GetRole_WhenRoleExists_ReturnsOkWithRole()
        {
            const string roleName = "Инвеститор";
            var role = new Role();

            _service
                .Setup(s => s.FindByName(roleName))
                .Returns(role);

            var result = _controller.GetRole(roleName);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(role, ok.Value);

            _service.Verify(
                s => s.FindByName(roleName),
                Times.Once);
        }

        [TestMethod]
        public void GetRole_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const string roleName = "Инвеститор";

            _service
                .Setup(s => s.FindByName(roleName))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetRole(roleName);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetRoleNames_WhenRolesExist_ReturnsOkWithRoleNames()
        {
            var roleNames = new List<string>
            {
                "Инвеститор",
                "Администратор"
            };

            _service
                .Setup(s => s.FindNames())
                .Returns(roleNames);

            var result = _controller.GetRoleNames();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(roleNames, ok.Value);

            _service.Verify(
                s => s.FindNames(),
                Times.Once);
        }

        [TestMethod]
        public void GetRoleNames_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _service
                .Setup(s => s.FindNames())
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetRoleNames();
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }
    }
}
