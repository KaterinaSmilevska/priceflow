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
    public class TypeSecurityServiceTests
    {
        private Mock<ITypeSecurityRepository> _repositoryMock = null!;
        private TypeSecurityService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _repositoryMock = new Mock<ITypeSecurityRepository>();
            _service = new TypeSecurityService(_repositoryMock.Object);
        }

        [TestMethod]
        public void FindAll_WhenTypesExist_ReturnsMappedTypes()
        {
            var types = new List<TipHv>
            {
                new TipHv
                {
                    Id = 1,
                    Ime = "Акции"
                },
                new TipHv
                {
                    Id = 2,
                    Ime = "Обврзници"
                }
            };

            _repositoryMock
                .Setup(r => r.GetAll())
                .Returns(types);

            var result = _service.FindAll().ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Акции", result[0].Name);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("Обврзници", result[1].Name);

            _repositoryMock.Verify(
                r => r.GetAll(),
                Times.Once);
        }

        [TestMethod]
        public void FindAll_WhenNoTypesExist_ReturnsEmptyCollection()
        {
            _repositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<TipHv>());

            var result = _service.FindAll().ToList();

            Assert.AreEqual(0, result.Count);

            _repositoryMock.Verify(
                r => r.GetAll(),
                Times.Once);
        }

        [TestMethod]
        public void FindAll_WhenOneTypeExists_ReturnsSingleMappedType()
        {
            var types = new List<TipHv>
            {
                new TipHv
                {
                    Id = 1,
                    Ime = "Акции"
                }
            };

            _repositoryMock
                .Setup(r => r.GetAll())
                .Returns(types);

            var result = _service.FindAll().ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Акции", result[0].Name);
        }
    }
}
