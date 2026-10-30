using Microsoft.AspNetCore.Mvc;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Services;

namespace Tests.Unit.Controllers
{
    [TestClass]
    public class TransactionsControllerTests
    {
        private Mock<ITransactionsService> _transactionsServiceMock = null!;
        private Mock<IPortfolioValueService> _portfolioValueServiceMock = null!;
        private TransactionsController _controller = null!;

        private const int PortfolioId = 1;
        private const int TransactionId = 10;
        private const string SecurityCode = "ALK";

        [TestInitialize]
        public void Setup()
        {
            _transactionsServiceMock = new Mock<ITransactionsService>();
            _portfolioValueServiceMock = new Mock<IPortfolioValueService>();
            _controller = new TransactionsController(_transactionsServiceMock.Object, _portfolioValueServiceMock.Object);
        }

        [TestMethod]
        public void GetAll_WhenTransactionsExist_ReturnsOkWithTransactions()
        {
            var transactions = new List<Transaction>
            {
                new Transaction { Id = 1 },
                new Transaction { Id = 2 }
            };

            _transactionsServiceMock
                .Setup(s => s.FindByPortfolioId(PortfolioId))
                .Returns(transactions);

            var result = _controller.GetAll(PortfolioId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(transactions, ok.Value);

            _transactionsServiceMock.Verify(
                s => s.FindByPortfolioId(PortfolioId),
                Times.Once);
        }

        [TestMethod]
        public void GetAll_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _transactionsServiceMock
                .Setup(s => s.FindByPortfolioId(PortfolioId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetAll(PortfolioId);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetOwnedShares_WhenSuccessful_ReturnsOkWithOwnedShares()
        {
            const int expectedShares = 10;

            _transactionsServiceMock
                .Setup(s => s.FindOwnedShares(PortfolioId, SecurityCode, true))
                .Returns(expectedShares);

            var result = _controller.GetOwnedShares(PortfolioId, SecurityCode, true);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreEqual(expectedShares, ok.Value);

            _transactionsServiceMock.Verify(
                s => s.FindOwnedShares(PortfolioId, SecurityCode, true),
                Times.Once);
        }

        [TestMethod]
        public void GetOwnedShares_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _transactionsServiceMock
                .Setup(s => s.FindOwnedShares(PortfolioId, SecurityCode, true))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetOwnedShares(PortfolioId, SecurityCode, true);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetOwnedSharesAtDate_WhenSuccessful_ReturnsOkWithOwnedShares()
        {
            const int expectedShares = 10;
            var date = new DateOnly(2026, 10, 12);
            int? excludedTransactionId = 3;

            _transactionsServiceMock
                .Setup(s => s.FindOwnedSharesAtDate(PortfolioId, SecurityCode, true, date, excludedTransactionId))
                .Returns(expectedShares);

            var result = _controller.GetOwnedSharesAtDate(PortfolioId, SecurityCode, true, date, excludedTransactionId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreEqual(expectedShares, ok.Value);

            _transactionsServiceMock.Verify(
                s => s.FindOwnedSharesAtDate(PortfolioId, SecurityCode, true, date, excludedTransactionId),
                Times.Once);
        }

        [TestMethod]
        public void GetOwnedSharesAtDate_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var date = new DateOnly(2026, 10, 12);

            _transactionsServiceMock
                .Setup(s => s.FindOwnedSharesAtDate(PortfolioId, SecurityCode, true, date, null))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetOwnedSharesAtDate(PortfolioId, SecurityCode, true, date, null);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Add_WhenSuccessful_ReturnsOkWithCreatedTransaction()
        {
            var transaction = new Transaction
            {
                Id = TransactionId
            };

            _transactionsServiceMock
                .Setup(s => s.Add(PortfolioId, transaction))
                .Returns(transaction);

            var result = _controller.Add(PortfolioId, transaction);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(transaction, ok.Value);

            _transactionsServiceMock.Verify(
                s => s.Add(PortfolioId, transaction),
                Times.Once);
        }

        [TestMethod]
        public void Add_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var transaction = new Transaction
            {
                Id = TransactionId
            };

            _transactionsServiceMock
                .Setup(s => s.Add(PortfolioId, transaction))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Add(PortfolioId, transaction);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Update_WhenSuccessful_ReturnsOkWithUpdatedTransaction()
        {
            var transaction = new Transaction
            {
                Id = TransactionId
            };

            _transactionsServiceMock
                .Setup(s => s.Update(PortfolioId, TransactionId, transaction))
                .Returns(transaction);

            var result = _controller.Update(PortfolioId, TransactionId, transaction);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(transaction, ok.Value);

            _transactionsServiceMock.Verify(
                s => s.Update(PortfolioId, TransactionId, transaction),
                Times.Once);
        }

        [TestMethod]
        public void Update_WhenIdsDoNotMatch_ReturnsBadRequest()
        {
            var transaction = new Transaction
            {
                Id = 1000
            };

            var result = _controller.Update(PortfolioId, TransactionId, transaction);
            var badRequest = result.Result as BadRequestObjectResult;

            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);

            _transactionsServiceMock.Verify(
                s => s.Update(It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Transaction>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var transaction = new Transaction
            {
                Id = TransactionId
            };

            _transactionsServiceMock
                .Setup(s => s.Update(PortfolioId, TransactionId, transaction))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Update(PortfolioId, TransactionId  , transaction);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void Delete_WhenSuccessful_ReturnsOkWithDeletedTransaction()
        {
            var transaction = new Transaction
            { 
                Id = TransactionId 
            };

            _transactionsServiceMock
                .Setup(s => s.Delete(TransactionId))
                .Returns(transaction);

            var result = _controller.Delete(TransactionId);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(transaction, ok.Value);

            _transactionsServiceMock.Verify(
                s => s.Delete(TransactionId),
                Times.Once);
        }

        [TestMethod]
        public void Delete_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _transactionsServiceMock
                .Setup(s => s.Delete(TransactionId))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.Delete(TransactionId);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetAnalytics_WhenSuccessful_ReturnsOkWithAnalytics()
        {
            var analytics = new PortfolioAnalytics();

            _transactionsServiceMock
                .Setup(s => s.GetAnalytics(PortfolioId, true))
                .Returns(analytics);

            var result = _controller.GetAnalytics(PortfolioId, true);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(analytics, ok.Value);

            _transactionsServiceMock.Verify(
                s => s.GetAnalytics(PortfolioId, true),
                Times.Once);
        }

        [TestMethod]
        public void GetAnalytics_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _transactionsServiceMock
                .Setup(s => s.GetAnalytics(PortfolioId, true))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetAnalytics(PortfolioId, true);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }

        [TestMethod]
        public void GetPortfolioValue_WhenSuccessful_ReturnsOkWithPortfolioValue()
        {
            var value = new List<PortfolioValue>
            {
                new PortfolioValue(),
                new PortfolioValue()
            };

            _portfolioValueServiceMock
                .Setup(s => s.GetCurrentValue(PortfolioId, true))
                .Returns(value);

            var result = _controller.GetPortfolioValue(PortfolioId, true);
            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(value, ok.Value);

            _portfolioValueServiceMock.Verify(
                s => s.GetCurrentValue(PortfolioId, true),
                Times.Once);
        }

        [TestMethod]
        public void GetPortfolioValue_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            _portfolioValueServiceMock
                .Setup(s => s.GetCurrentValue(PortfolioId, true))
                .Throws(new Exception("Unexpected error"));

            var result = _controller.GetPortfolioValue(PortfolioId, true);
            var error = result.Result as ObjectResult;

            Assert.IsNotNull(error);
            Assert.AreEqual(500, error.StatusCode);
        }
    }
}
