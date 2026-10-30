using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class AuthRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private AuthRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new AuthRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var verificationToken = Guid.NewGuid();
            var resetToken = Guid.NewGuid();

            var user1 = new Korisnici
            {
                Id = 1,
                Ime = "User1",
                Username = "user1",
                PasswordHash = new byte[] { 1, 2, 3 },
                Email = "user1@test.com",
                IsEmailVerified = true,
                EmailVerificationToken = verificationToken,
                ResetPasswordToken = resetToken,
                ResetPasswordTokenExpiry = new DateTime(2026, 12, 1),
                DateModified = new DateTime(2026, 10, 1)
            };

            var user2 = new Korisnici
            {
                Id = 2,
                Ime = "User2",
                Username = "user2",
                PasswordHash = new byte[] { 4, 5, 6 },
                Email = "user2@test.com",
                IsEmailVerified = false,
                DateModified = new DateTime(2026, 10, 2)
            };

            var role = new Ulogi
            {
                Id = 1,
                Ime = "Admin",
                DateModified = new DateTime(2026, 10, 1)
            };

            var userRole = new KorisniciUlogi
            {
                Id = 1,
                KorisnikId = 1,
                UlogaId = 1,
                DateModified = new DateTime(2026, 10, 1)
            };

            _context.Korisnici.AddRange(user1, user2);
            _context.Ulogi.Add(role);
            _context.KorisniciUlogi.Add(userRole);

            _context.SaveChanges();
            _context.ChangeTracker.Clear();
        }

        [TestMethod]
        public void GetById_ExistingUser_ReturnsUser()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("user1", result.Username);
        }

        [TestMethod]
        public void GetById_NonExistingUser_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByUsername_ExistingUser_ReturnsUserWithRoles()
        {
            var result = _repository.GetByUsername("user1");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("user1", result.Username);
            Assert.IsNotNull(result.KorisniciUlogi);
            Assert.AreEqual(1, result.KorisniciUlogi.Count);

            var userRole = result.KorisniciUlogi.First();
            Assert.IsNotNull(userRole.Uloga);
            Assert.AreEqual("Admin", userRole.Uloga.Ime);
        }

        [TestMethod]
        public void GetByUsername_NonExistingUsername_ReturnsNull()
        {
            var result = _repository.GetByUsername("unknown");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByVerificationToken_ExistingToken_ReturnsUser()
        {
            var user = _context.Korisnici
                .AsNoTracking()
                .First(k => k.Id == 1);

            var result = _repository.GetByVerificationToken(user.EmailVerificationToken!.Value);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void GetByVerificationToken_NonExistingToken_ReturnsNull()
        {
            var result = _repository.GetByVerificationToken(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByEmail_ExistingEmail_ReturnsUser()
        {
            var result = _repository.GetByEmail("user1@test.com");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("user1@test.com", result.Email);
        }

        [TestMethod]
        public void GetByEmail_NonExistingEmail_ReturnsNull()
        {
            var result = _repository.GetByEmail("unknown@test.com");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByResetPasswordToken_ExistingToken_ReturnsUser()
        {
            var user = _context.Korisnici
                .AsNoTracking()
                .First(k => k.Id == 1);

            var result = _repository.GetByResetPasswordToken(user.ResetPasswordToken!.Value);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }

        [TestMethod]
        public void GetByResetPasswordToken_NonExistingToken_ReturnsNull()
        {
            var result = _repository.GetByResetPasswordToken(Guid.NewGuid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAll_ReturnsAllUsersWithRoles()
        {
            var result = _repository.GetAll().ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(k => k.Username == "user1"));
            Assert.IsTrue(result.Any(k => k.Username == "user2"));

            var user1 = result.First(k => k.Id == 1);
            Assert.AreEqual(1, user1.KorisniciUlogi.Count);
            Assert.AreEqual("Admin", user1.KorisniciUlogi.First().Uloga.Ime);
        }

        [TestMethod]
        public void Add_ValidUser_SavesAndReturnsUser()
        {
            var user = new Korisnici
            {
                Ime = "User3",
                Username = "user3",
                PasswordHash = new byte[] { 7, 8, 9 },
                Email = "user3@test.com",
                IsEmailVerified = false,
                DateModified = new DateTime(2026, 10, 3)
            };

            var result = _repository.Add(user);

            Assert.AreSame(user, result);
            Assert.IsTrue(user.Id > 0);
            Assert.IsTrue(_context.Korisnici.Any(k => k.Username == "user3"));
        }

        [TestMethod]
        public void Update_ExistingUser_PersistsChanges()
        {
            var user = _context.Korisnici.First(k => k.Id == 1);
            user.Ime = "Updated user";

            var result = _repository.Update(user);

            Assert.AreSame(user, result);

            var updated = _context.Korisnici
                .AsNoTracking()
                .First(k => k.Id == 1);

            Assert.AreEqual("Updated user", updated.Ime);
        }

        [TestMethod]
        public void Delete_ExistingUser_RemovesUser()
        {
            var user = _context.Korisnici.First(k => k.Id == 2);

            var result = _repository.Delete(user);

            Assert.AreSame(user, result);
            Assert.IsFalse(_context.Korisnici.Any(k => k.Id == 2));
        }

        [TestMethod]
        public void UsernameExists_ExistingUsername_ReturnsTrue()
        {
            var result = _repository.UsernameExists("user1");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UsernameExists_NonExistingUsername_ReturnsFalse()
        {
            var result = _repository.UsernameExists("unknown");

            Assert.IsFalse(result);
        }
    }
}
