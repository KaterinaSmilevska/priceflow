using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Agents;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;
using System.Security.Claims;

namespace Tests.Unit.Controllers
{
    [TestClass]
    public class SecuritiesControllerTests
    {
        private Mock<ISecuritiesService> _serviceMock = null!;
        private Mock<IOrchestratorAgent> _orchestratorAgentMock = null!;
        private SecuritiesController _controller = null!;

        private const int UserId = 1;
        private const int SecurityId = 1;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<ISecuritiesService>();
            _orchestratorAgentMock = new Mock<IOrchestratorAgent>();
            _controller = new SecuritiesController(_serviceMock.Object, _orchestratorAgentMock.Object);

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
        public void GetById_WhenSecurityExists_ReturnsOkWithSecurity()
        {
            var security = new Security();

            _serviceMock
                .Setup(s => s.FindById(SecurityId))
                .Returns(security);

            var result = _controller.GetById(UserId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(security, ok.Value);

            _serviceMock.Verify(
                s => s.FindById(SecurityId), 
                Times.Once);
        }

        [TestMethod]
        public void GetById_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.FindById(SecurityId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetById(SecurityId);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetByCode_WhenSecurityExists_ReturnsOkWithSecurity()
        {
            const string code = "ALK";
            var security = new Security();

            _serviceMock
                .Setup(s => s.FindByCode(code))
                .Returns(security);

            var result = _controller.GetByCode(code);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(security, ok.Value);

            _serviceMock.Verify(
                s => s.FindByCode(code), 
                Times.Once);
        }

        [TestMethod]
        public void GetByCode_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const string code = "ALK";

            _serviceMock
                .Setup(s => s.FindByCode(code))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetByCode(code);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetAll_WhenSecuritiesExist_ReturnsOkWithSecurities()
        {
            var seucirites = new List<Security>
            {
                new Security(),
                new Security()
            };

            _serviceMock
                .Setup(s => s.FindAll())
                .Returns(seucirites);

            var result = _controller.GetAll();
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(seucirites, ok.Value);

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

        [TestMethod]
        public void GetSecurityCode_WhenSecurityExists_ReturnsOkWithCode()
        {
            const string code = "ALK";

            _serviceMock
                .Setup(s => s.FindSecurityCode(SecurityId))
                .Returns(code);

            var result = _controller.GetSecurityCode(SecurityId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreEqual(code, ok.Value);
        }

        [TestMethod]
        public void GetSecurityCode_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.FindSecurityCode(SecurityId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetSecurityCode(SecurityId);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetTotalNumShares_WhenSecurityExists_ReturnsOkWithTotalShares()
        {
            const string code = "ALK";
            const int totalShares = 1000;

            _serviceMock
                .Setup(s => s.FindTotalNumShares(code))
                .Returns(totalShares);

            var result = _controller.GetTotalNumShares(code);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(totalShares, ok.Value);
        }

        [TestMethod]
        public void GetTotalNumShares_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const string code = "ALK";

            _serviceMock
                .Setup(s => s.FindTotalNumShares(code))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetTotalNumShares(code);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Add_WhenSuccessful_ReturnsOkWithAddedSecurity()
        {
            var request = new AddSecurityRequest();
            var security = new Security();

            _serviceMock
                .Setup(s => s.Add(request))
                .Returns(security);

            var result = _controller.Add(request);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(security, ok.Value);

            _serviceMock.Verify(
                s => s.Add(request),
                Times.Once);
        }

        [TestMethod]
        public void Add_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var request = new AddSecurityRequest();

            _serviceMock
                .Setup(s => s.Add(request))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Add(request);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Update_WhenIdsMatch_ReturnsOkWithUpdatedSecurity()
        {
            var request = new UpdateSecurity
            {
                Id = SecurityId
            };
            var security = new Security();

            _serviceMock
                .Setup(s => s.Update(SecurityId, request))
                .Returns(security);

            var result = _controller.Update(SecurityId, request);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(security, ok.Value);

            _serviceMock.Verify(
                s => s.Update(SecurityId, request),
                Times.Once);
        }

        [TestMethod]
        public void Update_WhenIdsDoNotMatch_ReturnsBadRequest()
        {
            var request = new UpdateSecurity
            {
                Id = 2
            };

            var result = _controller.Update(SecurityId, request);
            var badRequest = result.Result as BadRequestObjectResult;

            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);

            _serviceMock.Verify(
                s => s.Update(It.IsAny<int>(),
                It.IsAny<UpdateSecurity>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var request = new UpdateSecurity
            {
                Id = SecurityId
            };

            _serviceMock
                .Setup(s => s.Update(SecurityId, request))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Update(SecurityId, request);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Delete_WhenSuccessful_ReturnsOkWithDeletedSecurity()
        {
            var security = new Security();

            _serviceMock
                .Setup(s => s.Delete(SecurityId))
                .Returns(security);

            var result = _controller.Delete(SecurityId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(security, ok.Value);

            _serviceMock.Verify(
                s => s.Delete(SecurityId),
                Times.Once);
        }

        [TestMethod]
        public void Delete_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _serviceMock
                .Setup(s => s.Delete(SecurityId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Delete(SecurityId);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetLatestPrices_WhenPricesExist_ReturnsOkWithPrices()
        {
            const string code = "ALK";
            var date = new DateTime(2026, 10, 10);
            var prices = new SecurityDailyPrices();

            _serviceMock
                .Setup(s => s.GetLatestPrices(code, date))
                .Returns(prices);

            var result = _controller.GetLatestPrices(code, date);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(prices, ok.Value);

            _serviceMock.Verify(
                s =>  s.GetLatestPrices(code, date),
                Times.Once);
        }

        [TestMethod]
        public void GetLatestPrices_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const string code = "ALK";
            var date = new DateTime(2026, 10, 10);

            _serviceMock
                .Setup(s => s.GetLatestPrices(code, date))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetLatestPrices(code, date);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void SearchByCode_WhenMatchesExist_ReturnsOkWithSecurities()
        {
            const string searchTerm = "AL";
            var securities = new List<Security>
            {
                new Security()
            };

            _serviceMock
                .Setup(s => s.SearchByCode(searchTerm))
                .Returns(securities);

            var result = _controller.SearchByCode(searchTerm);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(securities, ok.Value);

            _serviceMock.Verify(
                s => s.SearchByCode(searchTerm),
                Times.Once);
        }

        [TestMethod]
        public void SearchByCode_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            const string searchTerm = "AL";

            _serviceMock
                .Setup(s => s.SearchByCode(searchTerm))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.SearchByCode(searchTerm);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public async Task AskSecuritiesAgent_WhenQuestionIsBlank_ReturnsBadRequest()
        {
            var request = new SecuritiesController.ChatRequest(null, " ");

            var result = await _controller.AskSecuritiesAgent(request);

            var badRequest = result as ObjectResult;

            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);
        }

        [TestMethod]
        public async Task AskSecuritiesAgent_WhenSuccessful_ReturnsOkWithAnswer()
        {
            var request = new SecuritiesController.ChatRequest(null, "What is ALK?");
            var response = new SecuritiesAgentResponse
            (
                10,
                "ALK is Alkaloid AD Skopje."
            );

            _orchestratorAgentMock
                .Setup(a => a.AskAsync(UserId, request.ConversationId, request.Question))
                .ReturnsAsync(response);

            var result = await _controller.AskSecuritiesAgent(request);
            var ok = result as OkObjectResult;

            Assert.IsNotNull(ok);

            var conversationId = ok.Value!.GetType()
                .GetProperty("conversationId")!.GetValue(ok.Value);

            var answer = ok.Value.GetType()
                .GetProperty("answer")!.GetValue(ok.Value);

            Assert.AreEqual(response.ConversationId, conversationId);
            Assert.AreEqual(response.Answer, answer);
        }

        [TestMethod]
        public async Task AskSecuritiesAgent_WhenDailyLimitIsReached_ReturnsError()
        {
            var request = new SecuritiesController.ChatRequest(null, "What is ALK?");

            _orchestratorAgentMock
                .Setup(a => a.AskAsync(UserId, request.ConversationId, request.Question))
                .ThrowsAsync(new InvalidOperationException("Daily limit reached."));

            var result = await _controller.AskSecuritiesAgent(request);
            var response = result as ObjectResult;

            Assert.IsNotNull(response);
            Assert.AreEqual(StatusCodes.Status429TooManyRequests, response.StatusCode);
        }
    }
}
