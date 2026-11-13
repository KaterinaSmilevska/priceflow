using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Tests.Unit.Services
{
    [TestClass]
    public class AgentUsageServiceTests
    {
        private Mock<IAgentUsageRepository> _repositoryMock = null!;
        private AgentUsageService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IAgentUsageRepository>();
            _service = new AgentUsageService(_repositoryMock.Object);
        }

        [TestMethod]
        public void FindByUserIdAndDate_ExistingUsage_ReturnsUsage()
        {
            int userId = 1;
            DateOnly date = new DateOnly(2026, 9, 29);

            var usage = new AgentKoristenje
            {
                KorisnikId = userId,
                Datum = date,
                BrojPoraki = 3
            };

            _repositoryMock
                .Setup(r => r.GetByUserIdAndDate(userId, date))
                .Returns(usage);

            var result = _service.FindByUserIdAndDate(userId, date);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.KorisnikId);
            Assert.AreEqual(date, result.Datum);
            Assert.AreEqual(3, result.BrojPoraki);
        }

        [TestMethod]
        public void FindByUserIdAndDate_NoUsage_ReturnsNull()
        {
            int userId = 1;
            DateOnly date = new DateOnly(2026, 9, 29);

            _repositoryMock
                .Setup(r => r.GetByUserIdAndDate(userId, date))
                .Returns((AgentKoristenje?)null);

            var result = _service.FindByUserIdAndDate(userId, date);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Add_CreatUsageWithOneMessage()
        {
            int userId = 1;
            DateOnly date = new DateOnly(2026, 9, 29);

            var usage = new AgentKoristenje
            {
                KorisnikId = userId,
                Datum = date,
                BrojPoraki = 1
            };

            _repositoryMock
                .Setup(r => r.Add(It.IsAny<AgentKoristenje>()))
                .Returns(usage);

            var result = _service.Add(userId, date);


            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.KorisnikId);
            Assert.AreEqual(date, result.Datum);
            Assert.AreEqual(1, result.BrojPoraki);

            _repositoryMock.Verify(
                r => r.Add(It.Is<AgentKoristenje>( u=> 
                u.KorisnikId == userId &&
                u.Datum == date &&
                u.BrojPoraki == 1)),
                Times.Once);
        }

        [TestMethod]
        public void Increment_ExistingUsage_IncreasesMessageCount()
        {
            int userId = 1;
            DateOnly date = new DateOnly(2026, 9, 29);

            var usage = new AgentKoristenje
            {
                KorisnikId = userId,
                Datum = date,
                BrojPoraki = 3
            };

            _repositoryMock
                .Setup(r => r.GetByUserIdAndDate(userId, date))
                .Returns(usage);

            _repositoryMock
                .Setup(r => r.Update(It.IsAny<AgentKoristenje>()))
                .Returns((AgentKoristenje u) => u);

            var result = _service.Increment(userId, date);

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.BrojPoraki);

            _repositoryMock.Verify(
                r => r.Update(It.Is<AgentKoristenje>(u =>
                u.KorisnikId == userId &&
                u.Datum == date &&
                u.BrojPoraki == 4)),
                Times.Once);
        }

        [TestMethod]
        public void Increment_NoUsage_ThrowsNotFoundException()
        {
            int userId = 1;
            DateOnly date = new DateOnly(2026, 9, 29);

            _repositoryMock
                .Setup(r => r.GetByUserIdAndDate(userId, date))
                .Returns((AgentKoristenje?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.Increment(userId, date));

            _repositoryMock.Verify(
                r => r.Update(It.IsAny<AgentKoristenje>()),
                Times.Never);
        }
    }
}
