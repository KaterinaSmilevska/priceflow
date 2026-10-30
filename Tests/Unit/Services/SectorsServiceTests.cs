using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class SectorsServiceTests
    {
        private Mock<ISectorsRepository> _sectorRepositoryMock = null!;
        private SectorsService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _sectorRepositoryMock = new Mock<ISectorsRepository>();
            _service = new SectorsService(_sectorRepositoryMock.Object);
        }

        [TestMethod]
        public void FindAll_SectorsExist_ReturnsMappedSectors()
        {
            var sectors = new List<Sektori>
            {
                new Sektori
                {
                    Id = 1,
                    Ime = "Banking"
                },
                new Sektori
                {
                    Id = 2,
                    Ime = "Industry"
                }
            };

            _sectorRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(sectors);

            var result = _service.FindAll().ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Banking", result[0].Name);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("Industry", result[1].Name);

            _sectorRepositoryMock.Verify(
                r => r.GetAll(),
                Times.Once);
        }

        [TestMethod]
        public void FindAll_NoSectors_ReturnsEmptyList()
        {
            _sectorRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Sektori>());

            var result = _service.FindAll().ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);

            _sectorRepositoryMock.Verify(
                r => r.GetAll(),
                Times.Once);
        }
    }
}
