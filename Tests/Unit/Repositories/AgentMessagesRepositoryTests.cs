using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class AgentMessagesRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private AgentMessagesRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new AgentMessagesRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var user1 = new Korisnici
            {
                Id = 1,
                Ime = "User1",
                Username = "user1",
                PasswordHash = new byte[] { 1, 2, 3 },
                Email = "user1@test.com",
                IsEmailVerified = true,
                DateModified = new DateTime(2026, 10, 1)
            };

            var user2 = new Korisnici
            {
                Id = 2,
                Ime = "User2",
                Username = "user2",
                PasswordHash = new byte[] { 4, 5, 6 },
                Email = "user2@test.com",
                IsEmailVerified = true,
                DateModified = new DateTime(2026, 10, 1)
            };

            var conversation1 = new AgentRazgovori
            {
                Id = 1,
                KorisnikId = 1,
                CreatedAt = new DateTime(2026, 10, 10, 10, 0, 0),
                UpdatedAt = new DateTime(2026, 10, 10, 11, 0, 0)
            };

            var conversation2 = new AgentRazgovori
            {
                Id = 2,
                KorisnikId = 2,
                CreatedAt = new DateTime(2026, 10, 11, 10, 0, 0),
                UpdatedAt = new DateTime(2026, 10, 11, 11, 0, 0)
            };

            var message1 = new AgentPoraki
            {
                Id = 1,
                RazgovorId = 1,
                Uloga = "user",
                Sodrzina = "What is ALK?",
                CreatedAt = new DateTime(2026, 10, 10, 10, 1, 0),
                Razgovor = conversation1
            };

            var message2 = new AgentPoraki
            {
                Id = 2,
                RazgovorId = 1,
                Uloga = "assistant",
                Sodrzina = "ALK is Alkaloid AD Skopje.",
                CreatedAt = new DateTime(2026, 10, 10, 10, 2, 0),
                Razgovor = conversation1
            };

            var message3 = new AgentPoraki
            {
                Id = 3,
                RazgovorId = 2,
                Uloga = "user",
                Sodrzina = "What is KMB?",
                CreatedAt = new DateTime(2026, 10, 11, 10, 1, 0),
                Razgovor = conversation2
            };

            _context.Korisnici.AddRange(user1, user2);
            _context.AgentRazgovori.AddRange(conversation1, conversation2);
            _context.AgentPoraki.AddRange(message1, message2, message3);

            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }

        [TestMethod]
        public void GetByConversationId_ExistingConversation_ReturnsMessages()
        {
            var result = _repository.GetByConversationId(1).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(m => m.RazgovorId == 1));
        }

        [TestMethod]
        public void GetByConversationId_MessagesExist_ReturnsChronologicalOrder()
        {
            var result = _repository.GetByConversationId(1).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(2, result[1].Id);
            Assert.IsTrue(result[0].CreatedAt <= result[1].CreatedAt);
        }

        [TestMethod]
        public void GetByConversationId_NoMessages_ReturnsEmptyCollection()
        {
            var result = _repository.GetByConversationId(1000).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetByConversationIdAndUserId_MatchingUser_ReturnsMessages()
        {
            var result = _repository.GetByConversationIdAndUserId(1, 1).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(m => m.RazgovorId == 1));
        }

        [TestMethod]
        public void GetByConversationIdAndUserId_WrongUser_ReturnsEmptyCollection()
        {
            var result = _repository.GetByConversationIdAndUserId(1, 2).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetByConversationIdAndUserId_NonExistingConversation_ReturnsEmptyCollection()
        {
            var result = _repository.GetByConversationIdAndUserId(1000, 1).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Add_ValidMessage_SavesAndReturnsMessage()
        {
            var message = new AgentPoraki
            {
                RazgovorId = 1,
                Uloga = "user",
                Sodrzina = "What is KMB?",
                CreatedAt = new DateTime(2026, 10, 15, 10, 0, 0)
            };

            var result = _repository.Add(message);

            Assert.IsNotNull(result);
            Assert.AreSame(message, result);
            Assert.IsTrue(message.Id > 0);
            Assert.IsTrue(_context.AgentPoraki.Any(m => m.Id == message.Id));
        }
    }
}
