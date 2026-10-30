using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Unit.Controllers
{
    [TestClass]
    public class TypeSecurityControllerTests
    {
        private Mock<ITypeSecurityService> _serviceMock = null!;
        private TypeSecurityController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<ITypeSecurityService>();
            _controller = new TypeSecurityController(_serviceMock.Object);
        }

        [TestMethod]
        public void GetAll_WhenTypesExist_ReturnsOkWithTypes()
        {
            var types = new List<TypeSecurity>
            {
                new TypeSecurity(),
                new TypeSecurity()
            };

            _serviceMock
                .Setup(s => s.FindAll())
                .Returns(types);

            var result = _controller.GetAll();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(types, ok.Value);

            _serviceMock.Verify(
                s => s.FindAll(),
                Times.Once);
        }

        [TestMethod]
        public void GetAll_WhenNoTypesExist_ReturnsOkWithEmptyCollection()
        {
            var types = new List<TypeSecurity>();

            _serviceMock
                .Setup(s => s.FindAll())
                .Returns(types);

            var result = _controller.GetAll();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(types, ok.Value);

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
