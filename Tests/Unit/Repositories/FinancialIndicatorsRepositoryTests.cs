using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class FinancialIndicatorsRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private FinancialIndicatorsRepository _repository = null;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new FinancialIndicatorsRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public void GetAll_WhenRecordsExist_ReturnsAllRecords()
        {
            var indicators = new List<FinansiskiPokazateli>
            {
                new FinansiskiPokazateli { Id = 1},
                new FinansiskiPokazateli { Id = 2},
                new FinansiskiPokazateli { Id = 3}
            };

            _context.FinansiskiPokazateli.AddRange(indicators);
            _context.SaveChanges();

            var result = _repository.GetAll().ToList();

            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3 },
                result.Select(x => x.Id).ToArray());
        }

        [TestMethod]
        public void GetAll_WhenNoRecordsExist_ReturnsEmptyCollection()
        {
            var result = _repository.GetAll().ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
