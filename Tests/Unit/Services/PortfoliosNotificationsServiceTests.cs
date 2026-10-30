using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class PortfoliosNotificationsServiceTests
    {
        private Mock<IPortfoliosNotificationsRepository> _portfoliosNotificationsRepositoryMock = null!;
        private Mock<IUsersRepository> _usersRepositoryMock = null!;
        private Mock<ITransactionsService> _transactionsServiceMock = null!;
        private Mock<IEmailService> _emailServiceMock = null!;
        private IPortfoliosNotificationsService _portfoliosNotificationsService = null!;

        [TestInitialize]
        public void Setup()
        {
            _portfoliosNotificationsRepositoryMock = new Mock<IPortfoliosNotificationsRepository>();
            _usersRepositoryMock = new Mock<IUsersRepository>();
            _transactionsServiceMock = new Mock<ITransactionsService>();
            _emailServiceMock = new Mock<IEmailService>();
            _portfoliosNotificationsService = new PortfoliosNotificationsService(_portfoliosNotificationsRepositoryMock.Object,
                _transactionsServiceMock.Object, _usersRepositoryMock.Object, _emailServiceMock.Object);
        }

        [TestMethod]
        public void FindByPortfolioId_NotificationExists_ReturnsMappedNotification()
        {
            var notification = new IzvestuvanjaPortfolija
            {
                PortfolioId = 1,
                Ovozmozeno = true,
                Frekvencija = "Monthly",
                PoslednoIsprateno = DateTime.UtcNow
            };

            _portfoliosNotificationsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1))
                .Returns(notification);

            var result = _portfoliosNotificationsService.FindByPortfolioId(1);

            Assert.AreEqual(1, result.PortfolioId);
            Assert.IsTrue(result.IsEnabled);
            Assert.AreEqual("Monthly", result.Frequency);

            _portfoliosNotificationsRepositoryMock.Verify(
                r => r.GetByPortfolioId(1),
                Times.Once);
        }

        [TestMethod]
        public void FindByPortfolioId_NotificationDoesNotExist_ReturnsDefaultNotification()
        {
            _portfoliosNotificationsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1))
                .Returns((IzvestuvanjaPortfolija?)null);

            var result = _portfoliosNotificationsService.FindByPortfolioId(1);

            Assert.AreEqual(1, result.PortfolioId);
            Assert.IsFalse(result.IsEnabled);
            Assert.AreEqual("Weekly", result.Frequency);
        }

        [TestMethod]
        public void Update_InvalidFrequency_ThrowsValidationException()
        {
            var request = new UpdatePortfolioNotification
            {
                PortfolioId = 1,
                IsEnabled = true,
                Frequency = "Daily"
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _portfoliosNotificationsService.Update(request));

            Assert.AreEqual(
                "FREQUENCY_VALIDATION_REQUIRED", exception.Code);

            _portfoliosNotificationsRepositoryMock.Verify(
                r => r.GetByPortfolioId(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_NotificationDoesNotExist_AddsNotification()
        {
            var request = new UpdatePortfolioNotification
            {
                PortfolioId = 1,
                IsEnabled = true,
                Frequency = "Weekly"
            };

            _portfoliosNotificationsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1))
                .Returns((IzvestuvanjaPortfolija?)null);

            var result = _portfoliosNotificationsService.Update(request);

            Assert.AreEqual(1, result.PortfolioId);
            Assert.IsTrue(request.IsEnabled);
            Assert.AreEqual("Weekly", result.Frequency);

            _portfoliosNotificationsRepositoryMock.Verify(
                r => r.Add(It.Is<IzvestuvanjaPortfolija>(n =>
                n.PortfolioId == 1 &&
                n.Ovozmozeno == true &&
                n.Frekvencija == "Weekly" &&
                n.PoslednoIsprateno == null)),
                Times.Once);

            _portfoliosNotificationsRepositoryMock.Verify(
                r => r.Update(It.IsAny<IzvestuvanjaPortfolija>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_NotificationExists_UpdatesNotification()
        {
            var existingNotification = new IzvestuvanjaPortfolija
            {
                PortfolioId = 1,
                Ovozmozeno = false,
                Frekvencija = "Weekly",
                PoslednoIsprateno = DateTime.UtcNow.AddDays(-1)
            };

            var request = new UpdatePortfolioNotification
            {
                PortfolioId = 1,
                IsEnabled = true,
                Frequency = "Monthly"
            };

            _portfoliosNotificationsRepositoryMock
                .Setup(r => r.GetByPortfolioId(1))
                .Returns(existingNotification);

            _portfoliosNotificationsRepositoryMock
                .Setup(r => r.Update(It.IsAny<IzvestuvanjaPortfolija>()))
                .Returns(existingNotification);

            var result = _portfoliosNotificationsService.Update(request);

            Assert.AreEqual(1, result.PortfolioId);
            Assert.IsTrue(result.IsEnabled);
            Assert.AreEqual("Monthly", result.Frequency);

            _portfoliosNotificationsRepositoryMock.Verify(
                r => r.Update(It.Is<IzvestuvanjaPortfolija>(n =>
                n.PortfolioId == 1 &&
                n.Ovozmozeno == true &&
                n.Frekvencija == "Monthly")),
                Times.Once);

            _portfoliosNotificationsRepositoryMock.Verify(
                r => r.Add(It.IsAny<IzvestuvanjaPortfolija>()),
                Times.Never);
        }

        [TestMethod]
        public void SendScheduledNotifications_NoUsers_DoesNothing()
        {
            _usersRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Korisnici>());

            _portfoliosNotificationsService.SendScheduledNotifications();

            _portfoliosNotificationsRepositoryMock.Verify(
                r => r.GetByUserId(It.IsAny<int>()),
                Times.Never);

            _emailServiceMock.Verify(
                e => e.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void SendScheduledNotifications_NoEnabledNotifications_DoesNotSendEmail()
        {
            var user = new Korisnici
            {
                Id = 1,
                Email = "test@example.com"
            };

            var notifications = new List<IzvestuvanjaPortfolija>
            {
                new IzvestuvanjaPortfolija
                {
                    PortfolioId = 10,
                    Ovozmozeno = false,
                    Frekvencija = "Weekly"
                }
            };

            _usersRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Korisnici> { user });

            _portfoliosNotificationsRepositoryMock
                .Setup(r => r.GetByUserId(1))
                .Returns(notifications);

            _portfoliosNotificationsService.SendScheduledNotifications();

            _emailServiceMock.Verify(
                r => r.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);

            _transactionsServiceMock.Verify(
                t => t.GetAnalytics(
                    It.IsAny<int>(),
                    It.IsAny<bool>()),
                Times.Never);
        }

        [TestMethod]
        public void SendScheduledNotifications_EnabledNotificationWithNoPreviousSend_SendsEmail()
        {
            var user = new Korisnici
            {
                Id = 1,
                Email = "test@example.com"
            };

            var notification = new IzvestuvanjaPortfolija
            {
                PortfolioId = 10,
                Ovozmozeno = true,
                Frekvencija = "Weekly",
                PoslednoIsprateno = null
            };

            var realAnalytics = new PortfolioAnalytics
            {
                TotalRevenue = 5000m,
                TotalExpenses = 1000m,
                Balance = 4000m
            };

            var simulatedAnalytics = new PortfolioAnalytics
            {
                TotalRevenue = 6000m,
                TotalExpenses = 1500m,
                Balance = 4500m
            };

            _usersRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Korisnici> { user });

            _portfoliosNotificationsRepositoryMock
                .Setup(r => r.GetByUserId(1))
                .Returns(new List<IzvestuvanjaPortfolija> { notification });

            _transactionsServiceMock
                .Setup(r => r.GetAnalytics(10, true))
                .Returns(realAnalytics);

            _transactionsServiceMock
               .Setup(r => r.GetAnalytics(10, false))
               .Returns(simulatedAnalytics);

            _portfoliosNotificationsService.SendScheduledNotifications();

            _emailServiceMock.Verify(
                e => e.SendEmail(
                    "test@example.com",
                    "Your Portfolio Performance Summary",
                    It.Is<string>(body =>
                    body.Contains("Portfolio Id:") &&
                    body.Contains("Real Transactions") &&
                    body.Contains("Simulated Transactions"))),
                Times.Once);

            _transactionsServiceMock.Verify(
                t => t.GetAnalytics(10, true),
                Times.Once);

            _transactionsServiceMock.Verify(
               t => t.GetAnalytics(10, false),
               Times.Once);

            _portfoliosNotificationsRepositoryMock.Verify(
                r => r.Update(It.Is<IzvestuvanjaPortfolija>(n =>
                n.PortfolioId == 10 &&
                n.PoslednoIsprateno.HasValue)),
                Times.Once);
        }

        [TestMethod]
        public void SendScheduledNotifications_RecentlySentNotification_DoesNotSendEmail()
        {
            var user = new Korisnici
            {
                Id = 1,
                Email = "test@example.com"
            };

            var notification = new IzvestuvanjaPortfolija
            {
                PortfolioId = 10,
                Ovozmozeno = true,
                Frekvencija = "Weekly",
                PoslednoIsprateno = DateTime.UtcNow.AddDays(-2)
            };

            _usersRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Korisnici> { user });

            _portfoliosNotificationsRepositoryMock
                .Setup(r => r.GetByUserId(1))
                .Returns(new List<IzvestuvanjaPortfolija> { notification });

            _portfoliosNotificationsService.SendScheduledNotifications();

            _emailServiceMock.Verify(
                e => e.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);

            _transactionsServiceMock.Verify(
                t => t.GetAnalytics(
                    It.IsAny<int>(),
                    It.IsAny<bool>()),
                Times.Never);

            _portfoliosNotificationsRepositoryMock.Verify(
                r => r.Update(It.IsAny<IzvestuvanjaPortfolija>()),
                Times.Never);
        }
    }
}
