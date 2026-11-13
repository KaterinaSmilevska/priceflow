using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class ThresholdRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private ThresholdRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new ThresholdRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var user10 = new Korisnici
            {
                Id = 10,
                Ime = "User 10",
                Email = "user10@test.com",
                Username = "user10",
                PasswordHash = new byte[] { 1, 2, 3 }
            };

            var user20 = new Korisnici
            {
                Id = 20,
                Ime = "User 20",
                Email = "user20@test.com",
                Username = "user20",
                PasswordHash = new byte[] { 4, 5, 6 }
            };

            var sector = new Sektori
            {
                Id = 1,
                Ime = "Pharmaceuticals",
                DateModified = DateTime.Today
            };

            var issuer = new Izdavachi
            {
                Id = 1,
                Ime = "Alkaloid AD Skopje",
                Grad = "Skopje",
                Drzava = "Makedonija",
                SektorId = 1,
                Sektor = sector,
                DateModified = DateTime.Today
            };

            var securityType = new TipHv
            {
                Id = 1,
                Ime = "Shares",
                DateModified = DateTime.Today
            };

            var security1 = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                Isin = "MKALK101011",
                VkupenBrojAkcii = 100000,
                TipHvid = 1,
                IzdavachId = 1,
                Izdavach = issuer,
                TipHv = securityType,
                DateModified = DateTime.Today
            };

            var security2 = new HartiiOdVrednost
            {
                Id = 2,
                Kod = "KMB",
                Isin = "MKKMBS101019",
                VkupenBrojAkcii = 200000,
                TipHvid = 1,
                IzdavachId = 1,
                Izdavach = issuer,
                TipHv = securityType,
                DateModified = DateTime.Today
            };

            var security3 = new HartiiOdVrednost
            {
                Id = 3,
                Kod = "STB",
                Isin = "MKSTBS101014",
                VkupenBrojAkcii = 300000,
                TipHvid = 1,
                IzdavachId = 1,
                Izdavach = issuer,
                TipHv = securityType,
                DateModified = DateTime.Today
            };

            _context.Korisnici.AddRange(user10, user20);
            _context.Sektori.Add(sector);
            _context.Izdavachi.Add(issuer);
            _context.TipHv.Add(securityType);
            _context.HartiiOdVrednost.AddRange(security1, security2);

            var threshold1 = new HvPromenaCena
            {
                Id = 1,
                KorisnikId = 10,
                Hvid = 1,
                DolnaGranica = 100m,
                GornaGranica = 200m,
                Hv = security1,
                Korisnik = user10
            };

            var threshold2 = new HvPromenaCena
            {
                Id = 2,
                KorisnikId = 10,
                Hvid = 2,
                DolnaGranica = 50m,
                GornaGranica = 150m,
                Hv = security2,
                Korisnik = user10
            };

            var threshol3 = new HvPromenaCena
            {
                Id = 3,
                KorisnikId = 20,
                Hvid = 3,
                DolnaGranica = 120m,
                GornaGranica = 250m,
                Hv = security3,
                Korisnik = user20
            };

            _context.HvPromenaCena.AddRange(threshold1, threshold2, threshol3);
            _context.SaveChanges();
        }

        [TestMethod]
        public void GetById_WhenThresholdExists_ReturnsThresholdWithSecurity()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(10, result.KorisnikId);
            Assert.AreEqual(1, result.Hvid);
            Assert.IsNotNull(result.Hv);
            Assert.AreEqual("ALK", result.Hv.Kod);
        }

        [TestMethod]
        public void GetById_WhenThresholdDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByUserId_WhenThresholdExist_ReturnsOnlyUsersThresholds()
        {
            var result = _repository.GetByUserId(10).ToList();

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(t => t.KorisnikId == 10));
            Assert.IsTrue(result.All(t => t.Hv != null));

            CollectionAssert.AreEquivalent(
                new[] { 1, 2 },
                result.Select(t => t.Id).ToArray());
        }

        [TestMethod]
        public void GetByUserId_WhenUserHasNoThresholds_ReturnsEmptyCollection()
        {
            var result = _repository.GetByUserId(1000);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetBySecurityId_WhenThresholdsExist_ReturnsMatchingThresholdsWithSecurity()
        {
            var result = _repository.GetBySecurityId(1).ToList();

            Assert.AreEqual(1, result.Count);
            Assert.IsTrue(result.All(t => t.Hvid == 1));
            Assert.IsTrue(result.All(t => t.Hv != null));
            Assert.IsTrue(result.All(t => t.Hv.Kod == "ALK"));
        }

        [TestMethod]
        public void GetBySecurityId_WhenNoThresholdsExist_ReturnsEmptyCollection()
        {
            var result = _repository.GetBySecurityId(1000);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetByUserIdAndSecurityId_WhenThresholdExists_ReturnsMatchingThreshold()
        {
            var result = _repository.GetByUserIdAndSecurityId(10, 2);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Id);
            Assert.AreEqual(10, result.KorisnikId);
            Assert.AreEqual(2, result.Hvid);
        }

        [TestMethod]
        public void GetByUserIdAndSecurityId_WhenThresholdDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetByUserIdAndSecurityId(1000, 1);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByUserIdAndSecurityId_WhenSecurityDoesNotMatch_ReturnsNull()
        {
            var result = _repository.GetByUserIdAndSecurityId(10, 1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Add_WhenThresholdIsValid_SavesAndReturnsThreshold()
        {
            var threshold = new HvPromenaCena
            {
                Id = 4,
                KorisnikId = 20,
                Hvid = 2,
                DolnaGranica = 80m,
                GornaGranica = 180m
            };

            var result = _repository.Add(threshold);

            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Id);
            Assert.AreEqual(20, result.KorisnikId);
            Assert.AreEqual(2, result.Hvid);

            var saved = _context.HvPromenaCena.Find(4);

            Assert.IsNotNull(saved);
            Assert.AreEqual(80m, saved.DolnaGranica);
            Assert.AreEqual(180m, saved.GornaGranica);
        }

        [TestMethod]
        public void Update_WhenThresholdExist_UpdatesAndReturnsThreshold()
        {
            var threshold = _context.HvPromenaCena.Find(1);
            _context.Entry(threshold).State = EntityState.Detached;

            threshold.DolnaGranica = 110m;
            threshold.GornaGranica = 220m;

            var result = _repository.Update(threshold);

            Assert.AreEqual(110m, result.DolnaGranica);
            Assert.AreEqual(220m, result.GornaGranica);

            var saved = _context.HvPromenaCena.Find(1);

            Assert.IsNotNull(saved);
            Assert.AreEqual(110m, saved.DolnaGranica);
            Assert.AreEqual(220m, saved.GornaGranica);
        }

        [TestMethod]
        public void Delete_WhenThresholdExists_RemovesAndReturnsThreshold()
        {
            var threshold = _context.HvPromenaCena.Find(2)!;

            var result = _repository.Delete(threshold);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Id);
            Assert.IsNull(_context.HvPromenaCena.Find(2));
        }
    }
}
