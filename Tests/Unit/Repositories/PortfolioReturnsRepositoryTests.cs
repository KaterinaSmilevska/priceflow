using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class PortfolioReturnsRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private PortfolioReturnsRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new PortfolioReturnsRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public void GetByPortfolioId_WhenReturnsExist_ReturnsOnlyMatchingPortfolioReturns()
        {
            var returns = new List<PortfolioPrinosi>
            {
                new PortfolioPrinosi
                {
                    PortfolioId = 1,
                    Datum = new DateOnly(2026, 10, 10)
                },
                new PortfolioPrinosi
                {
                    PortfolioId = 1,
                    Datum = new DateOnly(2026, 10, 11)
                },
                new PortfolioPrinosi
                {
                    PortfolioId = 2,
                    Datum = new DateOnly(2026, 10, 10)
                },
            };

            _context.PortfolioPrinosi.AddRange(returns);
            _context.SaveChanges();

            var result = _repository.GetByPortfolioId(1).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(x => x.PortfolioId == 1));
        }

        [TestMethod]
        public void GetByPortfolioId_WhenNoReturnsExist_ReturnsEmptyCollection()
        {
            var result = _repository.GetByPortfolioId(1).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetByPortfolioIdForPeriod_ReturnsOnlyRecordsWithinInclusiveDateRange()
        {
            var from = new DateOnly(2026, 10, 10);
            var to = new DateOnly(2026, 10, 12);

            var returns = new List<PortfolioPrinosi>
            {
                new PortfolioPrinosi
                {
                    PortfolioId = 1,
                    Datum = from.AddDays(-1)
                },
                new PortfolioPrinosi
                {
                    PortfolioId = 1,
                    Datum = from
                },
                new PortfolioPrinosi
                {
                    PortfolioId = 1,
                    Datum = from.AddDays(1)
                },
                new PortfolioPrinosi
                {
                    PortfolioId = 1,
                    Datum = to
                },
                new PortfolioPrinosi
                {
                    PortfolioId = 1,
                    Datum = to.AddDays(1)
                },
                new PortfolioPrinosi
                {
                    PortfolioId = 2,
                    Datum = to.AddDays(1)
                }
            };

            _context.PortfolioPrinosi.AddRange(returns);
            _context.SaveChanges();

            var result = _repository
                .GetByPortfolioIdForPeriod(1, from, to)
                .ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(x =>
            x.PortfolioId == 1 &&
            x.Datum >= from &&
            x.Datum <= to));
        }

        [TestMethod]
        public void GetByPortfolioIdForPeriod_WhenNoRecordsMatch_ReturnsEmptyCollection()
        {
            var from = new DateOnly(2026, 10, 10);
            var to = new DateOnly(2026, 10, 12);

            var portfolioReturn = new PortfolioPrinosi
            {
                PortfolioId = 1,
                Datum = new DateOnly(2026, 10, 15)
            };

            _context.PortfolioPrinosi.Add(portfolioReturn);
            _context.SaveChanges();

            var result = _repository
                .GetByPortfolioIdForPeriod(1, from, to)
                .ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Add_SavesAndReturnsPortfolioReturn()
        {
            var portfolioReturn = new PortfolioPrinosi
            {
                PortfolioId = 1,
                Datum = new DateOnly(2026, 10, 19)
            };

            var result = _repository.Add(portfolioReturn);

            Assert.IsNotNull(result);
            Assert.AreSame(portfolioReturn, result);

            var saved = _context.PortfolioPrinosi
                .SingleOrDefault(x =>
                x.PortfolioId == 1 &&
                x.Datum == new DateOnly(2026, 10, 19));

            Assert.IsNotNull(saved);
        }
    }
}
