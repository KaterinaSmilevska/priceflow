using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class SectorsRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private SectorsRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new SectorsRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var banking = new Sektori
            {
                Id = 1,
                Ime = "Banking"
            };

            var pharmaceuticals = new Sektori
            {
                Id = 2,
                Ime = "Pharmaceuticals"
            };

            var industry = new Sektori
            {
                Id = 3,
                Ime = "Industry"
            };

            _context.Sektori.AddRange(banking, pharmaceuticals, industry);
            _context.SaveChanges();
        }

        [TestMethod]
        public void GetById_WhenSectorExists_ReturnsSector()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Banking", result.Ime);
        }

        [TestMethod]
        public void GetById_WhenSectorDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAll_WhenSectorsExist_ReturnsAllSectors()
        {
            var result = _repository.GetAll().ToList();

            Assert.AreEqual(3, result.Count);

            CollectionAssert.AreEquivalent(
                new[] { "Banking", "Pharmaceuticals", "Industry" },
                result.Select(s => s.Ime).ToList());
        }

        [TestMethod]
        public void GetAll_WhenNoSectorsExist_ReturnsEmptyCollection()
        {
            _context.Sektori.RemoveRange(_context.Sektori);
            _context.SaveChanges();

            var result = _repository.GetAll().ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
