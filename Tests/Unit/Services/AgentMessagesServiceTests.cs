using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class AgentMessagesServiceTests
    {
        private Mock<IAgentMessagesRepository> _repositoryMock = null!;
        private AgentMessagesService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IAgentMessagesRepository>();
            _service = new AgentMessagesService(_repositoryMock.Object);
        }

        [TestMethod]
        public void FindByConversationId_ReturnsMessages()
        {
            var messages = new List<AgentPoraki>
            {
                new AgentPoraki
                {
                    Id = 1,
                    RazgovorId = 10,
                    Uloga = "user",
                    Sodrzina = "What is ALK?",
                    CreatedAt = DateTime.Now
                },
                   new AgentPoraki
                {
                    Id = 2,
                    RazgovorId = 10,
                    Uloga = "assistant",
                    Sodrzina = "ALK is Alkaloid AD Skopje.",
                    CreatedAt = DateTime.Now
                }
            };

            _repositoryMock
                .Setup(r => r.GetByConversationId(10))
                .Returns(messages);

            var result = _service.FindByConversationId(10).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreSame(messages[0], result[0]);
            Assert.AreSame(messages[1], result[1]);

            _repositoryMock.Verify(
                r => r.GetByConversationId(10),
                Times.Once);
        }

        [TestMethod]
        public void FindByConversationId_NoMessages_ReturnsEmptyCollection()
        {
            _repositoryMock
                .Setup(r => r.GetByConversationId(10))
                .Returns(new List<AgentPoraki>());

            var result = _service.FindByConversationId(10).ToList();

            Assert.AreEqual(0, result.Count);

            _repositoryMock.Verify(
                r => r.GetByConversationId(10),
                Times.Once);
        }

        [TestMethod]
        public void FindByConversationIdAndUserId_ReturnsMappedMessages()
        {
            var createdAt1 = DateTime.UtcNow.AddMinutes(-5);
            var createdAt2 = DateTime.UtcNow;

            var messages = new List<AgentPoraki>
            {
                new AgentPoraki
                {
                    Id = 1,
                    RazgovorId = 10,
                    Uloga = "user",
                    Sodrzina = "What is ALK?",
                    CreatedAt = createdAt1
                },
                new AgentPoraki
                {
                    Id = 2,
                    RazgovorId = 10,
                    Uloga = "assistant",
                    Sodrzina = "ALK is Alkaloid AD Skopje.",
                    CreatedAt = createdAt2
                },
            };

            _repositoryMock
                .Setup(r => r.GetByConversationIdAndUserId(10, 1))
                .Returns(messages);

            var result = _service.FindByConversationIdAndUserId(10, 1).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("user", result[0].Role);
            Assert.AreEqual("What is ALK?", result[0].Content);
            Assert.AreEqual(createdAt1, result[0].CreatedAt);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("assistant", result[1].Role);
            Assert.AreEqual("ALK is Alkaloid AD Skopje.", result[1].Content);
            Assert.AreEqual(createdAt2, result[1].CreatedAt);

            _repositoryMock.Verify(
                r => r.GetByConversationIdAndUserId(10, 1),
                Times.Once);
        }

        [TestMethod]
        public void FindByConversationIdAndUserId_NoMessages_ReturnsEmptyCollection()
        {
            _repositoryMock
             .Setup(r => r.GetByConversationIdAndUserId(10, 1))
             .Returns(new List<AgentPoraki>());

            var result = _service.FindByConversationIdAndUserId(10, 1).ToList();

            Assert.AreEqual(0, result.Count);

            _repositoryMock.Verify(
                r => r.GetByConversationIdAndUserId(10, 1),
                Times.Once);
        }

        [TestMethod]
        public void Add_CreatesAndReturnsMessage()
        {
            var before = DateTime.UtcNow;

            var addedMessage = new AgentPoraki
            {
                Id = 1,
                RazgovorId = 10,
                Uloga = "user",
                Sodrzina = "What is the latest ALK price?",
                CreatedAt = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(r => r.Add(It.IsAny<AgentPoraki>()))
                .Returns(addedMessage);

            var result = _service.Add(10, "user", "What is the latest ALK price?");

            var after = DateTime.UtcNow;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(10, result.RazgovorId);
            Assert.AreEqual("user", result.Uloga);
            Assert.AreEqual("What is the latest ALK price?", result.Sodrzina);

            _repositoryMock.Verify(
                r => r.Add(It.Is<AgentPoraki>(m =>
                m.RazgovorId == 10 &&
                m.Uloga == "user" &&
                m.Sodrzina == "What is the latest ALK price?" &&
                m.CreatedAt >= before &&
                m.CreatedAt <= after)),
                Times.Once);
        }
    }
}
