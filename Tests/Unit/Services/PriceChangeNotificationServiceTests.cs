using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class PriceChangeNotificationServiceTests
    {
        private Mock<IPriceChangeNotificationsRepository> _priceChangeNotificationRepositoryMock = null!;
        private Mock<IDailyTurnoverRepository> _dailyTurnoverRepositoryMock = null!;
        private Mock<IAuthRepository> _authRepositoryMock = null!;
        private Mock<ITransactionsRepository> _transactionsRepositoryMock = null!;
        private Mock<IThresholdRepository> _thresholdRepositoryMock = null!;
        private PriceChangeNotificationService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _priceChangeNotificationRepositoryMock = new Mock<IPriceChangeNotificationsRepository>();
            _dailyTurnoverRepositoryMock = new Mock<IDailyTurnoverRepository>();
            _authRepositoryMock = new Mock<IAuthRepository>();
            _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
            _thresholdRepositoryMock = new Mock<IThresholdRepository>();
            _service = new PriceChangeNotificationService(_priceChangeNotificationRepositoryMock.Object,
                _dailyTurnoverRepositoryMock.Object, _authRepositoryMock.Object, _transactionsRepositoryMock.Object,
                _thresholdRepositoryMock.Object);
        }

        [TestMethod]
        public void GetUserNotifications_ValidUser_ReturnsMappedNotifications()
        {
            var user = new Korisnici
            {
                Id = 1
            };

            var notifications = new List<IzvestuvanjaPromenaCena>
            {
                new IzvestuvanjaPromenaCena
                {
                    Id = 10,
                    KorisnikId = 1,
                    Hvid = 5,
                    ProcentPromena = 4.25m,
                    DatumTrguvanje = new DateTime(2026, 9, 20, 14, 30, 0),
                    Poraka = "ALK changed 4.25%",
                    Procitano = false
                }
            };

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            _priceChangeNotificationRepositoryMock
                .Setup(r => r.GetByUserId(1))
                .Returns(notifications);

            var result = _service.GetUserNotifications(1).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(10, result[0].Id);
            Assert.AreEqual(5, result[0].SecurityId);
            Assert.AreEqual(4.25m, result[0].ChangePercent);
            Assert.AreEqual(new DateTime(2026, 9, 20), result[0].TradingDate);
            Assert.AreEqual("ALK changed 4.25%", result[0].Message);
            Assert.IsFalse(result[0].IsRead);

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.GetByUserId(1),
                Times.Once);
        }

        [TestMethod]
        public void GetUserNotifications_UserNotFound_ThrowsNotFoundException()
        {
            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((Korisnici?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.GetUserNotifications(1));

            Assert.AreEqual("USER_NOT_FOUND", exception.Code);

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.GetByUserId(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public void GetUnreadNotificationCount_ValidUser_ReturnsCount()
        {
            var user = new Korisnici
            {
                Id = 1 
            };

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            _priceChangeNotificationRepositoryMock
                .Setup(r => r.GetUnreadNotificationCount(1))
                .Returns(4);

            var result = _service.GetUnreadNotificationCount(1);

            Assert.AreEqual(4, result);

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.GetUnreadNotificationCount(1),
                Times.Once);
        }

        [TestMethod]
        public void GetUnreadNotificationCount_UserNotFound_ThrowsNotFoundException()
        {
            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((Korisnici?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.GetUnreadNotificationCount(1));

            Assert.AreEqual("USER_NOT_FOUND", exception.Code);
        }

        [TestMethod]
        public void GenerateNotifications_LatestDateDoesNotExist_ReturnsWithoutProcessing()
        {
            var latestDate = new DateTime(2026, 9, 29);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.ExistsForDate(latestDate))
                .Returns(false);

            _service.GenerateNotifications();

            _dailyTurnoverRepositoryMock.Verify(
                r => r.GetByDate(It.IsAny<DateTime>()),
                Times.Never);

            _thresholdRepositoryMock.Verify(
                r => r.GetBySecurityId(It.IsAny<int>()),
                Times.Never);

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.Add(It.IsAny<IzvestuvanjaPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void GenerateNotifications_NullOrZeroPriceChange_SkipsSecurity()
        {
            var latestDate = new DateTime(2026, 9, 29);

            var dailyTurnover = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    ProcentPromena = null
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    ProcentPromena = 0
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            _dailyTurnoverRepositoryMock
               .Setup(r => r.ExistsForDate(latestDate))
               .Returns(true);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDate(latestDate))
                .Returns(dailyTurnover);

            _service.GenerateNotifications();

            _thresholdRepositoryMock.Verify(
                r => r.GetBySecurityId(It.IsAny<int>()),
                Times.Never);

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.Add(It.IsAny<IzvestuvanjaPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void GenerateNotifications_UserOwnsNoShares_DoesNotCreateNotification()
        {
            var latestDate = new DateTime(2026, 9, 29);

            var dailyTurnover = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    ProcentPromena = 5m
                }
            };

            var securitiesPriceChange = new List<HvPromenaCena>
            {
                new HvPromenaCena
                {
                    KorisnikId = 10,
                    Hvid = 1,
                    DolnaGranica = -3m,
                    GornaGranica = 3m
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            _dailyTurnoverRepositoryMock
               .Setup(r => r.ExistsForDate(latestDate))
               .Returns(true);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDate(latestDate))
                .Returns(dailyTurnover);

            _thresholdRepositoryMock
                .Setup(r => r.GetBySecurityId(1))
                .Returns(securitiesPriceChange);

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSharesByUser(10, 1))
                .Returns(0);

            _service.GenerateNotifications();

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.Add(It.IsAny<IzvestuvanjaPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void GenerateNotifications_ThresholdNotReached_DoesNotCreateNotification()
        {
            var latestDate = new DateTime(2026, 9, 29);

            var dailyTurnover = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    ProcentPromena = 2m
                }
            };


            var securitiesPriceChange = new List<HvPromenaCena>
            {
                new HvPromenaCena
                {
                    KorisnikId = 10,
                    Hvid = 1,
                    DolnaGranica = -5m,
                    GornaGranica = 5m
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            _dailyTurnoverRepositoryMock
               .Setup(r => r.ExistsForDate(latestDate))
               .Returns(true);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDate(latestDate))
                .Returns(dailyTurnover);

            _thresholdRepositoryMock
                .Setup(r => r.GetBySecurityId(1))
                .Returns(securitiesPriceChange);

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSharesByUser(10, 1))
                .Returns(100);

            _service.GenerateNotifications();

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.Add(It.IsAny<IzvestuvanjaPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void GenerateNotifications_ThresholdReached_CreatesNotification()
        {
            var latestDate = new DateTime(2026, 9, 29);

            var dailyTurnover = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    ProcentPromena = 6.25m
                }
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var securitiesPriceChange = new List<HvPromenaCena>
            {
                new HvPromenaCena
                {
                    KorisnikId = 10,
                    Hvid = 1,
                    DolnaGranica = -5m,
                    GornaGranica = 5m,
                    Hv = security 
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            _dailyTurnoverRepositoryMock
               .Setup(r => r.ExistsForDate(latestDate))
               .Returns(true);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDate(latestDate))
                .Returns(dailyTurnover);

            _thresholdRepositoryMock
                .Setup(r => r.GetBySecurityId(1))
                .Returns(securitiesPriceChange);

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSharesByUser(10, 1))
                .Returns(100);

            _priceChangeNotificationRepositoryMock
                .Setup(r => r.ExistsForUserAndSecurityAndDate(10, 1, latestDate))
                .Returns(false);

            IzvestuvanjaPromenaCena? addedNotification = null;

            _priceChangeNotificationRepositoryMock
                .Setup(r => r.Add(It.IsAny<IzvestuvanjaPromenaCena>()))
                .Callback<IzvestuvanjaPromenaCena>(n => addedNotification = n)
                .Returns((IzvestuvanjaPromenaCena n) => n);

            _service.GenerateNotifications();

            Assert.IsNotNull(addedNotification);

            Assert.AreEqual(10, addedNotification.KorisnikId);
            Assert.AreEqual(1, addedNotification.Hvid);
            Assert.AreEqual(6.25m, addedNotification.ProcentPromena);
            Assert.AreEqual(latestDate, addedNotification.DatumTrguvanje);
            Assert.IsFalse(addedNotification.Procitano);

            Assert.AreEqual("Security ALK changed 6,25% (Threshold: -5% / 5%)", addedNotification.Poraka);

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.Add(It.IsAny<IzvestuvanjaPromenaCena>()),
                Times.Once);
        }

        [TestMethod]
        public void GenerateNotifications_NotificationAlreadyExists_DoesNotCreateDuplicate()
        {
            var latestDate = new DateTime(2026, 9, 29);

            var dailyTurnover = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    ProcentPromena = -6m
                }
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var securitiesPriceChange = new List<HvPromenaCena>
            {
                new HvPromenaCena
                {
                    KorisnikId = 10,
                    Hvid = 1,
                    DolnaGranica = -5m,
                    GornaGranica = 5m,
                    Hv = security
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetLatestDate())
                .Returns(latestDate);

            _dailyTurnoverRepositoryMock
               .Setup(r => r.ExistsForDate(latestDate))
               .Returns(true);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetByDate(latestDate))
                .Returns(dailyTurnover);

            _thresholdRepositoryMock
                .Setup(r => r.GetBySecurityId(1))
                .Returns(securitiesPriceChange);

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSharesByUser(10, 1))
                .Returns(100);

            _priceChangeNotificationRepositoryMock
                .Setup(r => r.ExistsForUserAndSecurityAndDate(10, 1, latestDate))
                .Returns(true);

            _service.GenerateNotifications();

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.Add(It.IsAny<IzvestuvanjaPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void MarkNotificationAsRead_OwnNotification_MarkAsRead()
        {
            var user = new Korisnici
            {
                Id = 1
            };

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            var notification = new IzvestuvanjaPromenaCena
            {
                Id = 10,
                KorisnikId = 1,
                Hvid = 5,
                Procitano = false
            };

            _priceChangeNotificationRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns(notification);

            _service.MarkNotificationAsRead(1, 10);

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.MarkNotificationAsRead(notification),
                Times.Once);
        }

        [TestMethod]
        public void MarkNotificationAsRead_NotificationNotFound_ThrowsNotFoundException()
        {
            var user = new Korisnici
            {
                Id = 1
            };

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            _priceChangeNotificationRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns((IzvestuvanjaPromenaCena?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.MarkNotificationAsRead(1, 10));

            Assert.AreEqual("NOTIFICATION_NOT_FOUND", exception.Code);

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.MarkNotificationAsRead(It.IsAny<IzvestuvanjaPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void MarkNotificationAsRead_AnotherUsersNotification_ThrowsUnauthorizedException()
        {
            var user = new Korisnici
            {
                Id = 1
            };

            var notification = new IzvestuvanjaPromenaCena
            {
                Id = 10,
                KorisnikId = 2,
                Hvid = 5,
                Procitano = false
            };

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            _priceChangeNotificationRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns(notification);

            var exception = Assert.ThrowsException<UnauthorizedException>(
                () => _service.MarkNotificationAsRead(1, 10));

            Assert.AreEqual("NOTIFICATION_ACCESS_DENIED", exception.Code);

            _priceChangeNotificationRepositoryMock.Verify(
                r => r.MarkNotificationAsRead(It.IsAny<IzvestuvanjaPromenaCena>()),
                Times.Never);
        }

    }
}
