using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class SecuritiesServiceTests
    {
        private Mock<ISecuritiesRepository> _securitiesRepositoryMock = null!;
        private Mock<IDailyTurnoverRepository> _dailyTurnoverRepositoryMock = null!;
        private Mock<IIssuersRepository> _issuersRepositoryMock = null!;
        private Mock<ITypeSecurityRepository> _typeSecurityRepositoryMock = null!;
        private SecuritiesService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _securitiesRepositoryMock = new Mock<ISecuritiesRepository>();
            _dailyTurnoverRepositoryMock = new Mock<IDailyTurnoverRepository>();
            _issuersRepositoryMock = new Mock<IIssuersRepository>();
            _typeSecurityRepositoryMock = new Mock<ITypeSecurityRepository>();
            _service = new SecuritiesService(_securitiesRepositoryMock.Object, _dailyTurnoverRepositoryMock.Object, 
                _issuersRepositoryMock.Object, _typeSecurityRepositoryMock.Object);
        }

        [TestMethod]
        public void FindById_ValidSecurity_ReturnsMappedSecurity()
        {
            var typeSecurity = new TipHv
            {
                Id = 2,
                Ime = "Shares"
            };

            var issuer = new Izdavachi
            {
                Id = 3,
                Ime = "Alkaloid AD Skopje"
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Isin = "MKALKA101011",
                Kod = "ALK",
                VkupenBrojAkcii = 100000,
                TipHv = typeSecurity,
                Izdavach = issuer
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(security);

            var result = _service.FindById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("MKALKA101011", result.Isin);
            Assert.AreEqual("ALK", result.Code);
            Assert.AreEqual("Shares", result.TypeSecurityName);
            Assert.AreEqual("Alkaloid AD Skopje", result.IssuerName);
            Assert.AreEqual(100000, result.TotalNumShares);

            _securitiesRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);
        }

        [TestMethod]
        public void FindById_SecurityNotFound_ThrowsNotFoundException()
        {
            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () =>  _service.FindById(1));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);
        }

        [TestMethod]
        public void FindByCode_ValidSecurity_ReturnsMappedSecurity()
        {
            var typeSecurity = new TipHv
            {
                Id = 2,
                Ime = "Shares"
            };

            var issuer = new Izdavachi
            {
                Id = 3,
                Ime = "Alkaloid AD Skopje"
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Isin = "MKALKA101011",
                Kod = "ALK",
                VkupenBrojAkcii = 100000,
                TipHv = typeSecurity,
                Izdavach = issuer
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(security);

            var result = _service.FindByCode("ALK");

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("MKALKA101011", result.Isin);
            Assert.AreEqual("ALK", result.Code);
            Assert.AreEqual("Shares", result.TypeSecurityName);
            Assert.AreEqual("Alkaloid AD Skopje", result.IssuerName);
            Assert.AreEqual(100000, result.TotalNumShares);

            _securitiesRepositoryMock.Verify(
                r => r.GetByCode("ALK"),
                Times.Once);
        }

        [TestMethod]
        public void FindByCode_SecurityNotFound_ThrowsNotFoundException()
        {
            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.FindByCode("ALK"));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);
        }

        [TestMethod]
        public void FindAll_SecuritiesExist_ReturnsMappedSecurities()
        {
            var typeSecurity = new TipHv
            {
                Id = 2,
                Ime = "Shares"
            };

            var issuers = new List<Izdavachi>
            {
                new Izdavachi
                {
                    Id = 1,
                    Ime = "Alkaloid AD Skopje"
                },
                 new Izdavachi
                {
                    Id = 2,
                    Ime = "Komercijalna Banka AD Skopje"
                }
            };

            var securities = new List<HartiiOdVrednost>
            { 
                new HartiiOdVrednost
                {
                    Id = 1,
                    Isin = "MKALKA101011",
                    Kod = "ALK",
                    VkupenBrojAkcii = 100,
                    TipHv = typeSecurity,
                    Izdavach = issuers[0]
                },
                new HartiiOdVrednost
                {
                    Id = 2,
                    Isin = "MKKMB101012",
                    Kod = "KMB",
                    VkupenBrojAkcii = 200,
                    TipHv = typeSecurity,
                    Izdavach = issuers[1]
                },
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(securities);

            var result = _service.FindAll().ToList();

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("ALK", result[0].Code);
            Assert.AreEqual("KMB", result[1].Code);

            _securitiesRepositoryMock.Verify(
                r => r.GetAll(),
                Times.Once);
        }

        [TestMethod]
        public void FindAll_NoSecurities_ReturnsEmptyList()
        {
            _securitiesRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<HartiiOdVrednost>());

            var result = _service.FindAll().ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void FindSecurityCode_ValidSecurity_ReturnsCode()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(security);

            _securitiesRepositoryMock
                .Setup(r => r.GetSecurityCode(1))
                .Returns("ALK");

            var result = _service.FindSecurityCode(1);

            Assert.AreEqual("ALK", result);

            _securitiesRepositoryMock.Verify(
                r => r.GetSecurityCode(1),
                Times.Once);
        }

        [TestMethod]
        public void FindSecurityCode_SecurityNotFound_ThrowsNotFoundException()
        {
            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.FindSecurityCode(1));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);

            _securitiesRepositoryMock.Verify(
                r => r.GetSecurityCode(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public void FindTotalNumShares_ById_ReturnsTotalShares()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(security);

            _securitiesRepositoryMock
                .Setup(r => r.GetTotalNumSharesById(1))
                .Returns(500000);

            var result = _service.FindTotalNumShares(1);

            Assert.AreEqual(500000, result);
        }

        [TestMethod]
        public void FindTotalNumShares_ById_SecurityNotFound_ThrowsNotFoundException()
        {
            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.FindTotalNumShares(1));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);
        }

        [TestMethod]
        public void FindTotalNumShares_ByCode_ReturnsTotalShares()
        {
            _securitiesRepositoryMock
                .Setup(r => r.GetTotalNumSharesBySecurityCode("ALK"))
                .Returns(500000);

            var result = _service.FindTotalNumShares("ALK");

            Assert.AreEqual(500000, result);

            _securitiesRepositoryMock.Verify(
                r => r.GetTotalNumSharesBySecurityCode("ALK"),
                Times.Once);
        }

        [TestMethod]
        public void Add_ValidRequest_ReturnsAddedSecurity()
        {
            var request = new AddSecurityRequest
            {
                Isin = "MKALK101011",
                Code = "ALK",
                TotalNumShares = 100000,
                TypeSecurityId = 2,
                IssuerId = 3
            };

            var typeSecurity = new TipHv
            {
                Id = 2,
                Ime = "Shares"
            };

            var issuer = new Izdavachi
            {
                Id = 3,
                Ime = "Alkaloid AD Skopje"
            };

            var addedSecurity = new HartiiOdVrednost
            {
                Id = 1,
                Isin = request.Isin,
                Kod = request.Code,
                VkupenBrojAkcii = request.TotalNumShares,
                TipHvid = 2,
                IzdavachId = 3,
                TipHv = typeSecurity,
                Izdavach = issuer
            };

           _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns((HartiiOdVrednost?)null);

            _typeSecurityRepositoryMock
                .Setup(r => r.GetById(2))
                .Returns(typeSecurity);

            _issuersRepositoryMock
                .Setup(r => r.GetById(3))
                .Returns(issuer);

            _securitiesRepositoryMock
                .Setup(r => r.Add(It.IsAny<HartiiOdVrednost>()))
                .Returns(addedSecurity);

            var result = _service.Add(request);

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("MKALK101011", result.Isin);
            Assert.AreEqual("ALK", result.Code);
            Assert.AreEqual("Shares", result.TypeSecurityName);
            Assert.AreEqual("Alkaloid AD Skopje", result.IssuerName);
            Assert.AreEqual(100000, result.TotalNumShares);

            _securitiesRepositoryMock.Verify(
                r => r.Add(It.IsAny<HartiiOdVrednost>()),
                Times.Once);
        }

        [TestMethod]
        public void Add_DuplicateCode_ThrowsAlreadyExistsException()
        {
            var request = new AddSecurityRequest
            {
                Isin = "MKALK101011",
                Code = "ALK",
                TotalNumShares = 100000,
                TypeSecurityId = 2,
                IssuerId = 3
            };

            var existingSecurity = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(existingSecurity);

            var exception = Assert.ThrowsException<AlreadyExistsException>(
                () => _service.Add(request));

            Assert.AreEqual("CODE_ALREADY_EXISTS", exception.Code);

            _securitiesRepositoryMock.Verify(
                r => r.Add(It.IsAny<HartiiOdVrednost>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_TypeSecurityNotFound_ThrowsNotFoundException()
        {
            var request = new AddSecurityRequest
            {
                Isin = "MKALK101011",
                Code = "ALK",
                TotalNumShares = 100000,
                TypeSecurityId = 2,
                IssuerId = 3
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns((HartiiOdVrednost?)null);

            _typeSecurityRepositoryMock
                .Setup(r => r.GetById(2))
                .Returns((TipHv?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.Add(request));

            Assert.AreEqual("TYPESECURITY_NOT_FOUND", exception.Code);
        }

        [TestMethod]
        public void Add_IssuerNotFound_ThrowsNotFoundException()
        {
            var request = new AddSecurityRequest
            {
                Isin = "MKALK101011",
                Code = "ALK",
                TotalNumShares = 100000,
                TypeSecurityId = 2,
                IssuerId = 3
            };

            var typeSecurity = new TipHv
            {
                Id = 2
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns((HartiiOdVrednost?)null);

            _typeSecurityRepositoryMock
                .Setup(r => r.GetById(2))
                .Returns(typeSecurity);

            _issuersRepositoryMock
                .Setup(r => r.GetById(3))
                .Returns((Izdavachi?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.Add(request));

            Assert.AreEqual("ISSUER_NOT_FOUND", exception.Code);
        }

        [TestMethod]
        public void Update_ValidSecurity_ReturnsUpdatedSecurity()
        {
            var request = new UpdateSecurity
            {
                Isin = "MKALK101011",
                Code = "ALK",
                TotalNumShares = 200000,
                TypeSecurityId = 2,
                IssuerId = 3
            };

            var existingTypeSecurity = new TipHv
            {
                Ime = "Old Type"
            };

            var existingIssuer = new Izdavachi
            {
                Ime = "Old Issuer"
            };

            var existingSecurity = new HartiiOdVrednost
            {
                Id = 1,
                Isin = "Old Isin",
                Kod = "Old",
                VkupenBrojAkcii = 100000,
                TipHv = existingTypeSecurity,
                Izdavach = existingIssuer
            };

            var typeSecurity = new TipHv
            {
                Id = 2,
                Ime = "Shares"
            };

            var issuer = new Izdavachi
            {
                Id = 3,
                Ime = "Alkaloid AD Skopje"
            };

            var updatedSecurity = new HartiiOdVrednost
            {
                Id = 1,
                Isin = "MKALKA101011",
                Kod = "ALK",
                VkupenBrojAkcii = 200000,
                TipHv = typeSecurity,
                Izdavach = issuer
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(existingSecurity);

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(existingSecurity);

            _typeSecurityRepositoryMock
                .Setup(r => r.GetById(2))
                .Returns(existingTypeSecurity);

            _issuersRepositoryMock
                .Setup(r => r.GetById(3))
                .Returns(existingIssuer);

            _securitiesRepositoryMock
                .Setup(r => r.Update(existingSecurity))
                .Returns(updatedSecurity);

            var result = _service.Update(1, request);

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("MKALKA101011", result.Isin);
            Assert.AreEqual("ALK", result.Code);
            Assert.AreEqual(200000, result.TotalNumShares);
            Assert.AreEqual("Shares", result.TypeSecurityName);
            Assert.AreEqual("Alkaloid AD Skopje", result.IssuerName);

            _securitiesRepositoryMock.Verify(
                r => r.Update(existingSecurity),
                Times.Once);
        }

        [TestMethod]
        public void Update_SecurityNotFound_ThrowsNotFoundException()
        {
            var request = new UpdateSecurity
            {
                Isin = "MKALK101011",
                Code = "ALK",
                TotalNumShares = 100000,
                TypeSecurityId = 2,
                IssuerId = 3
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns((HartiiOdVrednost?)null);

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.Update(1, request));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);
        }

        [TestMethod]
        public void Update_DuplicateCode_ThrowsAlreadyExistsException()
        {
            var request = new UpdateSecurity
            {
                Isin = "MKALK101012",
                Code = "KMB",
                TotalNumShares = 100000,
                TypeSecurityId = 2,
                IssuerId = 3
            };

            var security = new HartiiOdVrednost
            {
                Id = 2,
                Kod = "KMB"
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("KMB"))
                .Returns(security);

            var exception = Assert.ThrowsException<AlreadyExistsException>(
                () => _service.Update(1, request));

            Assert.AreEqual("CODE_ALREADY_EXISTS", exception.Code);

            _securitiesRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Never);
        }

        [TestMethod]
        public void Delete_ValidSecurity_ReturnsDeletedSecurity()
        {
            var typeSecurity = new TipHv
            {
                Id = 2,
                Ime = "Shares"
            };

            var issuer = new Izdavachi
            {
                Id = 3,
                Ime = "Alkaloid AD Skopje"
            };

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Isin = "MKALKA101011",
                Kod = "ALK",
                VkupenBrojAkcii = 100000,
                TipHv = typeSecurity,
                Izdavach = issuer
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(security);

            _securitiesRepositoryMock
                .Setup(r => r.Delete(security))
                .Returns(security);

            var result = _service.Delete(1);

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("ALK", result.Code);

            _securitiesRepositoryMock.Verify(
                r => r.Delete(security),
                Times.Once);
        }

        [TestMethod]
        public void Delete_SecurityNotFound_ThrowsNotFoundException()
        {
            _securitiesRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
             () => _service.Delete(1));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);

            _securitiesRepositoryMock.Verify(
                r => r.Delete(It.IsAny<HartiiOdVrednost>()),
                Times.Never);
        }

        [TestMethod]
        public void GetLatestPrices_DataExists_ReturnsPrices()
        {
            var date = new DateTime(2026, 9, 29);

            var dailyData = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = date,
                    MinCena = 100.50m,
                    MaxCena = 110.75m,
                    ProsecnaCena = 105.25m
                }
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetBySecurityCode("ALK", date))
                .Returns(dailyData);

            var result = _service.GetLatestPrices("ALK", date);

            Assert.IsNotNull(result);
            Assert.AreEqual("ALK", result.SecurityCode);
            Assert.AreEqual(100.50m, result.MinPrice);
            Assert.AreEqual(110.75m, result.MaxPrice);
            Assert.AreEqual(105.25m, result.AveragePrice);
        }

        [TestMethod]
        public void GetLatestPrice_NoData_ReturnsNull()
        {
            var date = new DateTime(2026, 9, 29);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetBySecurityCode("ALK", date))
                .Returns(new List<DnevenPromet>());

            var result = _service.GetLatestPrices("ALK", date);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void SearchByCode_MatchingSecurities_ReturnsMappedSecurities()
        {
            var typeSecurity = new TipHv
            {
                Id = 2,
                Ime = "Shares"
            };

            var issuer = new Izdavachi
            {
                Id = 3,
                Ime = "Alkaloid AD Skopje"
            };

            var securities = new List<HartiiOdVrednost>
            {
                new HartiiOdVrednost
                {
                    Id = 1,
                    Isin = "MKALKA101011",
                    Kod = "ALK",
                    VkupenBrojAkcii = 100,
                    TipHv = typeSecurity,
                    Izdavach = issuer
                }
            };

            _securitiesRepositoryMock
                .Setup(r => r.SearchByCode("AL"))
                .Returns(securities);

            var result = _service.SearchByCode("AL").ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("ALK", result[0].Code);
            Assert.AreEqual("Alkaloid AD Skopje", result[0].IssuerName);
        }

        [TestMethod]
        public void SearchByCode_NoMatches_ReturnsEmptyList()
        {
            _securitiesRepositoryMock
                .Setup(r => r.SearchByCode("XYZ"))
                .Returns(new List<HartiiOdVrednost?>());

            var result = _service.SearchByCode("XYZ").ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetDailyMarketData_DataExists_ReturnsMappedData()
        {
            var date = new DateTime(2026, 9, 29);

            var dailyData = new DnevenPromet
            {
                Hvid = 1,
                Datum = date,
                CenaPoslednaTransakcija = 105.50m,
                MinCena = 100.00m,
                MaxCena = 110.00m,
                ProsecnaCena = 105.25m,
                ProcentPromena = 2.50m,
                KolicinaIstrguvaniAkcii = 1500,
                PrometBestdenari = 150000,
                VkupenPrometDenari = 160000
            };

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetBySecurityCode("ALK", date))
                .Returns(new List<DnevenPromet> { dailyData });

            var result = _service.GetDailyMarketData("ALK", date);

            Assert.IsNotNull(result);
            Assert.AreEqual("ALK", result.SecurityCode);
            Assert.AreEqual(date, result.Date);
            Assert.AreEqual(105.50m, result.LatestTransactionPrice);
            Assert.AreEqual(100.00m, result.MinPrice);
            Assert.AreEqual(110.00m, result.MaxPrice);
            Assert.AreEqual(105.25m, result.AveragePrice);
            Assert.AreEqual(2.50m, result.ChangePercent);
            Assert.AreEqual(1500, result.TradedQuantity);
            Assert.AreEqual(150000, result.TurnoverBESTDenars);
            Assert.AreEqual(160000, result.TotalTurnoverDenars);
        }

        [TestMethod]
        public void GetDailyMarketData_NoData_ReturnsNull()
        {
            var date = new DateTime(2026, 9, 29);

            _dailyTurnoverRepositoryMock
                .Setup(r => r.GetBySecurityCode("ALK", date))
                .Returns(new List<DnevenPromet>());

            var result = _service.GetDailyMarketData("ALK", date);

            Assert.IsNull(result);
        }
    }
}