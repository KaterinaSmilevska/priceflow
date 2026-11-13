using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class ThresholdServiceTests
    {
        private Mock<IThresholdRepository> _thresholdRepositoryMock = null!;
        private Mock<ITransactionsRepository> _transactionsRepositoryMock = null!;
        private Mock<ISecuritiesRepository> _securitiesRepositoryMock = null!;
        private Mock<IAuthRepository> _authRepositoryMock = null!;
        private ThresholdService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _thresholdRepositoryMock = new Mock<IThresholdRepository>();
            _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
            _securitiesRepositoryMock = new Mock<ISecuritiesRepository>();
            _authRepositoryMock = new Mock<IAuthRepository>();
            _service = new ThresholdService(_thresholdRepositoryMock.Object, _transactionsRepositoryMock.Object,
                _securitiesRepositoryMock.Object, _authRepositoryMock.Object);
        }

        [TestMethod]
        public void GetUserThresholds_ReturnsUserThresholds()
        {
            var user = new Korisnici
            {
                Id = 1
            };

            var securities = new List<HartiiOdVrednost>
            {
                new HartiiOdVrednost
                {
                    Id = 1,
                    Kod = "ALK"
                },
                new HartiiOdVrednost
                {
                    Id = 2,
                    Kod = "KMB"
                }
            };

            var thresholds = new List<HvPromenaCena>
            {
                new HvPromenaCena
                {
                    Id = 10,
                    KorisnikId = 1,
                    Hvid = 1,
                    DolnaGranica = 100m,
                    GornaGranica = 150m,
                    Hv = securities[0]
                },
                new HvPromenaCena
                {
                    Id = 11,
                    KorisnikId = 1,
                    Hvid = 2,
                    DolnaGranica = 200m,
                    GornaGranica = 250m,
                    Hv = securities[1]
                }
            };

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            _thresholdRepositoryMock
                .Setup(r => r.GetByUserId(1))
                .Returns(thresholds);

            var result = _service.GetUserThresholds(1).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(10, result[0].Id);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(100m, result[0].LowerThreshold);
            Assert.AreEqual(150m, result[0].UpperThreshold);

            Assert.AreEqual(11, result[1].Id);
            Assert.AreEqual(2, result[1].SecurityId);
            Assert.AreEqual("KMB", result[1].SecurityCode);
            Assert.AreEqual(200m, result[1].LowerThreshold);
            Assert.AreEqual(250m, result[1].UpperThreshold);

            _authRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);

            _thresholdRepositoryMock.Verify(
                r => r.GetByUserId(1),
                Times.Once);
        }

        [TestMethod]
        public void GetUserThresholds_WhenUserDoesNotExist_ThrowsNotFoundException()
        {
            _authRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((Korisnici?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.GetUserThresholds(1000));

            Assert.AreEqual("USER_NOT_FOUND", exception.Code);

            _thresholdRepositoryMock.Verify(
                r => r.GetByUserId(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public void GetOwnedSecurities_ReturnsOwnedSecurities()
        {
            var user = new Korisnici
            {
                Id = 1
            };

            var securities = new List<HartiiOdVrednost>
            {
                   new HartiiOdVrednost
                {
                    Id = 1,
                    Kod = "ALK"
                },
                new HartiiOdVrednost
                {
                    Id = 2,
                    Kod = "KMB"
                }
            };

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(1))
                .Returns(new List<int>  { 1, 2 });

            _securitiesRepositoryMock
                .Setup(r => r.GetAllByIds(It.Is<List<int>>(ids =>
                ids.SequenceEqual(new List<int> { 1, 2 }))))
                .Returns(securities);

            var result = _service.GetOwnedSecurities(1).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("ALK", result[0].SecurityCode);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("KMB", result[1].SecurityCode);

            _authRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedSecuritiesIds(1),
                Times.Once);

            _securitiesRepositoryMock.Verify(
                r => r.GetAllByIds(It.IsAny<List<int>>()),
                Times.Once);
        }

        [TestMethod]
        public void GetOwnedSecurities_WhenUserDoesNotExist_ThrowsNotFoundException()
        {
            _authRepositoryMock
              .Setup(r => r.GetById(1000))
              .Returns((Korisnici?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.GetOwnedSecurities(1000));

            Assert.AreEqual("USER_NOT_FOUND", exception.Code);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedSecuritiesIds(It.IsAny<int>()),
                Times.Never);

            _securitiesRepositoryMock.Verify(
                r => r.GetAllByIds(It.IsAny<List<int>>()),
                Times.Never);
        }

        [TestMethod]
        public void GetOwnedSecurities_WhenUserOwnsNoSecurities_ReturnsEmptyList()
        {
            var user = new Korisnici
            {
                Id = 1
            };

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(1))
                .Returns(new List<int>());

            _securitiesRepositoryMock
                .Setup(r => r.GetAllByIds(It.Is<List<int>>(ids => ids.Count == 0)))
                .Returns(new List<HartiiOdVrednost>());

            var result = _service.GetOwnedSecurities(1).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Add_ValidRequest_AddsAndReturnsThreshold()
        {
            var user = new Korisnici
            {
                Id = 1
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var request = new AddThresholdRequest
            {
                SecurityId = 1,
                LowerThreshold = 100m,
                UpperThreshold = 150m
            };

            var addedThreshold = new HvPromenaCena
            {
                Id = 10,
                KorisnikId = 1,
                Hvid = 1,
                DolnaGranica = 100m,
                GornaGranica = 150m,
                Hv = security
            };

            _thresholdRepositoryMock
                .Setup(r => r.GetByUserIdAndSecurityId(1, 1))
                .Returns((HvPromenaCena?)null);

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(security);

            _thresholdRepositoryMock
                .Setup(r => r.Add(It.IsAny<HvPromenaCena>()))
                .Returns(addedThreshold);

            var result = _service.Add(1, request);

            Assert.AreEqual(10, result.Id);
            Assert.AreEqual(1, result.SecurityId);
            Assert.AreEqual("ALK", result.SecurityCode);
            Assert.AreEqual(100m, result.LowerThreshold);
            Assert.AreEqual(150m, result.UpperThreshold);

            _thresholdRepositoryMock.Verify(
                r => r.GetByUserIdAndSecurityId(1, 1),
                Times.Once);

            _authRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);

            _securitiesRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);

            _thresholdRepositoryMock.Verify(
                r => r.Add(It.Is<HvPromenaCena>(t =>
                t.KorisnikId == 1 &&
                t.Hvid == 1 &&
                t.DolnaGranica == 100m &&
                t.GornaGranica == 150m)),
                Times.Once);
        }

        [TestMethod]
        public void Add_WhenLowerThresholsIsGreaterThanUpperThreshold_ThrowsValidationException()
        {
            var request = new AddThresholdRequest
            {
                SecurityId = 5,
                LowerThreshold = 200m,
                UpperThreshold = 100m
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _service.Add(1, request));

            Assert.AreEqual("INVALID_THRESHOLD_RANGE", exception.Code);

            _thresholdRepositoryMock.Verify(
                r => r.GetByUserIdAndSecurityId(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);

            _thresholdRepositoryMock.Verify(
                r => r.Add(It.IsAny<HvPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_WhenThresholdsAreEqual_ThrowsValidationException()
        {
            var request = new AddThresholdRequest
            {
                SecurityId = 5,
                LowerThreshold = 100m,
                UpperThreshold = 100m
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _service.Add(1, request));

            Assert.AreEqual("INVALID_THRESHOLD_RANGE", exception.Code);

            _thresholdRepositoryMock.Verify(
                r => r.GetByUserIdAndSecurityId(It.IsAny<int>(), It.IsAny<int>()),
                Times.Never);

            _thresholdRepositoryMock.Verify(
                r => r.Add(It.IsAny<HvPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_WhenThresholdAlreadyExists_ThrowsAlreadyExistsException()
        {
            var request = new AddThresholdRequest
            {
                SecurityId = 1,
                LowerThreshold = 100m,
                UpperThreshold = 150m
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var existingThreshold = new HvPromenaCena
            {
                Id = 10,
                KorisnikId = 1,
                Hvid = 1,
                DolnaGranica = 100m,
                GornaGranica = 150m,
                Hv = security
            };

            _thresholdRepositoryMock
                .Setup(r => r.GetByUserIdAndSecurityId(1, 1))
                .Returns(existingThreshold);

            var exception = Assert.ThrowsException<AlreadyExistsException>(
                () => _service.Add(1, request));

            Assert.AreEqual("THRESHOLD_ALREADY_EXISTS", exception.Code);

            _authRepositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);

            _securitiesRepositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);

            _thresholdRepositoryMock.Verify(
                r => r.Add(It.IsAny<HvPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_WhenUserDoesNotExist_ThrowsNotFoundException()
        {
            var request = new AddThresholdRequest
            {
                SecurityId = 1,
                LowerThreshold = 100m,
                UpperThreshold = 150m
            };

            _thresholdRepositoryMock
               .Setup(r => r.GetByUserIdAndSecurityId(1, 1))
               .Returns((HvPromenaCena?)null);

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((Korisnici?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
               () => _service.Add(1, request));

            Assert.AreEqual("USER_NOT_FOUND", exception.Code);

            _securitiesRepositoryMock.Verify(
              r => r.GetById(It.IsAny<int>()),
              Times.Never);

            _thresholdRepositoryMock.Verify(
                r => r.Add(It.IsAny<HvPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_WhenSecurityDoesNotExist_ThrowsNotFoundException()
        {
            var user = new Korisnici
            {
                Id = 1
            };

            var request = new AddThresholdRequest
            {
                SecurityId = 1,
                LowerThreshold = 100m,
                UpperThreshold = 150m
            };

            _thresholdRepositoryMock
               .Setup(r => r.GetByUserIdAndSecurityId(1, 1))
               .Returns((HvPromenaCena?)null);

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
               () => _service.Add(1, request));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);

            _thresholdRepositoryMock.Verify(
                r => r.Add(It.IsAny<HvPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_ValidRequest_UpdatesAndReturnsThreshold()
        {
            var request = new UpdateThresholdRequest
            {
                SecurityId = 1,
                LowerThreshold = 120m,
                UpperThreshold = 180m
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var existingThreshold = new HvPromenaCena
            {
                Id = 10,
                KorisnikId = 1,
                Hvid = 1,
                DolnaGranica = 100m,
                GornaGranica = 150m,
                Hv = security
            };

            _thresholdRepositoryMock
               .Setup(r => r.GetByUserIdAndSecurityId(1, 1))
               .Returns((HvPromenaCena?)null);

            _thresholdRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns(existingThreshold);

            _thresholdRepositoryMock
                .Setup(r => r.Update(It.IsAny<HvPromenaCena>()))
                .Returns(existingThreshold);

            var beforeUpdate = DateTime.Now;

            var result = _service.Update(1, 10, request);

            var afterUpdate = DateTime.Now;

            Assert.AreEqual(10, result.Id);
            Assert.AreEqual(1, result.SecurityId);
            Assert.AreEqual("ALK", result.SecurityCode);
            Assert.AreEqual(120m, result.LowerThreshold);
            Assert.AreEqual(180m, result.UpperThreshold);

            Assert.AreEqual(120m, existingThreshold.DolnaGranica);
            Assert.AreEqual(180m, existingThreshold.GornaGranica);

            Assert.IsTrue(existingThreshold.DateModified >= beforeUpdate &&
                existingThreshold.DateModified <= afterUpdate);

            _thresholdRepositoryMock.Verify(
                r => r.GetByUserIdAndSecurityId(1, 1),
                Times.Once);

            _thresholdRepositoryMock.Verify(
                r => r.GetById(10),
                Times.Once);

            _thresholdRepositoryMock.Verify(
                r => r.Update(existingThreshold),
                Times.Once);
        }

        [TestMethod]
        public void Update_WhenLowerThresholdIsGreaterThanUpperThreshold_ThrowsValidationException()
        {
            var request = new UpdateThresholdRequest
            {
                SecurityId = 1,
                LowerThreshold = 200m,
                UpperThreshold = 100m
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _service.Update(1, 10, request));

            Assert.AreEqual("INVALID_THRESHOLD_RANGE", exception.Code);

            _thresholdRepositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);

            _thresholdRepositoryMock.Verify(
                r => r.Update(It.IsAny<HvPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_WhenThresholdAlreadyExistsForAnotherThreshold_ThrowsAlreadyExistsException()
        {
            var request = new UpdateThresholdRequest
            {
                SecurityId = 1,
                LowerThreshold = 120m,
                UpperThreshold = 180m
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var otherThreshold = new HvPromenaCena
            {
                Id = 11,
                KorisnikId = 1,
                Hvid = 1,
                DolnaGranica = 100,
                GornaGranica = 150m,
                Hv = security
            };

            _thresholdRepositoryMock
                .Setup(r => r.GetByUserIdAndSecurityId(1, 1))
                .Returns(otherThreshold);

            var exception = Assert.ThrowsException<AlreadyExistsException>(
                () => _service.Update(1, 10, request));

            Assert.AreEqual("THRESHOLD_ALREADY_EXISTS", exception.Code);

            _thresholdRepositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);

            _thresholdRepositoryMock.Verify(
                r => r.Update(It.IsAny<HvPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_WhenExistingThresholdIsSameThreshold_DoesNotThrowAlreadyExistsException()
        {
            var request = new UpdateThresholdRequest
            {
                SecurityId = 1,
                LowerThreshold = 120m,
                UpperThreshold = 180m
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var existingThreshold = new HvPromenaCena
            {
                Id = 10,
                KorisnikId = 1,
                Hvid = 1,
                DolnaGranica = 100m,
                GornaGranica = 150m,
                Hv = security
            };

            _thresholdRepositoryMock
                .Setup(r => r.GetByUserIdAndSecurityId(1, 1))
                .Returns(existingThreshold);

            _thresholdRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns(existingThreshold);

            _thresholdRepositoryMock
                .Setup(r => r.Update(It.IsAny<HvPromenaCena>()))
                .Returns(existingThreshold);

            var result = _service.Update(1, 10, request);

            Assert.AreEqual(120m, result.LowerThreshold);
            Assert.AreEqual(180m, result.UpperThreshold);

            _thresholdRepositoryMock.Verify(
                r => r.Update(existingThreshold),
                Times.Once);
        }

        [TestMethod]
        public void Update_WhenThresholdDoesNotExist_ThrowsNotFoundException()
        {
            var request = new UpdateThresholdRequest
            {
                SecurityId = 1,
                LowerThreshold = 120m,
                UpperThreshold = 180m
            };

            _thresholdRepositoryMock
                .Setup(r => r.GetByUserIdAndSecurityId(1, 1))
                .Returns((HvPromenaCena?)null);

            _thresholdRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns((HvPromenaCena?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.Update(1, 10, request));

            Assert.AreEqual("THRESHOLD_NOT_FOUND", exception.Code);

            _thresholdRepositoryMock.Verify(
                r => r.Update(It.IsAny<HvPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_WhenUserDoesNotOwnThreshold_ThrowsUnauthorizedException()
        {
            var request = new UpdateThresholdRequest
            {
                SecurityId = 1,
                LowerThreshold = 120m,
                UpperThreshold = 180m
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var existingThreshold = new HvPromenaCena
            {
                Id = 10,
                KorisnikId = 2,
                Hvid = 1,
                DolnaGranica = 100m,
                GornaGranica = 150m,
                Hv = security
            };

            _thresholdRepositoryMock
                .Setup(r => r.GetByUserIdAndSecurityId(1, 1))
                .Returns((HvPromenaCena?)null);

            _thresholdRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns(existingThreshold);

            var exception = Assert.ThrowsException<UnauthorizedException>(
            () => _service.Update(1, 10, request));

            Assert.AreEqual("THRESHOLD_ACCESS_DENIED", exception.Code);

            _thresholdRepositoryMock.Verify(
                r => r.Update(It.IsAny<HvPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void Delete_WhenUserOwnsThreshold_DeletesAndReturnsThreshold()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var existingThreshold = new HvPromenaCena
            {
                Id = 10,
                KorisnikId = 1,
                Hvid = 1,
                DolnaGranica = 100m,
                GornaGranica = 150m,
                Hv = security
            };

            _thresholdRepositoryMock
               .Setup(r => r.GetById(10))
               .Returns(existingThreshold);

            _thresholdRepositoryMock
                .Setup(r => r.Delete(existingThreshold))
                .Returns(existingThreshold);

            var result = _service.Delete(1, 10);

            Assert.AreEqual(10, result.Id);
            Assert.AreEqual(1, result.SecurityId);
            Assert.AreEqual("ALK", result.SecurityCode);
            Assert.AreEqual(100m, result.LowerThreshold);
            Assert.AreEqual(150m, result.UpperThreshold);

            _thresholdRepositoryMock.Verify(
                r => r.GetById(10),
                Times.Once);

            _thresholdRepositoryMock.Verify(
                r => r.Delete(existingThreshold),
                Times.Once);
        }

        [TestMethod]
        public void Delete_WhenThresholdDoesNotExist_ThrowsNotFoundException()
        {
            _thresholdRepositoryMock
                .Setup(r => r.GetById(10))
                .Returns((HvPromenaCena?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
            () => _service.Delete(1, 10));

            Assert.AreEqual("THRESHOLD_NOT_FOUND", exception.Code);

            _thresholdRepositoryMock.Verify(
                r => r.Delete(It.IsAny<HvPromenaCena>()),
                Times.Never);
        }

        [TestMethod]
        public void Delete_WhenUserDoesNotOwnThreshold_ThrowUnauthorizedException()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var existingThreshold = new HvPromenaCena
            {
                Id = 10,
                KorisnikId = 2,
                Hvid = 2,
                DolnaGranica = 100m,
                GornaGranica = 150m,
                Hv = security
            };

            _thresholdRepositoryMock
               .Setup(r => r.GetById(10))
               .Returns(existingThreshold);

            var exception = Assert.ThrowsException<UnauthorizedException>(
           () => _service.Delete(1, 10));

            Assert.AreEqual("THRESHOLD_ACCESS_DENIED", exception.Code);

            _thresholdRepositoryMock.Verify(
                r => r.Delete(It.IsAny<HvPromenaCena>()),
                Times.Never);
        }
    }
}
