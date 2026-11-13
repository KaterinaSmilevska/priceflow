using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class IssuersRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private IssuersRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new IssuersRepository(_context);
        }


        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        [TestMethod]
        public void GetById_WhenIssuerExist_ReturnsIssuer()
        {
            var issuer = new Izdavachi
            {
                Id = 1,
                Ime = "Alkaloid AD Skopje",
                Grad = "Skopje",
                Drzava = "Makedonija"
            };

            _context.Izdavachi.Add(issuer);
            _context.SaveChanges();

            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void GetById_WhenIssuerDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAll_WhenIssuersExist_ReturnsAllIssuers()
        {
            var issuers = new List<Izdavachi>
            {
                new Izdavachi
                { 
                    Id = 1,
                    Ime = "Issuer 1",
                    Grad = "Skopje",
                    Drzava = "Makedonija"
                
                },
                new Izdavachi
                { 
                    Id = 2,
                    Ime = "Issuer 2",
                    Grad = "Skopje",
                    Drzava = "Makedonija"
                },
                new Izdavachi
                { 
                    Id = 3,
                    Ime = "Issuer 3",
                    Grad = "Skopje",
                    Drzava = "Makedonija"
                }
            };

            _context.Izdavachi.AddRange(issuers);
            _context.SaveChanges();

            var result = _repository.GetAll().ToList();

            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3 },
                result.Select(x => x.Id).ToArray());
        }

        [TestMethod]
        public void GetAll_WhenNoIssuersExist_ReturnsEmptyCollection()
        {
            var result = _repository.GetAll().ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

    }
}
