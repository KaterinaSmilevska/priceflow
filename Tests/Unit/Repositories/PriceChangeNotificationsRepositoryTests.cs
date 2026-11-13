using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class PriceChangeNotificationsRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private PriceChangeNotificationsRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new PriceChangeNotificationsRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            _context.IzvestuvanjaPromenaCena.AddRange(
                CreateNotification(1, 10, 1, DateTime.Today.AddDays(-2), "Messsage 1", false),
                CreateNotification(2, 10, 2, DateTime.Today, "Messsage 2", true),
                CreateNotification(3, 10, 1, DateTime.Today.AddDays(-1), "Messsage 3", false),
                CreateNotification(4, 20, 1, DateTime.Today.AddDays(-3), "Messsage 4", false)
                );
            _context.SaveChanges();
        }

        private IzvestuvanjaPromenaCena CreateNotification(int id, int userId, int securityId, DateTime tradingDate, 
            string message, bool read)
        {
            return new IzvestuvanjaPromenaCena
            {
                Id = id,
                KorisnikId = userId,
                Hvid = securityId,
                DatumTrguvanje = tradingDate,
                Poraka = message,
                Procitano = read
            };
        }

        [TestMethod]
        public void GetById_WhenNotificationExist_ReturnsNotification()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(10, result.KorisnikId);
            Assert.AreEqual(1, result.Hvid);
        }

        [TestMethod]
        public void GetById_WhenNotificationDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByUserId_ReturnsUsersNotificationsOrderedByTradingDateDescending()
        {
            var result = _repository.GetByUserId(10).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(n => n.KorisnikId == 10));

            CollectionAssert.AreEqual(
                new[] { 2, 3, 1 },
                result.Select(n => n.Id).ToArray());
        }

        [TestMethod]
        public void GetByUserId_WhenUserHasNoNotifications_ReturnsEmptyCollection()
        {
            var result = _repository.GetByUserId(1000).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetUnreadNotificationCount_ReturnsOnlyUnreadNotificationsForUser()
        {
            var result = _repository.GetUnreadNotificationCount(10);

            Assert.AreEqual(2, result);
        }

        [TestMethod]
        public void GetUnreadNotificationCount_WhenNoUnreadNotifications_ReturnsZero()
        {
            var result = _repository.GetUnreadNotificationCount(1000);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void MarkNotificationAsRead_SetsReadStatusAndSavesChanges()
        {
            var notification = _context.IzvestuvanjaPromenaCena.Single(n => n.Id == 1);

            Assert.IsFalse(notification.Procitano);

            _repository.MarkNotificationAsRead(notification);

            Assert.IsTrue(notification.Procitano);

            _context.Entry(notification).State = EntityState.Detached;

            var saved = _context.IzvestuvanjaPromenaCena.Single(n => n.Id == 1);

            Assert.IsTrue(saved.Procitano);
        }

        [TestMethod]
        public void ExistsForUserAndSecurityAndDate_WhenMatchingNotificationExists_ReturnsTrue()
        {
            var result = _repository.ExistsForUserAndSecurityAndDate(10, 1, DateTime.Today.AddDays(-2));

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ExistsForUserAndSecurityAndDate_WhenUserDoesNotMatch_ReturnsFalse()
        {
            var result = _repository.ExistsForUserAndSecurityAndDate(1000, 1, DateTime.Today.AddDays(-2));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ExistsForUserAndSecurityAndDate_WhenSecurityDoesNotMatch_ReturnsFalse()
        {
            var result = _repository.ExistsForUserAndSecurityAndDate(10, 1000, DateTime.Today.AddDays(-2));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ExistsForUserAndSecurityAndDate_WhenDateDoesNotMatch_ReturnsFalse()
        {
            var result = _repository.ExistsForUserAndSecurityAndDate(10, 1, DateTime.Today.AddDays(-5));

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void Add_SavesAndReturnsNotification()
        {
            var notification = CreateNotification(10, 10, 1, DateTime.Today, "New notification", false);

            var result = _repository.Add(notification);

            Assert.AreSame(notification, result);

            var saved = _context.IzvestuvanjaPromenaCena.SingleOrDefault(n => n.Id == 10);

            Assert.IsNotNull(saved);
            Assert.AreEqual(10, saved.KorisnikId);
            Assert.AreEqual(1, saved.Hvid);
            Assert.AreEqual(DateTime.Today, saved.DatumTrguvanje);
            Assert.IsFalse(saved.Procitano);
        }
    }
}
