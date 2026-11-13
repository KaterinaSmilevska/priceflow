using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class BrokersServiceTests
    {
        private Mock<IBrokersRepository> _brokersRepositoryMock = null!;
        private BrokersService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _brokersRepositoryMock = new Mock<IBrokersRepository>();

            _service = new BrokersService(
                _brokersRepositoryMock.Object);
        }

        [TestMethod]
        public void FindById_ExistingBroker_ReturnsBroker()
        {
            var broker = new Brokeri
            {
                Id = 1,
                Kompanija = "ABC",
                ProcentProvizija = 0.5m
            };

            _brokersRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(broker);

            var result = _service.FindById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("ABC", result.Company);
            Assert.AreEqual(0.5m, result.CommissionPercent);

            _brokersRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);
        }

        [TestMethod]
        public void FindById_BrokerNotFound_ThrowsNotFoundException()
        {
            _brokersRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((Brokeri?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.FindById(1000));

            _brokersRepositoryMock.Verify(
                r => r.GetById(1000),
                Times.Once);
        }

        [TestMethod]
        public void FindByCompany_ExistingBroker_ReturnsBroker()
        {
            var broker = new Brokeri
            {
                Id = 1,
                Kompanija = "ABC",
                ProcentProvizija = 0.5m
            };

            _brokersRepositoryMock
                .Setup(r => r.GetByCompany("ABC"))
                .Returns(broker);

            var result = _service.FindByCompany("ABC");


            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("ABC", result.Company);
            Assert.AreEqual(0.5m, result.CommissionPercent);

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany("ABC"),
                Times.Once);
        }

        [TestMethod]
        public void FindByCompany_EmptyCompany_ThrowsValidationException()
        {
            Assert.ThrowsException<ValidationException>(
                () => _service.FindByCompany(""));

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void FindByCompany_BrokerNotFound_ThrowsNotFoundException()
        {
            _brokersRepositoryMock
              .Setup(r => r.GetByCompany("unknown"))
              .Returns((Brokeri?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.FindByCompany("unknown"));

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany("unknown"),
                Times.Once);
        }

        [TestMethod]
        public void FindAll_ReturnsMappedBrokers()
        {
            var brokers = new List<Brokeri>
            {
                new Brokeri
                {
                    Id = 1,
                    Kompanija = "Broker A",
                    ProcentProvizija = 0.5m
                },
                new Brokeri
                {
                    Id = 2,
                    Kompanija = "Broker B",
                    ProcentProvizija = 0.75m
                },
            };

            _brokersRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(brokers);

            var result = _service.FindAll().ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("Broker A", result[0].Company);
            Assert.AreEqual(0.5m, result[0].CommissionPercent);


            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("Broker B", result[1].Company);
            Assert.AreEqual(0.75m, result[1].CommissionPercent);

            _brokersRepositoryMock.Verify(
                r => r.GetAll(),
                Times.Once);
        }

        [TestMethod]
        public void FindAll_NoBrokers_ReturnsEmptyCollection()
        {
            _brokersRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Brokeri>());

            var result = _service.FindAll().ToList();

            Assert.AreEqual(0, result.Count);

            _brokersRepositoryMock.Verify(
                r => r.GetAll(),
                Times.Once);
        }

        [TestMethod]
        public void Add_ValidRequest_AddsAndReturnsBroker()
        {
            var request = new AddBrokerRequest
            {
                Company = "Broker A",
                CommissionPercent = 0.5m
            };

            var addedBroker = new Brokeri
            {
                Id = 2,
                Kompanija = request.Company,
                ProcentProvizija = request.CommissionPercent
            };

            _brokersRepositoryMock
                .Setup(r => r.GetByCompany("Broker A"))
                .Returns((Brokeri?)null);

            _brokersRepositoryMock
                .Setup(r => r.Add(It.IsAny<Brokeri>()))
                .Returns(addedBroker);

            var result = _service.Add(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Id);
            Assert.AreEqual("Broker A", result.Company);
            Assert.AreEqual(0.5m, result.CommissionPercent);

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany("Broker A"),
                Times.Once);

            _brokersRepositoryMock.Verify(
                r => r.Add(It.Is<Brokeri>(b =>
                b.Kompanija == "Broker A" &&
                b.ProcentProvizija == 0.5m)),
                Times.Once);
        }

        [TestMethod]
        public void Add_EmptyCompany_ThrowsValidationException()
        {
            var request = new AddBrokerRequest
            {
                Company = "",
                CommissionPercent = 0.5m
            };

            Assert.ThrowsException<ValidationException>(
                () => _service.Add(request));

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany(It.IsAny<string>()),
                Times.Never);

            _brokersRepositoryMock.Verify(
                r => r.Add(It.IsAny<Brokeri>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_DuplicateCompany_ThrowsAlreadyExistsException()
        {
            var request = new AddBrokerRequest
            {
                Company = "Broker A",
                CommissionPercent = 0.5m
            };

            var existingCompanyBroker = new Brokeri
            {
                Id = 1,
                Kompanija = "Broker A",
                ProcentProvizija = 0.75m
            };

            _brokersRepositoryMock
                .Setup(r => r.GetByCompany("Broker A"))
                .Returns(existingCompanyBroker);

            Assert.ThrowsException<AlreadyExistsException>(
                () => _service.Add(request));

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany("Broker A"),
                Times.Once);

            _brokersRepositoryMock.Verify(
                r => r.Add(It.IsAny<Brokeri>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_NegativeCommission_ThrowsValidationException()
        {
            var request = new AddBrokerRequest
            {
                Company = "Broker A",
                CommissionPercent = -1.0m
            };

            _brokersRepositoryMock
                .Setup(r => r.GetByCompany("Broker A"))
                .Returns((Brokeri?)null);

            Assert.ThrowsException<ValidationException>(
                () => _service.Add(request));

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany("Broker A"),
                Times.Once);

            _brokersRepositoryMock.Verify(
                r => r.Add(It.IsAny<Brokeri>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_ValidRequest_UpdatesAndReturnsBroker()
        {
            var existingBroker = new Brokeri
            {
                Id = 1,
                Kompanija = "Old Broker",
                ProcentProvizija = 0.5m
            };

            var request = new UpdateBrokerRequest
            {
                Company = "New Broker",
                CommissionPercent = 1.0m
            };

            _brokersRepositoryMock
                .Setup(r => r.GetByCompany("New Broker"))
                .Returns((Brokeri?)null);

            _brokersRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(existingBroker);

            _brokersRepositoryMock
                .Setup(r => r.Update(It.IsAny<Brokeri>()))
                .Returns((Brokeri b) => b);

            var result = _service.Update(1, request);

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("New Broker", result.Company);
            Assert.AreEqual(1.0m, result.CommissionPercent);

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany("New Broker"),
                Times.Once);

            _brokersRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);

            _brokersRepositoryMock.Verify(
                r => r.Update(It.Is<Brokeri>(b =>
                b.Id == 1 &&
                b.Kompanija == "New Broker" &&
                b.ProcentProvizija == 1.0m)),
                Times.Once);
        }

        [TestMethod]
        public void Update_EmptyCompany_ThrowsValidationException()
        {
            var request = new UpdateBrokerRequest
            {
                Company = "",
                CommissionPercent = 0.5m
            };

            Assert.ThrowsException<ValidationException>(
                () => _service.Update(1, request));

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany(It.IsAny<string>()),
                Times.Never);

            _brokersRepositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);

            _brokersRepositoryMock.Verify(
                r => r.Update(It.IsAny<Brokeri>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_DuplicateCompany_ThrowsAlreadyExistsException()
        {
            var request = new UpdateBrokerRequest
            {
                Company = "Broker A",
                CommissionPercent = 0.5m
            };

            var existingCompanyBroker = new Brokeri
            {
                Id = 2,
                Kompanija = "Broker A",
                ProcentProvizija = 0.75m
            };

            _brokersRepositoryMock
                .Setup(r => r.GetByCompany("Broker A"))
                .Returns(existingCompanyBroker);

            Assert.ThrowsException<AlreadyExistsException>(
                () => _service.Update(1, request));

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany("Broker A"),
                Times.Once);

            _brokersRepositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);

            _brokersRepositoryMock.Verify(
                r => r.Update(It.IsAny<Brokeri>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_NegativeCommission_ThrowsValidationException()
        {
            var request = new UpdateBrokerRequest
            {
                Company = "Broker A",
                CommissionPercent = -1.0m
            };

            _brokersRepositoryMock
                .Setup(r => r.GetByCompany("Broker A"))
                .Returns((Brokeri?)null);

            Assert.ThrowsException<ValidationException>(
                () => _service.Update(1, request));

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany("Broker A"),
                Times.Once);

            _brokersRepositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);

            _brokersRepositoryMock.Verify(
                r => r.Update(It.IsAny<Brokeri>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_BroketNotFound_ThrowsNotFoundException()
        {
            var request = new UpdateBrokerRequest
            {
                Company = "Broker A",
                CommissionPercent = 0.5m
            };

            _brokersRepositoryMock
                .Setup(r => r.GetByCompany("Broker A"))
                .Returns((Brokeri?)null);

            _brokersRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((Brokeri?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.Update(1, request));

            _brokersRepositoryMock.Verify(
                r => r.GetByCompany("Broker A"),
                Times.Once);

            _brokersRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);

            _brokersRepositoryMock.Verify(
                r => r.Update(It.IsAny<Brokeri>()),
                Times.Never);
        }

        [TestMethod]
        public void Delete_ExistingBroker_DeletesAndReturnsBroker()
        {
            var broker = new Brokeri
            {
                Id = 1,
                Kompanija = "Broker A",
                ProcentProvizija = 0.5m
            };

            var deletedBroker = new Brokeri
            {
                Id = 1,
                Kompanija = "Broker A",
                ProcentProvizija = 0.5m
            };

            _brokersRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(broker);

            _brokersRepositoryMock
                .Setup(r => r.Delete(broker))
                .Returns(deletedBroker);

            var result = _service.Delete(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Broker A", result.Company);
            Assert.AreEqual(0.5m, result.CommissionPercent);

            _brokersRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);

            _brokersRepositoryMock.Verify(
                r => r.Delete(broker),
                Times.Once);
        }

        [TestMethod]
        public void Delete_BrokerNotFound_ThrowsNotFoundException()
        {
            int brokerId = 1000;

            _brokersRepositoryMock
                .Setup(r => r.GetById(brokerId))
                .Returns((Brokeri?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.Delete(brokerId));

            _brokersRepositoryMock.Verify(
                r => r.GetById(brokerId),
                Times.Once);

            _brokersRepositoryMock.Verify(
                r => r.Delete(It.IsAny<Brokeri>()),
                Times.Never);
        }
    }
}
