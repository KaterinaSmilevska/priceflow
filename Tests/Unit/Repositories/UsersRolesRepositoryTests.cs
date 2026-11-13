using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class UsersRolesRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private UsersRolesRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new UsersRolesRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var user1 = new Korisnici
            {
                Id = 1,
                Ime = "User 1",
                Email = "user1@test.com",
                Username = "user1",
                PasswordHash = new byte[] { 1, 2, 3 }
            };

            var user2 = new Korisnici
            {
                Id = 2,
                Ime = "User 2",
                Email = "user2@test.com",
                Username = "user2",
                PasswordHash = new byte[] { 4, 5, 6 }
            };

            var role1 = new Ulogi
            {
                Id = 1,
                Ime = "Admin"
            };

            var role2 = new Ulogi
            {
                Id = 2,
                Ime = "User"
            };

            _context.Korisnici.AddRange(user1, user2);
            _context.Ulogi.AddRange(role1, role2);
            _context.SaveChanges();

            _context.KorisniciUlogi.AddRange(
                new KorisniciUlogi
                {
                    KorisnikId = 1,
                    UlogaId = 1
                },
                new KorisniciUlogi
                {
                    KorisnikId = 1,
                    UlogaId = 2
                },
                new KorisniciUlogi
                {
                    KorisnikId = 2,
                    UlogaId = 2
                });

            _context.SaveChanges();
        }

        [TestMethod]
        public void Add_WhenUserRoleIsValid_SavesAndReturnsUserRole()
        {
            var userRole = new KorisniciUlogi
            {
                KorisnikId = 2,
                UlogaId = 1
            };

            var result = _repository.Add(userRole);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.KorisnikId);
            Assert.AreEqual(1, result.UlogaId);

            var saved = _context.KorisniciUlogi.SingleOrDefault(ku => ku.KorisnikId == 2 && ku.UlogaId == 1);

            Assert.IsNotNull(saved);
        }

        [TestMethod]
        public void RemoveByUserId_RemovesAllRolesForSpecifiedUser()
        {
            var result = _repository.RemoveByUserId(1);

            Assert.AreEqual(1, result);

            Assert.AreEqual(0, _context.KorisniciUlogi.Count(ku => ku.KorisnikId == 1));

            Assert.AreEqual(1, _context.KorisniciUlogi.Count(ku => ku.KorisnikId == 2));

            Assert.AreEqual(1, _context.KorisniciUlogi.Count());
        }

        [TestMethod]
        public void RemoveByUserId_WhenUserHasNoRoles_ReturnsUserId()
        {
            var initialRoleCount = _context.KorisniciUlogi.Count();
            var initialUserCount = _context.Korisnici.Count();

            var result = _repository.RemoveByUserId(1000);

            Assert.AreEqual(1000, result);
            Assert.AreEqual(initialRoleCount, _context.KorisniciUlogi.Count());
            Assert.AreEqual(initialUserCount, _context.Korisnici.Count());
        }

        [TestMethod]
        public void RemoveByUserId_WhenNoUserRoleExists_ReturnsUserId()
        {
            _context.KorisniciUlogi.RemoveRange(
                _context.KorisniciUlogi.ToList());
            _context.SaveChanges();

            var result = _repository.RemoveByUserId(1);

            Assert.AreEqual(1, result);
            Assert.AreEqual(0, _context.KorisniciUlogi.Count());
        }
    }
}
