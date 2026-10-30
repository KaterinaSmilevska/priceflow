using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Unit.Services
{
    [TestClass]
    public class IssuersServiceTests
    {
        private Mock<IIssuersRepository> _repositoryMock = null!;
        private IssuersService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<IIssuersRepository>();
            _service = new IssuersService(_repositoryMock.Object);
        }

        [TestMethod]
        public void FindAll_ReturnsMappedIssuers()
        {
            var issuers = new List<Izdavachi>
            {
                new Izdavachi
                {
                    Id = 1,
                    Ime = "Issuer A"
                },
                new Izdavachi
                {
                    Id = 2,
                    Ime = "Issuer B"
                }
            };

            _repositoryMock
                .Setup(r => r.GetAll())
                .Returns(issuers);

            var result = _service.FindAll().ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Issuer A", result[0].Name);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("Issuer B", result[1].Name);

            _repositoryMock.Verify(
                r => r.GetAll(),
                Times.Once);
        }

        [TestMethod]
        public void FindAll_NoIssuers_ReturnsEmptyCollection()
        {
            _repositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Izdavachi>());

            var result = _service.FindAll().ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);

            _repositoryMock.Verify(
                r => r.GetAll(),
                Times.Once);
        }
    }
}
