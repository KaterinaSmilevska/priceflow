using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class PortfoliosNotificationsRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private PortfoliosNotificationsRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new PortfoliosNotificationsRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var portfolio1 = new Portfolija
            {
                Id = 1,
                Ime = "Portfolio 1",
                KorisnikId = 10
            };

            var portfolio2 = new Portfolija
            {
                Id = 2,
                Ime = "Portfolio 2",
                KorisnikId = 10
            };

            var portfolio3 = new Portfolija
            {
                Id = 3,
                Ime = "Portfolio 3",
                KorisnikId = 20
            };

            _context.Portfolija.AddRange(portfolio1, portfolio2, portfolio3);
            _context.SaveChanges();
        }

        [TestMethod]
        public void GetByUserId_ReturnsNotificationsForUsersPortfolios()
        {
            _context.IzvestuvanjaPortfolija.AddRange(
                new IzvestuvanjaPortfolija
                {
                    Id = 1,
                    PortfolioId = 1,
                    Frekvencija = "Monthly"
                },
                new IzvestuvanjaPortfolija
                {
                    Id = 2,
                    PortfolioId = 2,
                    Frekvencija = "Weekly"
                },
                new IzvestuvanjaPortfolija
                {
                    Id = 3,
                    PortfolioId = 3,
                    Frekvencija = "Monthly"
                });

            _context.SaveChanges();

            var result = _repository.GetByUserId(10).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(x => x.Portfolio != null));
            Assert.IsTrue(result.All(x => x.Portfolio.KorisnikId == 10));
        }

        [TestMethod]
        public void GetByUserId_WhenUserHasNoNotifications_ReturnsEmptyCollection()
        {
            var result = _repository.GetByUserId(1000).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetByPortfolioId_WhenNotificationExists_ReturnsNotification()
        {
            var notification = new IzvestuvanjaPortfolija
            {
                Id = 1,
                PortfolioId = 1,
                Frekvencija = "Weekly"
            };

            _context.IzvestuvanjaPortfolija.Add(notification);
            _context.SaveChanges();

            var result = _repository.GetByPortfolioId(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(1, result.PortfolioId);
        }

        [TestMethod]
        public void GetByPortfolioId_WhenNotificationDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetByPortfolioId(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Add_SavesAndReturnsNotification()
        {
            var notification = new IzvestuvanjaPortfolija
            {
                Id = 1,
                PortfolioId = 1,
                Frekvencija = "Monthly"
            };

            var result = _repository.Add(notification);

            Assert.AreSame(notification, result);

            var saved = _context.IzvestuvanjaPortfolija.SingleOrDefault(x => x.Id == 1);

            Assert.IsNotNull(saved);
            Assert.AreEqual(1, saved.PortfolioId);
        }

        [TestMethod]
        public void Update_ModifiesAndReturnsNotification()
        {
            var notification = new IzvestuvanjaPortfolija
            {
                Id = 1,
                PortfolioId = 1,
                Frekvencija = "Monthly"
            };

            _context.IzvestuvanjaPortfolija.Add(notification);
            _context.SaveChanges();

            _context.Entry(notification).State = EntityState.Detached;

            var updated = new IzvestuvanjaPortfolija
            {
                Id = 1,
                PortfolioId = 2,
                Frekvencija = "Weekly"
            };

            var result = _repository.Update(updated);

            Assert.AreSame(updated, result);

            var saved = _context.IzvestuvanjaPortfolija.Single(x => x.Id == 1);

            Assert.AreEqual(2, saved.PortfolioId);
        }
    }
}
