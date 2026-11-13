using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class AgentConversationsRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private AgentConversationsRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new AgentConversationsRepository(_context);

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
                UpdatedAt = new DateTime(2026, 10, 12, 11, 0, 0)
            };

            var conversation3 = new AgentRazgovori
            {
                Id = 3,
                KorisnikId = 1,
                CreatedAt = new DateTime(2026, 10, 11, 12, 0, 0),
                UpdatedAt = new DateTime(2026, 10, 11, 13, 0, 0)
            };

            conversation1.AgentPoraki.Add(new AgentPoraki
            {
                Id = 1,
                RazgovorId = 1,
                Uloga = "user",
                Sodrzina = "What is ALK?",
                CreatedAt = new DateTime(2026, 10, 10, 10, 1, 0)
            });

            conversation1.AgentPoraki.Add(new AgentPoraki
            {
                Id = 2,
                RazgovorId = 1,
                Uloga = "assistant",
                Sodrzina = "ALK is Alkaloid AD Skopje.",
                CreatedAt = new DateTime(2026, 10, 10, 10, 2, 0)
            });

            _context.Korisnici.AddRange(user1, user2);
            _context.AgentRazgovori.AddRange(conversation1, conversation2, conversation3);

            _context.SaveChanges();

            _context.ChangeTracker.Clear();
        }

        [TestMethod]
        public void GetById_ExistingConversation_ReturnsConversation()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(1, result.KorisnikId);
        }

        [TestMethod]
        public void GetById_NonExistingConversation_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetById_ExistingConversation_IncludesMessages()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.AgentPoraki.Count);
            Assert.IsTrue(result.AgentPoraki.Any(
                p => p.Sodrzina == "What is ALK?"));
            Assert.IsTrue(result.AgentPoraki.Any(
                p => p.Sodrzina == "ALK is Alkaloid AD Skopje."));
        }

        [TestMethod]
        public void GetByIdAndUserId_MatchingUser_ReturnsConversation()
        {
            var result = _repository.GetByIdAndUserId(1, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(1, result.KorisnikId);
            Assert.AreEqual(2, result.AgentPoraki.Count);
        }

        [TestMethod]
        public void GetByIdAndUserId_WrongUser_ReturnsNull()
        {
            var result = _repository.GetByIdAndUserId(1, 2);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByIdAndUserId_NonExistingConversation_ReturnsNull()
        {
            var result = _repository.GetByIdAndUserId(1000, 1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByUserId_ExistingUser_ReturnsOnlyUsersConversations()
        {
            var result = _repository.GetByUserId(1).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(c => c.KorisnikId == 1));
        }

        [TestMethod]
        public void GetByUserId_ConversationsExist_ReturnsDescendingUpdatedAt()
        {
            var result = _repository.GetByUserId(1).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(3, result[0].Id);
            Assert.AreEqual(1, result[1].Id);
            Assert.IsTrue(result[0].UpdatedAt > result[1].UpdatedAt);
        }

        [TestMethod]
        public void GetByUserId_UserHasNoConversations_ReturnsEmptyCollection()
        {
            var result = _repository.GetByUserId(1000);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Add_ValidConversation_SavesAndReturnsConversation()
        {
            var conversation = new AgentRazgovori
            {
                KorisnikId = 1,
                CreatedAt = new DateTime(2026, 10, 15, 10, 0, 0),
                UpdatedAt = new DateTime(2026, 10, 15, 10, 0, 0)
            };
            var result = _repository.Add(conversation);

            Assert.IsNotNull(result);
            Assert.AreEqual(conversation.Id, result.Id);
            Assert.IsTrue(conversation.Id > 0);
            Assert.IsTrue(_context.AgentRazgovori.Any(
                c => c.Id == conversation.Id));
        }

        [TestMethod]
        public void Update_ExistingConversation_PersistsChanges()
        {
            var conversation = _context.AgentRazgovori.First(c => c.Id == 1);

            conversation.UpdatedAt = new DateTime(2026, 10, 15, 15, 0, 0);

            var result = _repository.Update(conversation);

            Assert.AreSame(conversation, result);
            Assert.AreEqual(new DateTime(2026, 10, 15, 15, 0, 0),
                _context.AgentRazgovori
                .AsNoTracking()
                .First(c => c.Id == 1)
                .UpdatedAt);
        }

        [TestMethod]
        public void Delete_ExistingConversation_RemovesConversation()
        {
            var conversation = _context.AgentRazgovori
                .Include(c => c.AgentPoraki)
                .First(c => c.Id == 1);

            var result = _repository.Delete(conversation);

            Assert.AreSame(conversation, result);
            Assert.IsFalse(_context.AgentRazgovori
                .Any(c => c.Id == 1));
        }

        [TestMethod]
        public void Delete_ExistingConversation_RemovesMessages()
        {
            var conversation = _context.AgentRazgovori
                .Include(c => c.AgentPoraki)
                .First(c => c.Id == 1);

            Assert.AreEqual(2, conversation.AgentPoraki.Count);

            _repository.Delete(conversation);

            Assert.IsFalse(_context.AgentPoraki
                .Any(p => p.RazgovorId == 1));
        }
    }
}
