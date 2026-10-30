using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class PortfoliosRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private PortfoliosRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new PortfoliosRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var portfolio1 = new Portfolija
            {
                Id = 1,
                Ime = "Portfolio 1",
                KorisnikId = 10
            };

            var portfolio2 = new Portfolija
            {
                Id = 2,
                Ime = "Portfolio 2",
                KorisnikId = 10
            };

            var portfolio3 = new Portfolija
            {
                Id = 3,
                Ime = "Portfolio 3",
                KorisnikId = 20
            };

            _context.Portfolija.AddRange(portfolio1, portfolio2, portfolio3);
            _context.SaveChanges();
        }

        [TestMethod]
        public void GetById_WhenPortfolioExists_ReturnsPortfolio()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Portfolio 1", result.Ime);
            Assert.AreEqual(10, result.KorisnikId);
        }

        [TestMethod]
        public void GetById_WhenPortfolioDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByName_WhenPortfolioExistsForUser_ReturnsPortfolio()
        {
            var result = _repository.GetByName("Portfolio 1", 10);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Portfolio 1", result.Ime);
            Assert.AreEqual(10, result.KorisnikId);
        }

        [TestMethod]
        public void GetByName_WhenPortfolioBelongsToAnotherUser_ReturnsNull()
        {
            var result = _repository.GetByName("Portfolio 1", 20);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByName_WhenNameDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetByName("Missing portfolio", 10);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByUserId_ReturnsOnlyUsersPortfolios()
        {
            var result = _repository.GetByUserId(10).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(p => p.KorisnikId == 10));
        }

        [TestMethod]
        public void GetByUserId_WhenUserHasNoPortfolios_ReturnsEmptyCollection()
        {
            var result = _repository.GetByUserId(1000).ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Add_SavesAndReturnsPortfolio()
        {
            var portfolio = new Portfolija
            {
                Id = 4,
                Ime = "New Portfolio",
                KorisnikId = 10
            };

            var result = _repository.Add(portfolio);

            Assert.AreSame(portfolio, result);

            var saved = _context.Portfolija.SingleOrDefault(p => p.Id == 4);

            Assert.IsNotNull(saved);
            Assert.AreEqual("New Portfolio", saved.Ime);
            Assert.AreEqual(10, saved.KorisnikId);
        }

        [TestMethod]
        public void Update_ModifiesAndReturnsPortfolio()
        {
            var portfolio = new Portfolija
            {
                Id = 4,
                Ime = "Original Portfolio",
                KorisnikId = 10
            };

            _context.Portfolija.Add(portfolio);
            _context.SaveChanges();
            _context.Entry(portfolio).State = EntityState.Detached;

            var updated = new Portfolija
            {
                Id = 4,
                Ime = "Updated Portfolio",
                KorisnikId = 10
            };

            var result = _repository.Update(updated);

            Assert.AreSame(updated, result);

            var saved = _context.Portfolija
                .Single(p => p.Id == 4);

            Assert.AreEqual("Updated Portfolio", saved.Ime);
            Assert.AreEqual(10, saved.KorisnikId);
        }

        [TestMethod]
        public void Delete_RemovesPortfolioAndReturnsIt()
        {
            var portfolio = new Portfolija
            {
                Id = 4,
                Ime = "Portfolio to Delete",
                KorisnikId = 10
            };

            _context.Portfolija.Add(portfolio);
            _context.SaveChanges();

            var result = _repository.Delete(portfolio);

            Assert.AreSame(portfolio, result);
            Assert.IsNull(_context.Portfolija.Find(4));
        }

    }
}
