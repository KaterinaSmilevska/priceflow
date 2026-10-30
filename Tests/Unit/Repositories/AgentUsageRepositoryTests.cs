using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class AgentUsageRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private AgentUsageRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new AgentUsageRepository(_context);

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

            _context.Korisnici.AddRange(user1, user2);

            _context.AgentKoristenje.AddRange(
                new AgentKoristenje
                {
                    Id = 1,
                    KorisnikId = 1,
                    Datum = new DateOnly(2026, 10, 15)
                },
                new AgentKoristenje
                {
                    Id = 2,
                    KorisnikId = 1,
                    Datum = new DateOnly(2026, 10, 16)
                },
                new AgentKoristenje
                {
                    Id = 3,
                    KorisnikId = 2,
                    Datum = new DateOnly(2026, 10, 15)
                });

            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }

        [TestMethod]
        public void GetByUserIdAndDate_ExistingRecord_ReturnsUsage()
        {
            var result = _repository.GetByUserIdAndDate(1, new DateOnly(2026, 10, 15));

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(1, result.KorisnikId);
            Assert.AreEqual(new DateOnly(2026, 10, 15), result.Datum);
        }

        [TestMethod]
        public void GetByUserIdAndDate_NonExistingUser_ReturnsNull()
        {
            var result = _repository.GetByUserIdAndDate(1000, new DateOnly(2026, 10, 15));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByUserIdAndDate_NonExistingDate_ReturnsNull()
        {
            var result = _repository.GetByUserIdAndDate(1, new DateOnly(2026, 10, 20));

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByUserIdAndDate_DifferentUserSameDate_ReturnsCorrectRecord()
        {
            var result = _repository.GetByUserIdAndDate(2, new DateOnly(2026, 10, 15));

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Id);
            Assert.AreEqual(2, result.KorisnikId);
        }

        [TestMethod]
        public void Add_ValidUsage_SavesAndReturnsUsage()
        {
            var usage = new AgentKoristenje
            {
                KorisnikId = 1,
                Datum = new DateOnly(2026, 10, 17)
            };

            var result = _repository.Add(usage);

            Assert.IsNotNull(result);
            Assert.AreSame(usage, result);
            Assert.IsTrue(usage.Id > 0);
            Assert.IsTrue(_context.AgentKoristenje.Any(
                a => a.Id == usage.Id &&
                a.KorisnikId == 1 &&
                a.Datum == new DateOnly(2026, 10, 17)));
        }

        [TestMethod]
        public void Update_ExistingUsage_PersistsChanges()
        {
            var usage = _context.AgentKoristenje.First(a => a.Id == 1);

            usage.Datum = new DateOnly(2026, 10, 18);

            var result = _repository.Update(usage);

            Assert.AreSame(usage, result);

            var updated = _context.AgentKoristenje
                .AsNoTracking()
                .First(a => a.Id == 1);

            Assert.AreEqual(new DateOnly(2026, 10, 18), updated.Datum);
        }
    }
}
