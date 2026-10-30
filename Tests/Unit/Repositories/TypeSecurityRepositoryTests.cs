using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class TypeSecurityRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private TypeSecurityRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new TypeSecurityRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var types = new List<TipHv>
            {
                new TipHv
                {
                    Id = 1,
                    Ime = "Shares",
                    DateModified = DateTime.Today
                },
                new TipHv
                {
                    Id = 2,
                    Ime = "Bonds",
                    DateModified = DateTime.Today
                }
            };

            _context.TipHv.AddRange(types);
            _context.SaveChanges();
        }

        [TestMethod]
        public void GetById_WhenTypeExists_ReturnsType()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Shares", result.Ime);
        }

        [TestMethod]
        public void GetById_WhenTypeDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAll_ReturnsAllTypes()
        {
            var result = _repository.GetAll().ToList();

            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEquivalent(
                new[] { "Shares", "Bonds" },
                result.Select(t => t.Ime).ToArray());
        }

        [TestMethod]
        public void GetAll_WhenNoTypesExist_ReturnsEmptyCollection()
        {
            _context.TipHv.RemoveRange(_context.TipHv.ToList());
            _context.SaveChanges();

            var result = _repository.GetAll().ToList();

            Assert.AreEqual(0, result.Count);
        }
    }
}
