using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace Tests.Unit.Controllers
{
    [TestClass]
    public class BrokersControllerTests
    {
        private Mock<IBrokersService> _serviceMock = null!;
        private BrokersController _controller = null!;

        private const int BrokerId = 1;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IBrokersService>();
            _controller = new BrokersController(_serviceMock.Object);
        }

        [TestMethod]
        public void GetAll_WhenBrokersExist_ReturnsOkWithBrokers()
        {
            var brokers = new List<BrokerResponse>();

            _serviceMock
                .Setup(s => s.FindAll())
                .Returns(brokers);

            var result = _controller.GetAll();

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(brokers, ok.Value);

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

            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [TestMethod]
        public void AddBroker_WhenValidBroker_ReturnsOkWithAddedBroker()
        {
            var request = new AddBrokerRequest();
            var response = new BrokerResponse();

            _serviceMock
                .Setup(s => s.Add(request))
                .Returns(response);

            var result = _controller.AddBroker(request);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(response, ok.Value);

            _serviceMock.Verify(
                s => s.Add(request),
                Times.Once);
        }

        [TestMethod]
        public void AddBroker_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var request = new AddBrokerRequest();

            _serviceMock
                .Setup(s => s.Add(request))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.AddBroker(request);
            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [TestMethod]
        public void UpdateBroker_WhenIdsMatch_ReturnsOkWithUpdatedBroker()
        {
            var request = new UpdateBrokerRequest
            {
                Id = BrokerId
            };

            var response = new BrokerResponse();

            _serviceMock
                .Setup(s => s.Update(BrokerId, request))
                .Returns(response);

            var result = _controller.UpdateBroker(BrokerId, request);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(response, ok.Value);

            _serviceMock.Verify(
                s => s.Update(BrokerId, request),
                Times.Once);
        }

        [TestMethod]
        public void UpdateBroker_WhenIdsDoNotMatch_ReturnsBadRequest()
        {
            var request = new UpdateBrokerRequest
            {
                Id = 2
            };

            var result = _controller.UpdateBroker(BrokerId, request);
            var badRequest = result.Result as BadRequestObjectResult;

            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);

            _serviceMock.Verify(
                s => s.Update(It.IsAny<int>(),
                It.IsAny<UpdateBrokerRequest>()),
                Times.Never);
        }

        [TestMethod]
        public void UpdateBroker_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var request = new UpdateBrokerRequest
            {
                Id = BrokerId
            };

            _serviceMock
                .Setup(s => s.Update(BrokerId, request))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.UpdateBroker(BrokerId, request);
            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [TestMethod]
        public void DeleteBroker_WhenBrokerExists_ReturnsOkWithDeletedBroker()
        {
            var response = new BrokerResponse();

            _serviceMock
                .Setup(s => s.Delete(BrokerId))
                .Returns(response);

            var result = _controller.DeleteBroker(BrokerId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(response, ok.Value);

            _serviceMock.Verify(
                s => s.Delete(BrokerId),
                Times.Once);
        }

        [TestMethod]
        public void DeleteBroker_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.Delete(BrokerId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.DeleteBroker(BrokerId);
            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
        }
    }
}
