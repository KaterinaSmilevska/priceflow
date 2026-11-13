using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class RolesRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private RolesRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new RolesRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var roles = new List<Ulogi>
            {
                new Ulogi { Id = 1, Ime = "Admin"},
                new Ulogi { Id = 2, Ime = "User"},
                new Ulogi { Id = 3, Ime = "Analyst"}
            };

            _context.Ulogi.AddRange(roles);

            _context.KorisniciUlogi.AddRange(
                new KorisniciUlogi
                {
                    KorisnikId = 10,
                    UlogaId = 1
                },
                new KorisniciUlogi
                {
                     KorisnikId = 10,
                     UlogaId = 2
                },
                new KorisniciUlogi
                {
                    KorisnikId = 20,
                    UlogaId = 3
                }
            );

            _context.SaveChanges();
        }

        [TestMethod]
        public void GetById_WhenRoleExists_ReturnsRole()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Admin", result.Ime);
        }

        [TestMethod]
        public void GetById_WhenRoleDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByUserId_WhenUserHasRoles_ReturnsRoleNames()
        {
            var result = _repository.GetByUserId(10);

            CollectionAssert.AreEquivalent(
                new List<string> { "Admin", "User" },
                result);
        }

        [TestMethod]
        public void GetByUserId_WhenUserHasNoRoles_ReturnsEmptyList()
        {
            var result = _repository.GetByUserId(1000);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetByName_WhenRoleExists_ReturnsRole()
        {
            var result = _repository.GetByName("Analyst");

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Id);
            Assert.AreEqual("Analyst", result.Ime);
        }

        [TestMethod]
        public void GetByName_WhenRoleDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetByName("Investor");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAll_WhenRolesExist_ReturnsAllRoles()
        {
            var result = _repository.GetAll().ToList();

            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEquivalent(
                new[] { "Admin", "User", "Analyst" },
                result.Select(r => r.Ime).ToList());
        }

        [TestMethod]
        public void GetAll_WhenNoRolesExist_ReturnsEmptyCollection()
        {
            _context.Ulogi.RemoveRange(_context.Ulogi);
            _context.SaveChanges();

            var result = _repository.GetAll().ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetNames_ReturnsAllRoleNames()
        {
            var result = _repository.GetNames();

            CollectionAssert.AreEquivalent(
                new List<string> { "Admin", "User", "Analyst" }, result);
        }

        [TestMethod]
        public void GetNames_WhenNoRolesExist_ReturnsEmptyList()
        {
            _context.Ulogi.RemoveRange(_context.Ulogi);
            _context.SaveChanges();

            var result = _repository.GetNames();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetIdsByNames_ReturnsIdsForMatchingNames()
        {
            var result = _repository.GetIdsByNames(
                new List<string> { "Admin", "Analyst" });

            CollectionAssert.AreEquivalent(
                new List<int> { 1, 3 }, result);
        }

        [TestMethod]
        public void GetIdsByNames_WhenNoNamesMatch_ReturnsEmptyList()
        {
            var result = _repository.GetIdsByNames(
               new List<string> { "Unknown", "Guest" });

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetIdsByNames_WhenNamesListIsEmpty_ReturnsEmptyList()
        {
            var result = _repository.GetIdsByNames(new List<string>());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetNames_WhenRolesAreProvided_ReturnsMatchingNames()
        {
            var roles = new List<Ulogi>
            {
                new Ulogi { Id = 1, Ime = "Different name" },
                new Ulogi { Id = 3, Ime = "Another name" }
            };

            var result = _repository.GetNames(roles);

            CollectionAssert.AreEquivalent(
                new List<string> { "Admin", "Analyst" }, result);
        }

        [TestMethod]
        public void GetNames_WhenNoRoleIdsMatch_ReturnsEmptyList()
        {
            var roles = new List<Ulogi>
            {
                new Ulogi { Id = 1000, Ime = "Unknown" }
            };

            var result = _repository.GetNames(roles);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetNames_WhenRolesCollectionIsEmpty_ReturnsEmptyList()
        {
            var result = _repository.GetNames(new List<Ulogi>());

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);
        }
    }
}
