using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;
using System.Security.Claims;

namespace Tests.Unit.Controllers
{
    [TestClass]
    public class AgentConversationsControllerTests
    {
        private Mock<IAgentConversationsService> _agentConversationsServiceMock = null!;
        private Mock<IAgentMessagesService> _agentMessagesServiceMock = null!;
        private AgentConversationsController _controller = null!;

        private const int UserId = 10;
        private const int ConversationId = 20;

        [TestInitialize]
        public void Setup()
        {
            _agentConversationsServiceMock = new Mock<IAgentConversationsService>();
            _agentMessagesServiceMock = new Mock<IAgentMessagesService>();
            _controller = new AgentConversationsController(_agentConversationsServiceMock.Object,
                _agentMessagesServiceMock.Object);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, UserId.ToString())
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
        public void GetAll_WhenServiceReturnsConversations_ReturnsOkWithConversations()
        {
            var conversations = new List<AgentConversationResponse>();

            _agentConversationsServiceMock
                .Setup(s => s.FindByUserId(UserId))
                .Returns(conversations);

            var result = _controller.GetAll();

            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreSame(conversations, okResult.Value);

            _agentConversationsServiceMock.Verify(
                s => s.FindByUserId(UserId),
                Times.Once);
        }

        [TestMethod]
        public void GetAll_WhenServiceReturnsEmptyCollection_ReturnsOk()
        {
            _agentConversationsServiceMock
                .Setup(s => s.FindByUserId(UserId))
                .Returns(new List<AgentConversationResponse>());

            var result = _controller.GetAll();

            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public void GetAll_WhenServiceThrowsPriceFlowException_ReturnsMappedError()
        {
            var exception = new NotFoundException(
                "CONVERSATIONS_NOT_FOUND",
                "Conversations not found.");

            _agentConversationsServiceMock
                .Setup(s => s.FindByUserId(UserId))
                .Throws(exception);

            var result = _controller.GetAll();

            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(exception.StatusCode, objectResult.StatusCode);

            var error = objectResult.Value as ErrorResponse;

            Assert.IsNotNull(error);
            Assert.AreEqual(exception.Code, error.Code);
            Assert.AreEqual(exception.Message, error.Message);
        }

        [TestMethod]
        public void GetAll_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _agentConversationsServiceMock
                .Setup(s => s.FindByUserId(UserId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetAll();

            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var error = objectResult.Value as ErrorResponse;

            Assert.IsNotNull(error);
            Assert.AreEqual("INTERNAL_SERVER_ERROR", error.Code);
            Assert.AreEqual("An unexpected error occured.", error.Message);
        }

        [TestMethod]
        public void GetMessages_WhenServiceReturnsMessages_ReturnsOkWithMessages()
        {
            var messages = new List<AgentMessageResponse>();

            _agentMessagesServiceMock
                .Setup(s => s.FindByConversationIdAndUserId(ConversationId, UserId))
                .Returns(messages);

            var result = _controller.GetMessages(ConversationId);

            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreSame(messages, okResult.Value);

            _agentMessagesServiceMock.Verify(
                s => s.FindByConversationIdAndUserId(
                    ConversationId, UserId),
                Times.Once);
        }

        [TestMethod]
        public void GetMessages_WhenServiceReturnsEmptyCollection_ReturnsOk()
        {
            _agentMessagesServiceMock
                .Setup(s => s.FindByConversationIdAndUserId(ConversationId, UserId))
                .Returns(new List<AgentMessageResponse>());

            var result = _controller.GetMessages(ConversationId);

            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
        }

        [TestMethod]
        public void GetMessages_WhenServiceThrowsPriceFlowException_ReturnsMappedError()
        {
            var exception = new NotFoundException(
               "CONVERSATIONS_NOT_FOUND",
               "Conversations not found.");

            _agentMessagesServiceMock
                .Setup(s => s.FindByConversationIdAndUserId(ConversationId, UserId))
                .Throws(exception);

            var result = _controller.GetMessages(ConversationId);

            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(exception.StatusCode, objectResult.StatusCode);

            var error = objectResult.Value as ErrorResponse;

            Assert.IsNotNull(error);
            Assert.AreEqual(exception.Code, error.Code);
            Assert.AreEqual(exception.Message, error.Message);
        }

        [TestMethod]
        public void GetMessages_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _agentMessagesServiceMock
                .Setup(s => s.FindByConversationIdAndUserId(ConversationId, UserId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetMessages(ConversationId);

            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var error = objectResult.Value as ErrorResponse;

            Assert.IsNotNull(error);
            Assert.AreEqual("INTERNAL_SERVER_ERROR", error.Code);
            Assert.AreEqual("An unexpected error occured.", error.Message);
        }

        [TestMethod]
        public void Delete_WhenServiceReturnsConversation_ReturnsOkWithConversation()
        {
            var conversation = new AgentConversationResponse();

            _agentConversationsServiceMock
                .Setup(s => s.Delete(ConversationId, UserId))
                .Returns(conversation);

            var result = _controller.Delete(ConversationId);

            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreSame(conversation, okResult.Value);

            _agentConversationsServiceMock.Verify(
                s => s.Delete(ConversationId, UserId),
                Times.Once);
        }

        [TestMethod]
        public void Delete_WhenServiceThrowsPriceFlowException_ReturnsMappedError()
        {
            var exception = new NotFoundException(
              "CONVERSATIONS_NOT_FOUND",
              "Conversations not found.");

            _agentConversationsServiceMock
                .Setup(s => s.Delete(ConversationId, UserId))
                .Throws(exception);

            var result = _controller.Delete(ConversationId);

            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(exception.StatusCode, objectResult.StatusCode);

            var error = objectResult.Value as ErrorResponse;

            Assert.IsNotNull(error);
            Assert.AreEqual(exception.Code, error.Code);
            Assert.AreEqual(exception.Message, error.Message);
        }

        [TestMethod]
        public void Delete_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _agentConversationsServiceMock
             .Setup(s => s.Delete(ConversationId, UserId))
             .Throws(new Exception("Unexpected error"));

            var result = _controller.Delete(ConversationId);

            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);

            var error = objectResult.Value as ErrorResponse;

            Assert.IsNotNull(error);
            Assert.AreEqual("INTERNAL_SERVER_ERROR", error.Code);
            Assert.AreEqual("An unexpected error occured.", error.Message);
        }
    }
}
