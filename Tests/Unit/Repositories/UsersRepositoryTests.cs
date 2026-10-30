using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class UsersRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private UsersRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new UsersRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var users = new List<Korisnici>
            {
                new Korisnici
                {
                    Id = 1,
                    Ime = "User 1",
                    Email = "user1@test.com",
                    Username = "user1",
                    PasswordHash = new byte[] { 1, 2, 3 }
                },
                new Korisnici
                {
                    Id = 2,
                    Ime = "User 2",
                    Email = "user2@test.com",
                    Username = "user2",
                    PasswordHash = new byte[] { 4, 5, 6 }
                }
            };

            _context.Korisnici.AddRange(users);
            _context.SaveChanges();
        }

        [TestMethod]
        public void GetAll_ReturnsAllUsers()
        {
            var result = _repository.GetAll().ToList();

            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2 },
                result.Select(u => u.Id).ToArray());
        }

        [TestMethod]
        public void GetAll_WhenNoUsersExist_ReturnsEmptyCollection()
        {
            _context.Korisnici.RemoveRange(_context.Korisnici.ToList());
            _context.SaveChanges();

            var result = _repository.GetAll().ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Update_WhenUserExists_UpdatesAndReturnesUser()
        {
            var user = _context.Korisnici.Find(1);

            Assert.IsNotNull(user);

            user.Ime = "Updated user";
            user.Email = "updated@test.com";
            user.Username = "updateduser";

            var result = _repository.Update(user);

            Assert.AreSame(user, result);
            Assert.AreEqual("Updated user", result.Ime);
            Assert.AreEqual("updated@test.com", result.Email);
            Assert.AreEqual("updateduser", result.Username);

            var saved = _context.Korisnici.Find(1);

            Assert.IsNotNull(saved);
            Assert.AreEqual("Updated user", saved.Ime);
            Assert.AreEqual("updated@test.com", saved.Email);
            Assert.AreEqual("updateduser", saved.Username);
        }

    }
}
