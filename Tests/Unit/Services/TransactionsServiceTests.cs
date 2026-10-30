using DataAccess.Enums;
using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class TransactionsServiceTests
    {
        private Mock<ITransactionsRepository> _transactionsRepositoryMock = null!;
        private Mock<ISecuritiesRepository> _securitiesRepositoryMock = null!;
        private Mock<IPortfolioReturnsService> _portfolioReturnsServiceMock = null!;
        private Mock<IDailyTurnoverRepository> _dailyTurnoverRepository = null!;
        private TransactionsService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _transactionsRepositoryMock = new Mock<ITransactionsRepository>();
            _securitiesRepositoryMock = new Mock<ISecuritiesRepository>();
            _portfolioReturnsServiceMock = new Mock<IPortfolioReturnsService>();
            _dailyTurnoverRepository = new Mock<IDailyTurnoverRepository>();
            _service = new TransactionsService(_transactionsRepositoryMock.Object, _securitiesRepositoryMock.Object,
                _portfolioReturnsServiceMock.Object, _dailyTurnoverRepository.Object);
        }

        [TestMethod]
        public void FindByPortfolioId_ReturnsMappedTransactions()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "KMB"
            };

            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Id = 1,
                    PortfolioId = 10,
                    Hvid = 2,
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 5,
                    EdinecnaCenaAkcija = 100,
                    Iznos = 500m,
                    Realna = true,
                    BerzanskaProvizija = 0.5m,
                    BrokerskaProvizija = 1m,
                    Cdhvprovizija = 0.2m,
                    Datum = new DateOnly(2026, 10, 1),
                    Hv = security
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(10))
                .Returns(transactions);

            var result = _service.FindByPortfolioId(10).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual(2, result[0].SecurityId);
            Assert.AreEqual("KMB", result[0].SecurityCode);
            Assert.AreEqual(5, result[0].SharesQuantity);
            Assert.AreEqual(100, result[0].SharesUnitPrice);
            Assert.AreEqual(500m, result[0].Amount);
            Assert.AreEqual("Купување", result[0].TypeTransaction);
            Assert.IsTrue(result[0].IsReal);

            _transactionsRepositoryMock.Verify(
                r => r.GetByPortfolioId(10),
                Times.Once);
        }

        [TestMethod]
        public void FindByPortfolioId_NoTransactions_ReturnsEmptyList()
        {
            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(10))
                .Returns(new List<Transakcii>());

            var result = _service.FindByPortfolioId(10).ToList();

            Assert.AreEqual(0, result.Count);

            _transactionsRepositoryMock.Verify(
                r => r.GetByPortfolioId(10),
                Times.Once);
        }

        [TestMethod]
        public void FindOwnedShares_ReturnsOwnedShares()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(security);

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedShares(10, 1, true))
                .Returns(25);

            var result = _service.FindOwnedShares(10, "ALK", true);

            Assert.AreEqual(25, result);

            _securitiesRepositoryMock.Verify(
                r => r.GetByCode("ALK"),
                Times.Once);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedShares(10, 1, true),
                Times.Once);
        }

        [TestMethod]
        public void GetOwnedShares_WhenSecurityDoesNotExist_ThrowsNotFoundException()
        {
            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("Invalid"))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.FindOwnedShares(10, "Invalid", true));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedShares(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>()),
                Times.Never);
        }

        [TestMethod]
        public void FindOwnedSharesAtDate_ReturnsOwnedShares()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var date = new DateOnly(2026, 10, 8);

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(security);

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSharesAtDate(10, 1, true, date, 7))
                .Returns(15);

            var result = _service.FindOwnedSharesAtDate(10, "ALK", true, date, 7);

            Assert.AreEqual(15, result);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedSharesAtDate(10, 1, true, date, 7),
                Times.Once);
        }

        [TestMethod]
        public void FindOwnedSharesAtDate_WhenSecurityDoesNotExist_ThrowsNotFoundException()
        {
            var date = new DateOnly(2026, 10, 8);

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("Invalid"))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.FindOwnedSharesAtDate(10, "Invalid", true, date, null));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedSharesAtDate(
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<bool>(),
                    It.IsAny<DateOnly>(),
                    It.IsAny<int?>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_WhenValidBuy_ReturnsAddedTransaction()
        {
            var date = new DateOnly(2026, 10, 8);

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = "Купување",
                SharesQuantity = 10,
                SharesUnitPrice = 100,
                StockExchangeCommission = 0.5m,
                BrokerageCommission = 1m,
                CDHVCommission = 0.2m,
                IsReal = true,
                Date = date
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(security);

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(10))
                .Returns(new List<Transakcii>());

            _transactionsRepositoryMock
                .Setup(r => r.Add(It.IsAny<Transakcii>()))
                .Returns((Transakcii t) =>
                {
                    t.Id = 20;
                    t.Hv = security;
                    return t;
                });

            var result = _service.Add(10, transaction);

            Assert.AreEqual(20, result.Id);
            Assert.AreEqual(1, result.SecurityId);
            Assert.AreEqual("ALK", result.SecurityCode);
            Assert.AreEqual(10, result.SharesQuantity);
            Assert.AreEqual(100, result.SharesUnitPrice);
            Assert.AreEqual(1000m, result.Amount);
            Assert.AreEqual("Купување", result.TypeTransaction);
            Assert.IsTrue(result.IsReal);

            _transactionsRepositoryMock.Verify(
                r => r.Add(It.Is<Transakcii>(t =>
                t.PortfolioId == 10 &&
                t.Hvid == 1 &&
                t.TipTransakcija == "Купување" &&
                t.KolicinaAkcii == 10 &&
                t.Iznos == 1000m &&
                t.EdinecnaCenaAkcija == 100m &&
                t.Realna == true &&
                t.BerzanskaProvizija == 0.5m &&
                t.BrokerskaProvizija == 1m &&
                t.Cdhvprovizija == 0.2m &&
                t.Datum == date)),
                Times.Once);
        }

        [TestMethod]
        public void Add_WhenValidSell_ReturnsAddedTransaction()
        {
            var date = new DateOnly(2026, 10, 8);

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = "Продавање",
                SharesQuantity = 5,
                SharesUnitPrice = 120,
                StockExchangeCommission = 0.5m,
                BrokerageCommission = 1m,
                CDHVCommission = 0.2m,
                IsReal = true,
                Date = date
            };

            var existingBuy = new Transakcii
            {
                Id = 1,
                PortfolioId = 10,
                Hvid = 1,
                TipTransakcija = "Купување",
                KolicinaAkcii = 10,
                Datum = date,
                Realna = true
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(security);

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(10))
                .Returns(new List<Transakcii> { existingBuy });

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSharesAtDate(10, 1, true, date, null))
                .Returns(10);

            _transactionsRepositoryMock
                .Setup(r => r.Add(It.IsAny<Transakcii>()))
                .Returns((Transakcii t) =>
                {
                    t.Id = 20;
                    t.Hv = security;
                    return t;
                });

            var result = _service.Add(10, transaction);

            Assert.AreEqual(20, result.Id);
            Assert.AreEqual(1, result.SecurityId);
            Assert.AreEqual("ALK", result.SecurityCode);
            Assert.AreEqual(5, result.SharesQuantity);
            Assert.AreEqual(120, result.SharesUnitPrice);
            Assert.AreEqual(600m, result.Amount);
            Assert.AreEqual("Продавање", result.TypeTransaction);
            Assert.IsTrue(result.IsReal);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedSharesAtDate(10, 1, true, date, null),
                Times.Once);

            _transactionsRepositoryMock.Verify(
                r => r.Add(It.IsAny<Transakcii>()),
                Times.Once);
        }

        [TestMethod]
        public void Add_WhenTransactionTypeIsMissing_ThrowsValidationException()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = null!,
                SharesQuantity = 10,
                SharesUnitPrice = 100,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(security);

            var exception = Assert.ThrowsException<ValidationException>(
                () => _service.Add(10, transaction));

            Assert.AreEqual("TYPE_VALIDATION_REQUIRED", exception.Code);

            _transactionsRepositoryMock.Verify(
                r => r.Add(It.IsAny<Transakcii>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_WhenSharesQuantityIsZero_ThrowsInvalidOperationException()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = "Купување",
                SharesQuantity = 0,
                SharesUnitPrice = 1000,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(security);

            var exception = Assert.ThrowsException<InvalidOperationException>(
                () => _service.Add(10, transaction));

            Assert.AreEqual("Transaction quantity must be greater than 0.", exception.Message);

            _transactionsRepositoryMock.Verify(
                r => r.Add(It.IsAny<Transakcii>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_WhenSharesQuantityIsNegative_ThrowsInvalidOperationException()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = "Купување",
                SharesQuantity = -5,
                SharesUnitPrice = 100,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(security);

            var exception = Assert.ThrowsException<InvalidOperationException>(
                () => _service.Add(10, transaction));

            Assert.AreEqual("Transaction quantity must be greater than 0.", exception.Message);

            _transactionsRepositoryMock.Verify(
                r => r.Add(It.IsAny<Transakcii>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_WhenSellingMoreSharesThanOwned_ThrowsBusinessRuleException()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = "Продавање",
                SharesQuantity = 15,
                SharesUnitPrice = 120,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(security);

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(10))
                .Returns(new List<Transakcii>());

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSharesAtDate(10, 1, true, new DateOnly(2026, 10, 8), null))
                .Returns(10);

            var exception = Assert.ThrowsException<BusinessRuleException>(
                () => _service.Add(10, transaction));

            Assert.AreEqual("SELL_MORE_THAN_OWNED", exception.Code);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedSharesAtDate(10, 1, true, new DateOnly(2026, 10, 8), null),
                Times.Once);

            _transactionsRepositoryMock.Verify(
                r => r.Add(It.IsAny<Transakcii>()),
                Times.Never);
        }

        [TestMethod]
        public void Add_WhenSecurityDoesNotExist_ThrowsNotFoundException()
        {
            var transaction = new Transaction
            {
                SecurityCode = "Invalid",
                TypeTransaction = "Купување",
                SharesQuantity = 10,
                SharesUnitPrice = 100,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("Invalid"))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.Add(10, transaction));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);

            _transactionsRepositoryMock.Verify(
                r => r.Add(It.IsAny<Transakcii>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_ValidRequest_ReturnsUpdatedTransaction()
        {
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

            var existingTransaction = new Transakcii
            {
                Id = 1,
                PortfolioId = 10,
                Hvid = 2,
                TipTransakcija = "Купување",
                KolicinaAkcii = 5,
                EdinecnaCenaAkcija = 90,
                Iznos = 450m,
                Datum = new DateOnly(2026, 10, 1),
                Realna = true,
                Hv = securities[1]
            };

            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = "Купување",
                SharesQuantity = 10,
                SharesUnitPrice = 100,
                StockExchangeCommission = 0.5m,
                BrokerageCommission = 1m,
                CDHVCommission = 0.2m,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(existingTransaction);

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(securities[0]);

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(10))
                .Returns(new List<Transakcii>());

            _transactionsRepositoryMock
                .Setup(r => r.Update(It.IsAny<Transakcii>()))
                .Returns((Transakcii t) => t);

            var result = _service.Update(10, 1, transaction);

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(1, result.SecurityId);
            Assert.AreEqual("ALK", result.SecurityCode);
            Assert.AreEqual(10, result.SharesQuantity);
            Assert.AreEqual(100, result.SharesUnitPrice);
            Assert.AreEqual(1000m, result.Amount);
            Assert.AreEqual("Купување", result.TypeTransaction);
            Assert.IsTrue(result.IsReal);

            _transactionsRepositoryMock.Verify(
            r => r.Update(It.Is<Transakcii>(t =>
            t.Id == 1 &&
            t.PortfolioId == 10 &&
            t.Hvid == 1 &&
            t.Hv.Kod == "ALK" &&
            t.TipTransakcija == "Купување" &&
            t.KolicinaAkcii == 10 &&
            t.EdinecnaCenaAkcija == 100m &&
            t.Iznos == 1000m &&
            t.Realna == true &&
            t.BerzanskaProvizija == 0.5m &&
            t.BrokerskaProvizija == 1m &&
            t.Cdhvprovizija == 0.2m &&
            t.Datum == new DateOnly(2026, 10, 8))),
            Times.Once);
        }

        [TestMethod]
        public void Update_WhenTransactionTypeIsMissing_ThrowsValidationException()
        {
            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = null!,
                SharesQuantity = 10,
                SharesUnitPrice = 100,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            var exception = Assert.ThrowsException<ValidationException>(
                () => _service.Update(10, 1, transaction));

            Assert.AreEqual("TYPE_VALIDATION_REQUIRED", exception.Code);

            _transactionsRepositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);

            _transactionsRepositoryMock.Verify(
                r => r.Update(It.IsAny<Transakcii>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_WhenTransactionDoesNotExist_ThrowsNotFoundException()
        {
            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = "Купување",
                SharesQuantity = 10,
                SharesUnitPrice = 100,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetById(20))
                .Returns((Transakcii?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.Update(10, 20, transaction));

            Assert.AreEqual("TRANSACTION_NOT_FOUND", exception.Code);

            _transactionsRepositoryMock.Verify(
                r => r.Update(It.IsAny<Transakcii>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_WhenTransactionBelongsToAnotherPortfolio_ThrowsNotFoundException()
        {

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

            var existingTransaction = new Transakcii
            {
                Id = 1,
                PortfolioId = 1000,
                Hvid = 2,
                TipTransakcija = "Купување",
                KolicinaAkcii = 10,
                EdinecnaCenaAkcija = 100,
                Datum = new DateOnly(2026, 10, 1),
                Realna = true,
                Hv = securities[1]
            };

            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = "Купување",
                SharesQuantity = 10,
                SharesUnitPrice = 100,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(existingTransaction);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.Update(10, 1, transaction));

            Assert.AreEqual("TRANSACTION_NOT_FOUND", exception.Code);

            _securitiesRepositoryMock.Verify(
                r => r.GetByCode(It.IsAny<string>()),
                Times.Never);

            _transactionsRepositoryMock.Verify(
                r => r.Update(It.IsAny<Transakcii>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_WhenSecurityDoesNotExist_ThrowsNotFoundException()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var existingTransaction = new Transakcii
            {
                Id = 1,
                PortfolioId = 10,
                Hvid = 1,
                TipTransakcija = "Купување",
                KolicinaAkcii = 10,
                EdinecnaCenaAkcija = 100,
                Datum = new DateOnly(2026, 10, 1),
                Realna = true,
                Hv = security
            };

            var transaction = new Transaction
            {
                SecurityCode = "Invalid",
                TypeTransaction = "Купување",
                SharesQuantity = 10,
                SharesUnitPrice = 100,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(existingTransaction);

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("Invalid"))
                .Returns((HartiiOdVrednost?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.Update(10, 1, transaction));

            Assert.AreEqual("SECURITY_NOT_FOUND", exception.Code);

            _transactionsRepositoryMock.Verify(
                r => r.Update(It.IsAny<Transakcii>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_WhenSharesQuantityIsZero_ThrowsInvalidOperationException()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var existingTransaction = new Transakcii
            {
                Id = 1,
                PortfolioId = 10,
                Hvid = 1,
                TipTransakcija = "Купување",
                KolicinaAkcii = 10,
                EdinecnaCenaAkcija = 100,
                Datum = new DateOnly(2026, 10, 1),
                Realna = true,
                Hv = security
            };

            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = "Купување",
                SharesQuantity = 0,
                SharesUnitPrice = 100,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(existingTransaction);

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(security);

            var exception = Assert.ThrowsException<InvalidOperationException>(
                () => _service.Update(10, 1, transaction));

            Assert.AreEqual("Transaction quantity must be greater than 0.", exception.Message);

            _transactionsRepositoryMock.Verify(
                r => r.Update(It.IsAny<Transakcii>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_WhenSellingMoreSharesThanOwned_ThrowsBusinessRuleException()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var existingTransaction = new Transakcii
            {
                Id = 1,
                PortfolioId = 10,
                Hvid = 1,
                TipTransakcija = "Продавање",
                KolicinaAkcii = 5,
                EdinecnaCenaAkcija = 100,
                Datum = new DateOnly(2026, 10, 1),
                Realna = true,
                Hv = security
            };

            var transaction = new Transaction
            {
                SecurityCode = "ALK",
                TypeTransaction = "Продавање",
                SharesQuantity = 15,
                SharesUnitPrice = 120,
                IsReal = true,
                Date = new DateOnly(2026, 10, 8)
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(existingTransaction);

            _securitiesRepositoryMock
                .Setup(r => r.GetByCode("ALK"))
                .Returns(security);

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(10))
                .Returns(new List<Transakcii>());

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSharesAtDate(10, 1, true, new DateOnly(2026, 10, 8), 1))
                .Returns(10);

            var exception = Assert.ThrowsException<BusinessRuleException>(
                () => _service.Update(10, 1, transaction));

            Assert.AreEqual("SELL_MORE_THAN_OWNED", exception.Code);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedSharesAtDate(10, 1, true, new DateOnly(2026, 10, 8), 1),
                Times.Once);

            _transactionsRepositoryMock.Verify(
                r => r.Update(It.IsAny<Transakcii>()),
                Times.Never);
        }

        [TestMethod]
        public void Delete_WhenTransactionExists_ReturnsDeletedTransaction()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var transaction = new Transakcii
            {
                Id = 1,
                PortfolioId = 10,
                Hvid = 1,
                TipTransakcija = "Купување",
                KolicinaAkcii = 10,
                EdinecnaCenaAkcija = 100,
                Iznos = 1000m,
                BerzanskaProvizija = 0.5m,
                BrokerskaProvizija = 1m,
                Cdhvprovizija = 0.2m,
                Datum = new DateOnly(2026, 10, 8),
                Realna = true,
                Hv = security
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(transaction);

            _transactionsRepositoryMock
                .Setup(r => r.Delete(transaction))
                .Returns(transaction);

            var result = _service.Delete(1);

            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(1, result.SecurityId);
            Assert.AreEqual("ALK", result.SecurityCode);
            Assert.AreEqual(10, result.SharesQuantity);
            Assert.AreEqual(1000m, result.Amount);

            _transactionsRepositoryMock.Verify(
                r => r.GetById(1),
                Times.Once);

            _transactionsRepositoryMock.Verify(
                r => r.Delete(transaction),
                Times.Once);
        }

        [TestMethod]
        public void Delete_WhenTransactionDoesNotExist_ThrowsNotFoundException()
        {
            _transactionsRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((Transakcii?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.Delete(1));

            Assert.AreEqual("TRANSACTION_NOT_FOUND", exception.Code);

            _transactionsRepositoryMock.Verify(
                r => r.Delete(It.IsAny<Transakcii>()),
                Times.Never);
        }


        [TestMethod]
        public void GetAnalytics_RealPortfolio_ReturnsCorrectAnalytics()
        {
            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Id = 1,
                    PortfolioId = 10,
                    Hvid = 1,
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 10,
                    EdinecnaCenaAkcija = 100,
                    Iznos = 1000m,
                    BerzanskaProvizija = 0.5m,
                    BrokerskaProvizija = 1m,
                    Cdhvprovizija = 0.2m,
                    Realna = true
                },
                new Transakcii
                {
                    Id = 2,
                    PortfolioId = 10,
                    Hvid = 1,
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 5,
                    EdinecnaCenaAkcija = 120,
                    Iznos = 600m,
                    BerzanskaProvizija = 0.5m,
                    BrokerskaProvizija = 1m,
                    Cdhvprovizija = 0.2m,
                    Realna = true
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(10))
                .Returns(transactions);

            _portfolioReturnsServiceMock
                .Setup(r => r.CalculateSummary(10))
                .Returns(new PortfolioReturnsSummary
                {
                    TotalDividends = 100m,
                    TotalTaxes = 50m
                });

            var result = _service.GetAnalytics(10, true);

            Assert.AreEqual(700m, result.TotalRevenue);
            Assert.AreEqual(1027.2m, result.TotalExpenses);
            Assert.AreEqual(-327.2m, result.Balance);
            Assert.AreEqual(50m, result.Taxes);
            Assert.IsTrue(result.IsReal);

            _transactionsRepositoryMock.Verify(
                r => r.GetByPortfolioId(10),
                Times.Once);

            _portfolioReturnsServiceMock.Verify(
                r => r.CalculateSummary(10),
                Times.Once);
        }

        [TestMethod]
        public void GetAnalytics_VirtualPortfolio_ReturnsCorrectAnalytics()
        {
            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Id = 1,
                    PortfolioId = 10,
                    Hvid = 1,
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 10,
                    EdinecnaCenaAkcija = 100,
                    Iznos = 1000m,
                    BerzanskaProvizija = 0.5m,
                    BrokerskaProvizija = 1m,
                    Cdhvprovizija = 0.2m,
                    Realna = false
                },
                new Transakcii
                {
                    Id = 2,
                    PortfolioId = 10,
                    Hvid = 1,
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 5,
                    EdinecnaCenaAkcija = 120,
                    Iznos = 600m,
                    BerzanskaProvizija = 0.5m,
                    BrokerskaProvizija = 1m,
                    Cdhvprovizija = 0.2m,
                    Realna = false
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(10))
                .Returns(transactions);

            _portfolioReturnsServiceMock
                .Setup(r => r.CalculateSummary(10))
                .Returns(new PortfolioReturnsSummary
                {
                    TotalDividends = 100m,
                    TotalTaxes = 50m
                });

            var result = _service.GetAnalytics(10, false);

            Assert.AreEqual(600m, result.TotalRevenue);
            Assert.AreEqual(1027.2m, result.TotalExpenses);
            Assert.AreEqual(-427.2m, result.Balance);
            Assert.AreEqual(0m, result.Taxes);
            Assert.IsFalse(result.IsReal);

            _transactionsRepositoryMock.Verify(
                r => r.GetByPortfolioId(10),
                Times.Once);

            _portfolioReturnsServiceMock.Verify(
                r => r.CalculateSummary(10),
                Times.Once);
        }

        [TestMethod]
        public void GetAnalytics_RealPortfolio_IgnoresVirtualTransactions()
        {
            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Id = 1,
                    PortfolioId = 10,
                    Hvid = 1,
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 10,
                    EdinecnaCenaAkcija = 100,
                    Iznos = 1000m,
                    BerzanskaProvizija = 0,
                    BrokerskaProvizija = 0,
                    Cdhvprovizija = 0,
                    Realna = true
                },
                new Transakcii
                {
                    Id = 2,
                    PortfolioId = 10,
                    Hvid = 1,
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 20,
                    EdinecnaCenaAkcija = 500,
                    Iznos = 10000m,
                    BerzanskaProvizija = 0,
                    BrokerskaProvizija = 0,
                    Cdhvprovizija = 0,
                    Realna = false
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(10))
                .Returns(transactions);

            _portfolioReturnsServiceMock
                .Setup(r => r.CalculateSummary(10))
                .Returns(new PortfolioReturnsSummary
                {
                    TotalDividends = 0,
                    TotalTaxes = 0
                });

            var result = _service.GetAnalytics(10, true);

            Assert.AreEqual(0m, result.TotalRevenue);
            Assert.AreEqual(1000m, result.TotalExpenses);
            Assert.AreEqual(-1000m, result.Balance);
            Assert.AreEqual(0m, result.Taxes);
            Assert.IsTrue(result.IsReal);

            _transactionsRepositoryMock.Verify(
                r => r.GetByPortfolioId(10),
                Times.Once);

            _portfolioReturnsServiceMock.Verify(
                r => r.CalculateSummary(10),
                Times.Once);
        }

        [TestMethod]
        public void GetAnalytics_WhenNoTransactions_ReturnsZeroAnalytics()
        {
            _transactionsRepositoryMock
                .Setup(r => r.GetByPortfolioId(10))
                .Returns(new List<Transakcii>());

            _portfolioReturnsServiceMock
                .Setup(r => r.CalculateSummary(10))
                .Returns(new PortfolioReturnsSummary
                {
                    TotalDividends = 0m,
                    TotalTaxes = 0m
                });

            var result = _service.GetAnalytics(10, true);

            Assert.AreEqual(0m, result.TotalRevenue);
            Assert.AreEqual(0m, result.TotalExpenses);
            Assert.AreEqual(0m, result.Balance);
            Assert.AreEqual(0m, result.Taxes);
            Assert.IsTrue(result.IsReal);

            _transactionsRepositoryMock.Verify(
             r => r.GetByPortfolioId(10),
             Times.Once);

            _portfolioReturnsServiceMock.Verify(
                r => r.CalculateSummary(10),
                Times.Once);
        }

        [TestMethod]
        public void FindPriceTrend_WithPeriodAndResolution_ReturnsMappedPriceTrend()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var dailyPrices = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 100m,
                    Hv = security
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = 105m,
                    Hv = security
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(new List<int> { 1 });

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
                    It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1 })),
                    PriceTrendPeriod.Monthly,
                    PriceTrendResolution.Day))
                .Returns(dailyPrices);

            var result = _service.FindPriceTrend(10, PriceTrendPeriod.Monthly, PriceTrendResolution.Day).ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(new DateTime(2026, 10, 1), result[0].Date);
            Assert.AreEqual(1, result[0].SecurityId);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(100m, result[0].Price);

            Assert.AreEqual(new DateTime(2026, 10, 2), result[1].Date);
            Assert.AreEqual(1, result[1].SecurityId);
            Assert.AreEqual("ALK", result[1].SecurityCode);
            Assert.AreEqual(105m, result[1].Price);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedSecuritiesIds(10),
                Times.Once);

            _dailyTurnoverRepository.Verify(
                r => r.GetBySecuritiesIds(
                    It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1 })),
                    PriceTrendPeriod.Monthly,
                    PriceTrendResolution.Day),
                Times.Once);
        }

        [TestMethod]
        public void FindPriceTrend_WhenPeriodAndResolutionAreMissing_UsesDefaultValues()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var dailyPrices = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 100m,
                    Hv = security
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(new List<int> { 1, 2, 3 });

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
                    It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1, 2, 3 })),
                    PriceTrendPeriod.Monthly,
                    PriceTrendResolution.Day))
                .Returns(dailyPrices);

            var result = _service.FindPriceTrend(10, null, null).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(100m, result[0].Price);

            _dailyTurnoverRepository.Verify(
              r => r.GetBySecuritiesIds(
                  It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1,2, 3 })),
                  PriceTrendPeriod.Monthly,
                  PriceTrendResolution.Day),
              Times.Once);
        }

        [TestMethod]
        public void FindPriceTrend_WithMoreThanThreeSecurities_UsesWeeklyResolution()
        {
            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(new List<int> { 1, 2, 3, 4 });

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
                    It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1, 2, 3, 4 })),
                    PriceTrendPeriod.Monthly,
                    PriceTrendResolution.Week))
                .Returns(new List<DnevenPromet>());

            var result = _service.FindPriceTrend(10, PriceTrendPeriod.Monthly, null).ToList();

            Assert.AreEqual(0, result.Count);

            _dailyTurnoverRepository.Verify(
              r => r.GetBySecuritiesIds(
                  It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1, 2, 3, 4 })),
                  PriceTrendPeriod.Monthly,
                  PriceTrendResolution.Week),
              Times.Once);
        }

        [TestMethod]
        public void FindPriceTrend_WhenNoSecuritiesAreOwned_ReturnsEmptyList()
        {
            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(new List<int>());

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
                    It.Is<List<int>>(ids => ids.Count == 0),
                    PriceTrendPeriod.Monthly,
                    PriceTrendResolution.Day))
                .Returns(new List<DnevenPromet>());

            var result = _service.FindPriceTrend(10, null, null).ToList();

            Assert.AreEqual(0, result.Count);

            _transactionsRepositoryMock.Verify(
                r => r.GetOwnedSecuritiesIds(10),
                Times.Once);

            _dailyTurnoverRepository.Verify(
             r => r.GetBySecuritiesIds(
                 It.Is<List<int>>(ids => ids.Count == 0),
                 PriceTrendPeriod.Monthly,
                 PriceTrendResolution.Day),
             Times.Once);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WithValidData_ReturnsCorrectReport()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var dailyPrices = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 100m,
                    Hv = security
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = 120m,
                    Hv = security
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 3),
                    CenaPoslednaTransakcija = 110m,
                    Hv = security
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(new List<int> { 1 });

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
              It.Is<List<int>>(ids => ids.SequenceEqual(new List<int> { 1 })),
              PriceTrendPeriod.Monthly,
              PriceTrendResolution.Day))
          .Returns(dailyPrices);

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly, 
                PriceTrendResolution.Day, null).ToList();

            Assert.AreEqual(1, result.Count);

            var report = result[0];

            Assert.AreEqual("ALK", report.SecurityCode);
            Assert.AreEqual("Monthly", report.Period);

            Assert.AreEqual(new DateTime(2026, 10, 1), report.StartDate);
            Assert.AreEqual(new DateTime(2026, 10, 3), report.EndDate);

            Assert.AreEqual(3, report.NumberOfMeasurements);

            Assert.AreEqual(100m, report.StartPrice);
            Assert.AreEqual(110m, report.EndPrice);

            Assert.AreEqual(100m, report.LowestPrice);
            Assert.AreEqual(120m, report.HighestPrice);

            Assert.AreEqual(110m, report.AveragePrice);

            Assert.AreEqual(10m, report.PriceChange);
            Assert.AreEqual(10m, report.PriceChangePercent);

            Assert.AreEqual("Increasing", report.Trend);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WhenPriceDecreases_ReturnsDecreasingTrend()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var dailyPrices = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 200m,
                    Hv = security
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = 150m,
                    Hv = security
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(new List<int> { 1 });

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
              It.IsAny<List<int>>(),
              PriceTrendPeriod.Monthly,
              PriceTrendResolution.Day))
          .Returns(dailyPrices);

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly,
                PriceTrendResolution.Day, null).ToList();

            Assert.AreEqual(1, result.Count);

            var report = result[0];

            Assert.AreEqual(200m, report.StartPrice);
            Assert.AreEqual(150m, report.EndPrice);

            Assert.AreEqual(-50m, report.PriceChange);
            Assert.AreEqual(-25m, report.PriceChangePercent);

            Assert.AreEqual("Decreasing", report.Trend);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WhenPriceIsUnchanged_ReturnsStableTrend()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var dailyPrices = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 100m,
                    Hv = security
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = 100m,
                    Hv = security
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(new List<int> { 1 });

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
              It.IsAny<List<int>>(),
              PriceTrendPeriod.Monthly,
              PriceTrendResolution.Day))
          .Returns(dailyPrices);

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly,
                PriceTrendResolution.Day, null).ToList();

            Assert.AreEqual(1, result.Count);

            var report = result[0];

            Assert.AreEqual(0m, report.PriceChange);
            Assert.AreEqual(0m, report.PriceChangePercent);

            Assert.AreEqual("Stable", report.Trend);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_IgnoresMeasurementsWithoutPrice()
        {
            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var dailyPrices = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = null,
                    Hv = security
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = 100m,
                    Hv = security
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 3),
                    CenaPoslednaTransakcija = 120m,
                    Hv = security
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(new List<int> { 1 });

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
              It.IsAny<List<int>>(),
              PriceTrendPeriod.Monthly,
              PriceTrendResolution.Day))
          .Returns(dailyPrices);

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly,
                PriceTrendResolution.Day, null).ToList();

            Assert.AreEqual(1, result.Count);

            var report = result[0];

            Assert.AreEqual(2, report.NumberOfMeasurements);

            Assert.AreEqual(new DateTime(2026, 10, 2), report.StartDate);
            Assert.AreEqual(new DateTime(2026, 10, 3), report.EndDate);

            Assert.AreEqual(100m, report.StartPrice);
            Assert.AreEqual(120m, report.EndPrice);

            Assert.AreEqual(100m, report.LowestPrice);
            Assert.AreEqual(120m, report.HighestPrice);

            Assert.AreEqual(110m, report.AveragePrice);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WithSecurityCode_ReturnsOnlyMatchingSecurity()
        {
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

            var dailyPrices = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 100m  ,
                    Hv = securities[0]
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = 110m,
                    Hv = securities[0]
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 200m,
                    Hv = securities[1]
                }
            };

            _transactionsRepositoryMock
               .Setup(r => r.GetOwnedSecuritiesIds(10))
               .Returns(new List<int> { 1, 2, 3 });

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
              It.IsAny<List<int>>(),
              PriceTrendPeriod.Monthly,
              PriceTrendResolution.Day))
          .Returns(dailyPrices);

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly,
                PriceTrendResolution.Day, "ALK").ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("ALK", result[0].SecurityCode);
            Assert.AreEqual(2, result[0].NumberOfMeasurements);
            Assert.AreEqual(100m, result[0].StartPrice);
            Assert.AreEqual(110m, result[0].EndPrice);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WithMultipleSecurities_ReturnsSeparateReports()
        {
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

            var dailyPrices = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 100m,
                    Hv = securities[0]
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = 120m,
                    Hv = securities[0]
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 200m,
                    Hv = securities[1]
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = 180m,
                    Hv = securities[1]
                }
            };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(new List<int> { 1, 2 });

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
                    It.IsAny<List<int>>(),
                    PriceTrendPeriod.Monthly,
                    PriceTrendResolution.Day))
                .Returns(dailyPrices);

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly, 
                PriceTrendResolution.Day, null).ToList();

            Assert.AreEqual(2, result.Count);

            var alkReport = result.Single(r => r.SecurityCode == "ALK");

            Assert.AreEqual(100m, alkReport.StartPrice);
            Assert.AreEqual(120m, alkReport.EndPrice);
            Assert.AreEqual("Increasing", alkReport.Trend);

            var kmbReport = result.Single(r => r.SecurityCode == "KMB");
            Assert.AreEqual(200m, kmbReport.StartPrice);
            Assert.AreEqual(180, kmbReport.EndPrice);
            Assert.AreEqual("Decreasing", kmbReport.Trend);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WhenNoPriceData_ReturnsEmptyList()
        {
            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(new List<int> { 1, 2 });

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
                    It.IsAny<List<int>>(),
                    PriceTrendPeriod.Monthly,
                    PriceTrendResolution.Day))
                .Returns(new List<DnevenPromet>());

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly,
                PriceTrendResolution.Day, null).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WhenStartPriceIsZero_ReturnsZeroPercentChange()
        {
           var security = new HartiiOdVrednost
           {
                Id = 1,
                Kod = "ALK"
           };

           var dailyPrices = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 0m,
                    Hv = security
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = 100m,
                    Hv = security
                }
            };

            _transactionsRepositoryMock
               .Setup(r => r.GetOwnedSecuritiesIds(10))
               .Returns(new List<int> { 1 });

            _dailyTurnoverRepository
                .Setup(r => r.GetBySecuritiesIds(
                    It.IsAny<List<int>>(),
                    PriceTrendPeriod.Monthly,
                    PriceTrendResolution.Day))
                .Returns(dailyPrices);

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly,
                PriceTrendResolution.Day, null).ToList();

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual(0m, result[0].StartPrice);
            Assert.AreEqual(100m, result[0].EndPrice);
            Assert.AreEqual(100m, result[0].PriceChange);
            Assert.AreEqual(0m, result[0].PriceChangePercent);
            Assert.AreEqual("Increasing", result[0].Trend);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WhenYearlyAndFiveOrFewerSecurities_UsesMonthlyResolution()
        {
            var securitiesIds = new List<int> { 1, 2, 3, 4, 5 };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(securitiesIds);

            _dailyTurnoverRepository
               .Setup(r => r.GetBySecuritiesIds(
                   securitiesIds,
                   PriceTrendPeriod.Yearly,
                   PriceTrendResolution.Month))
               .Returns(new List<DnevenPromet>());

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Yearly, null, null).ToList();

            Assert.AreEqual(0, result.Count);

            _dailyTurnoverRepository.Verify(
                r => r.GetBySecuritiesIds(
                    securitiesIds,
                    PriceTrendPeriod.Yearly,
                    PriceTrendResolution.Month),
                Times.Once);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WhenYearlyAndMoreThanFiveSecurities_UsesQuarterlyResolution()
        {
            var securitiesIds = new List<int> { 1, 2, 3, 4, 5, 6 };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(securitiesIds);

            _dailyTurnoverRepository
               .Setup(r => r.GetBySecuritiesIds(
                   securitiesIds,
                   PriceTrendPeriod.Yearly,
                   PriceTrendResolution.Quarter))
               .Returns(new List<DnevenPromet>());

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Yearly, null, null).ToList();

            Assert.AreEqual(0, result.Count);

            _dailyTurnoverRepository.Verify(
                r => r.GetBySecuritiesIds(
                    securitiesIds,
                    PriceTrendPeriod.Yearly,
                    PriceTrendResolution.Quarter),
                Times.Once);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WithSecurityCodeAndMissingResolution_UsesDailyResolution()
        {
            var securitiesIds = new List<int> { 1, 2, 3, 4, 5, 6 };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(securitiesIds);

            _dailyTurnoverRepository
               .Setup(r => r.GetBySecuritiesIds(
                   securitiesIds,
                   PriceTrendPeriod.Monthly,
                   PriceTrendResolution.Day))
               .Returns(new List<DnevenPromet>());

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly, null, "ALK").ToList();

            Assert.AreEqual(0, result.Count);

            _dailyTurnoverRepository.Verify(
                r => r.GetBySecuritiesIds(
                    securitiesIds,
                    PriceTrendPeriod.Monthly,
                    PriceTrendResolution.Day),
                Times.Once);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WithWhiteSpaceSecurityCode_DoesNotFilterAndUsesWeeklyResolution()
        {
            var securitiesIds = new List<int> { 1, 2, 3, 4 };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(securitiesIds);

            _dailyTurnoverRepository
               .Setup(r => r.GetBySecuritiesIds(
                   securitiesIds,
                   PriceTrendPeriod.Monthly,
                   PriceTrendResolution.Week))
               .Returns(new List<DnevenPromet>());

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly, null, "  ").ToList();

            Assert.AreEqual(0, result.Count);

            _dailyTurnoverRepository.Verify(
                r => r.GetBySecuritiesIds(
                    securitiesIds,
                    PriceTrendPeriod.Monthly,
                    PriceTrendResolution.Week),
                Times.Once);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_WhenSecurityHasOnlyNullPrices_ExcludesThatSecurity()
        {
            var securitiesIds = new List<int> { 1, 2 };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(securitiesIds);

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

            var dailyPrices = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = null,
                    Hv = securities[0]
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = null,
                    Hv = securities[0]
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 200m,
                    Hv = securities[1]
                },
                new DnevenPromet
                {
                    Hvid = 2,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = 220m,
                    Hv = securities[1]
                }
            };

            _dailyTurnoverRepository
               .Setup(r => r.GetBySecuritiesIds(
                   securitiesIds,
                   PriceTrendPeriod.Monthly,
                   PriceTrendResolution.Day))
               .Returns(dailyPrices);

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly, 
                PriceTrendResolution.Day, null).ToList();

            Assert.AreEqual(1, result.Count);

            Assert.AreEqual("KMB", result[0].SecurityCode);
            Assert.AreEqual(2, result[0].NumberOfMeasurements);
            Assert.AreEqual(200m, result[0].StartPrice);
            Assert.AreEqual(220m, result[0].EndPrice);
        }

        [TestMethod]
        public void GetSecuritiesPriceTrendReport_RoundsPercentageChangeAndAveragePrice()
        {
            var securitiesIds = new List<int> { 1 };

            _transactionsRepositoryMock
                .Setup(r => r.GetOwnedSecuritiesIds(10))
                .Returns(securitiesIds);

            var security = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK"
            };

            var dailyPrices = new List<DnevenPromet>
            {
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 1),
                    CenaPoslednaTransakcija = 3m,
                    Hv = security
                },
                new DnevenPromet
                {
                    Hvid = 1,
                    Datum = new DateTime(2026, 10, 2),
                    CenaPoslednaTransakcija = 4m,
                    Hv = security
                }
            };

            _dailyTurnoverRepository
               .Setup(r => r.GetBySecuritiesIds(
                   securitiesIds,
                   PriceTrendPeriod.Monthly,
                   PriceTrendResolution.Day))
               .Returns(dailyPrices);

            var result = _service.GetSecuritiesPriceTrendReport(10, PriceTrendPeriod.Monthly,
                PriceTrendResolution.Day, null).Single();

            Assert.AreEqual(3m, result.StartPrice);
            Assert.AreEqual(4m, result.EndPrice);
            Assert.AreEqual(3.5m, result.AveragePrice);
            Assert.AreEqual(1m, result.PriceChange);
            Assert.AreEqual(33.33m, result.PriceChangePercent);
            Assert.AreEqual("Increasing", result.Trend);
        }
    }
}
