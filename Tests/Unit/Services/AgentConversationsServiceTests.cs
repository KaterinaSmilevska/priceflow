using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class AgentConversationsServiceTests
    {
        private Mock<IAgentConversationsRepository> _repositoryMock = null!;
        private AgentConversationsService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IAgentConversationsRepository>();
            _service = new AgentConversationsService(_repositoryMock.Object);
        }

        [TestMethod]
        public void FindById_ExistingCoversation_ReturnsConversation()
        {
            var conversation = new AgentRazgovori
            {
                Id = 1,
                KorisnikId = 10,
                CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                UpdatedAt = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(r => r.GetById(1))
                .Returns(conversation);

            var result = _service.FindById(1);

            Assert.AreSame(conversation, result);

            _repositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);
        }

        [TestMethod]
        public void FindById_ConversationNotFound_ThrowsNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((AgentRazgovori?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.FindById(1000));

            _repositoryMock.Verify(
                r => r.GetById(1000),
                Times.Once);
        }

        [TestMethod]
        public void FindByIdAndUserId_ExistingConversation_ReturnsConversation()
        {
            var conversation = new AgentRazgovori
            {
                Id = 1,
                KorisnikId = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(r => r.GetByIdAndUserId(1, 10))
                .Returns(conversation);

            var result = _service.FindByIdAndUserId(1, 10);

            Assert.AreSame(conversation, result);

            _repositoryMock.Verify(
                r => r.GetByIdAndUserId(1, 10),
                Times.Once);
        }

        [TestMethod]
        public void FindByIdAndUserId_ConversationNotFound_ThrowsNotFoundException()
        {
            _repositoryMock
                .Setup(r => r.GetByIdAndUserId(10, 1000))
                .Returns((AgentRazgovori?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.FindByIdAndUserId(10, 1000));

            _repositoryMock.Verify(
                r => r.GetByIdAndUserId(10, 1000),
                Times.Once);
        }

        [TestMethod]
        public void FindByUserId_ExistingConversations_ReturnsMappedResponses()
        {
            var createdAt = DateTime.UtcNow.AddMinutes(-20);
            var updatedAt = DateTime.UtcNow;

            var conversation1 = new AgentRazgovori
            {
                Id = 1,
                KorisnikId = 10,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            var conversation2 = new AgentRazgovori
            {
                Id = 2,
                KorisnikId = 10,
                CreatedAt = createdAt,
                UpdatedAt = updatedAt
            };

            var messages1 = new List<AgentPoraki>
            {
                new AgentPoraki
                {
                    Uloga = "user",
                    Sodrzina = "What is the latest price of ALK?",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-15)
                }
            };

            var messages2 = new List<AgentPoraki>
            {
                new AgentPoraki
                {
                    Uloga = "assistant",
                    Sodrzina = "ALK is Alkaloid.",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-10)
                },
                new AgentPoraki
                {
                    Uloga = "user",
                    Sodrzina = "Show me KMB data.",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5)
                }
            };

            _repositoryMock
                .Setup(r => r.GetByUserId(10))
                .Returns(new List<AgentRazgovori>
                {
                    conversation1,
                    conversation2
                });

            _repositoryMock
                .Setup(r => r.GetById(1))
                .Returns(new AgentRazgovori
                {
                    Id = 1,
                    AgentPoraki = messages1
                });

            _repositoryMock
               .Setup(r => r.GetById(2))
               .Returns(new AgentRazgovori
               {
                   Id = 2,
                   AgentPoraki = messages2
               });

            var result = _service.FindByUserId(10).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("What is the latest price of ALK?", result[0].Title);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("Show me KMB data.", result[1].Title);

            _repositoryMock.Verify(
                r => r.GetByUserId(10),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetById(2),
                Times.Once);
        }

        [TestMethod]
        public void FindByUserId_NoConversations_ReturnsEmptyCollection()
        {
            _repositoryMock
                .Setup(r => r.GetByUserId(10))
                .Returns(new List<AgentRazgovori>());

            var result = _service.FindByUserId(10).ToList();

            Assert.AreEqual(0, result.Count);

            _repositoryMock.Verify(
                r => r.GetByUserId(10),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_CreatesAndReturnsConversation()
        {
            var before = DateTime.UtcNow;

            var addedConversation = new AgentRazgovori
            {
                Id = 1,
                KorisnikId =  10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(r => r.Add(It.IsAny<AgentRazgovori>()))
                .Returns(addedConversation);

            var result = _service.Add(10);

            var after = DateTime.UtcNow;

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(10, result.KorisnikId);

            _repositoryMock.Verify(
                r => r.Add(It.Is<AgentRazgovori>(c =>
                c.KorisnikId == 10 &&
                c.CreatedAt >= before &&
                c.CreatedAt <= after &&
                c.UpdatedAt >= before &&
                c.UpdatedAt <= after)),
                Times.Once);
        }

        [TestMethod]
        public void Update_SetsUpdatedAtAndReturnsConversation()
        {
            var oldUpdatedAt = DateTime.UtcNow.AddHours(-1);

            var conversation = new AgentRazgovori
            {
                Id = 1,
                KorisnikId = 10,
                CreatedAt = DateTime.UtcNow.AddHours(-2),
                UpdatedAt = oldUpdatedAt
            };

            var before = DateTime.UtcNow;

            _repositoryMock
                .Setup(r => r.Update(conversation))
                .Returns(conversation);

            var result = _service.Update(conversation);

            var after = DateTime.UtcNow;

            Assert.AreSame(conversation, result);
            Assert.AreNotEqual(oldUpdatedAt, result.UpdatedAt);
            Assert.IsTrue(result.UpdatedAt >= before);
            Assert.IsTrue(result.UpdatedAt <= after);

            _repositoryMock.Verify(
                r => r.Update(conversation),
                Times.Once);
        }

        [TestMethod]
        public void Delete_ExistingCoversation_DeletesAndReturnsConversation()
        {
            var conversation = new AgentRazgovori
            {
                Id = 1,
                KorisnikId = 10,
                CreatedAt = DateTime.UtcNow.AddMinutes(-10),
                UpdatedAt = DateTime.UtcNow
            };

            var messages = new List<AgentPoraki>
            {
                new AgentPoraki
                {
                    Uloga = "user",
                    Sodrzina = "Show me ALK data.",
                    CreatedAt = DateTime.UtcNow.AddMinutes(-5)
                }
            };

            _repositoryMock
                .Setup(r => r.GetByIdAndUserId(1, 10))
                .Returns(conversation);

            _repositoryMock
              .Setup(r => r.Delete(conversation))
              .Returns(conversation);

            _repositoryMock
              .Setup(r => r.GetById(1))
              .Returns(new AgentRazgovori
              {
                  Id = 1,
                  AgentPoraki = messages
              });

            var result = _service.Delete(1, 10);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Show me ALK data.", result.Title);

            _repositoryMock.Verify(
                r => r.GetByIdAndUserId(1, 10),
                Times.Once);

            _repositoryMock.Verify(
                r => r.Delete(conversation),
                Times.Once);

            _repositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);
        }

        [TestMethod]
        public void FindByUserId_ConversationWithoutUserMessages_UsesNewConversationTitle()
        {
            var conversation = new AgentRazgovori
            {
                Id = 1,
                KorisnikId = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(r => r.GetByUserId(10))
                .Returns(new List<AgentRazgovori> { conversation });

            _repositoryMock
                .Setup(r => r.GetById(1))
                .Returns(new AgentRazgovori
                {
                    Id = 1,
                    AgentPoraki = new List<AgentPoraki>
                    {
                        new AgentPoraki
                        {
                            Uloga = "assistant",
                            Sodrzina = "Assistant response",
                            CreatedAt = DateTime.UtcNow
                        }
                    }
                });

            var result = _service.FindByUserId(10).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("New conversation", result[0].Title);
        }

        [TestMethod]
        public void FindByUserId_LongFirstUserMessage_TruncatesTitle()
        {
            var longMessage = new string('A', 100);

            var conversation = new AgentRazgovori
            {
                Id = 1,
                KorisnikId = 10,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _repositoryMock
                .Setup(r => r.GetByUserId(10))
                .Returns(new List<AgentRazgovori> { conversation });

            _repositoryMock
                .Setup(r => r.GetById(1))
                .Returns(new AgentRazgovori
                {
                    Id = 1,
                    AgentPoraki = new List<AgentPoraki>
                    {
                        new AgentPoraki
                        {
                            Uloga = "user",
                            Sodrzina = longMessage,
                            CreatedAt = DateTime.UtcNow
                        }
                    }
                });

            var result = _service.FindByUserId(10).ToList();

            Assert.AreEqual(63, result[0].Title.Length);
            Assert.AreEqual(new string('A', 60) + "...", result[0].Title);
        }
    }
}
