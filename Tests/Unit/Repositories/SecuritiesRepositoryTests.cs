using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class SecuritiesRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private SecuritiesRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new SecuritiesRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            var sector1 = new Sektori
            {
                Id = 1,
                Ime = "Pharcmaceuticals"
            };

            var sector2 = new Sektori
            {
                Id = 2,
                Ime = "Banking"
            };

            var issuer1 = new Izdavachi
            {
                Id = 1,
                Ime = "Alkaloid AD Skopje",
                SektorId = 1,
                Sektor = sector1,
                Grad = "Skopje",
                Drzava = "Makedonija"
            };

            var issuer2 = new Izdavachi
            {
                Id = 2,
                Ime = "Komercijalna banka AD Skopje",
                SektorId = 2,
                Sektor = sector2,
                Grad = "Skopje",
                Drzava = "Makedonija"
            };

            var issuer3 = new Izdavachi
            {
                Id = 3,
                Ime = "Stopanska banka AD Skopje",
                SektorId = 2,
                Sektor = sector2,
                Grad = "Skopje",
                Drzava = "Makedonija"
            };

            var securityType = new TipHv
            {
                Id = 1,
                Ime = "Shares"
            };

            _context.Sektori.AddRange(sector1, sector2);
            _context.Izdavachi.AddRange(issuer1, issuer2, issuer3);
            _context.TipHv.Add(securityType);

            var security1 = new HartiiOdVrednost
            {
                Id = 1,
                Kod = "ALK",
                Isin = "MKALK101011",
                IzdavachId = 1,
                Izdavach = issuer1,
                TipHvid = 1,
                TipHv = securityType,
                VkupenBrojAkcii = 100000
            };

            var security2 = new HartiiOdVrednost
            {
                Id = 2,
                Kod = "KMB",
                Isin = "MKKMBS101019",
                IzdavachId = 2,
                Izdavach = issuer2,
                TipHvid = 1,
                TipHv = securityType,
                VkupenBrojAkcii = 200000
            };

            var security3 = new HartiiOdVrednost
            {
                Id = 3,
                Kod = "STB",
                Isin = "MKSTBS101014",
                IzdavachId = 3,
                Izdavach = issuer3,
                TipHvid = 1,
                TipHv = securityType,
                VkupenBrojAkcii = 300000
            };

            _context.HartiiOdVrednost.AddRange(security1, security2, security3);
            _context.SaveChanges();
        }

        [TestMethod]
        public void GetById_WhenSecurityExists_ReturnsSecurity()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("ALK", result.Kod);
            Assert.IsNotNull(result.Izdavach);
            Assert.AreEqual("Alkaloid AD Skopje", result.Izdavach.Ime);
            Assert.IsNotNull(result.TipHv);
            Assert.AreEqual("Shares", result.TipHv.Ime);
        }

        [TestMethod]
        public void GetById_WhenSecurityDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByCode_WhenCodeExists_ReturnsSecurity()
        {
            var result = _repository.GetByCode("KMB");

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Id);
            Assert.AreEqual("KMB", result.Kod);
        }

        [TestMethod]
        public void GetByCode_WhenCodeDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetByCode("Unknown");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAllByIds_WhenIdsExist_ReturnsMatchingSecurities()
        {
            var result = _repository.GetAllByIds(new List<int> { 1, 3 }).ToList();

            Assert.AreEqual(2, result.Count);

            CollectionAssert.AreEquivalent(
                new[] { "ALK", "STB" },
                result.Select(s => s.Kod).ToList());
        }

        [TestMethod]
        public void GetAllByIds_WhenNoIdsMatch_ReturnsEmptyCollection()
        {
            var result = _repository.GetAllByIds(new List<int> { 1000, 10001 });

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetAllByIds_WhenIdsListIsEmpty_ReturnsEmptyCollection()
        {
            var result = _repository.GetAllByIds(new List<int>());

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetAll_WhenSecuritiesExist_ReturnsAllSecurities()
        {
            var result = _repository.GetAll().ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(s => s.Izdavach != null));
            Assert.IsTrue(result.All(s => s.TipHv != null));

            CollectionAssert.AreEquivalent(
                new[] { "ALK", "KMB", "STB" },
                result.Select(s => s.Kod).ToList());
        }

        [TestMethod]
        public void GetAll_WhenNoSecuritiesExist_ReturnsEmptyCollection()
        {
            _context.HartiiOdVrednost.RemoveRange(_context.HartiiOdVrednost);
            _context.SaveChanges();

            var result = _repository.GetAll();

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void GetSecurityCode_WhenSecuritiesExist_ReturnsCode()
        {
            var result = _repository.GetSecurityCode(1);

            Assert.AreEqual("ALK", result);
        }

        [TestMethod]
        public void GetTotalNumSharesById_WhenSecurityExists_ReturnsSharesCount()
        {
            var result = _repository.GetTotalNumSharesById(1);

            Assert.AreEqual(100000, result);
        }

        [TestMethod]
        public void GetTotalNumSharesById_WhenSecurityDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetTotalNumSharesById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetTotalNumSharesBySecurityCode_WhenCodeExists_ReturnsSharesCount()
        {
            var result = _repository.GetTotalNumSharesBySecurityCode("KMB");

            Assert.AreEqual(200000, result);
        }

        [TestMethod]
        public void GetTotalNumSecurities_ReturnsTotalCount()
        {
            var result = _repository.GetTotalNumSecurities();

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public void GetTotalNumSecurities_WhenNoSecuritiesExist_ReturnsZero()
        {
            _context.HartiiOdVrednost.RemoveRange(_context.HartiiOdVrednost);
            _context.SaveChanges();

            var result = _repository.GetTotalNumSecurities();

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Add_WhenSecurityIsValid_SavesAndReturnsSecurity()
        {
            var security = new HartiiOdVrednost
            {
                Id = 4,
                Kod = "STBP",
                Isin = "MKSTBS120014",
                IzdavachId = 3,
                TipHvid = 1,
                VkupenBrojAkcii = 400000
            };

            var result = _repository.Add(security);

            Assert.IsNotNull(result);
            Assert.AreEqual("STBP", result.Kod);

            var saved = _context.HartiiOdVrednost.Find(4);

            Assert.IsNotNull(saved);
            Assert.AreEqual("STBP", saved.Kod);
            Assert.AreEqual(400000, saved.VkupenBrojAkcii);
        }

        [TestMethod]
        public void Update_WhenSecurityExists_UpdatesAndReturnsSecurity()
        {
            var security = _context.HartiiOdVrednost.Find(1);

            _context.Entry(security).State = EntityState.Detached;

            security.Kod = "ALK-Updated";
            security.VkupenBrojAkcii = 150000;

            var result = _repository.Update(security);

            Assert.AreEqual("ALK-Updated", result.Kod);
            Assert.AreEqual(150000, result.VkupenBrojAkcii);

            var saved = _context.HartiiOdVrednost.Find(1);

            Assert.IsNotNull(saved);
            Assert.AreEqual("ALK-Updated", saved.Kod);
            Assert.AreEqual(150000, saved.VkupenBrojAkcii);
        }

        [TestMethod]
        public void Delete_WhenSecurityExists_RemovesAndReturnsSecurity()
        {
            var security = _context.HartiiOdVrednost.Find(2)!;

            var result = _repository.Delete(security);

            Assert.IsNotNull(result);
            Assert.AreEqual("KMB", result.Kod);
            Assert.IsNull(_context.HartiiOdVrednost.Find(2));
        }

        [TestMethod]
        public void SearchByCode_WhenMatchesExist_ReturnsMatchingSecurities()
        {
            var result = _repository.SearchByCode("A").ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("ALK", result[0].Kod);
            Assert.IsNotNull(result[0].Izdavach);
            Assert.IsNotNull(result[0].TipHv);
        }

        [TestMethod]
        public void SearchByCode_WhenMultipleCodesMatch_ReturnsAllMatches()
        {
            var result = _repository.SearchByCode("B").ToList();

            CollectionAssert.AreEquivalent(
                new[] { "STB", "KMB" },
                result.Select(s => s.Kod).ToList());
        }

        [TestMethod]
        public void SearchByCode_WhenNoCodesMatch_ReturnsEmptyCollection()
        {
            var result = _repository.SearchByCode("Not found");

            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void SearchByCode_WhenSearchTermIsEmpty_ReturnsAllSecurities()
        {
            var result = _repository.SearchByCode("").ToList();

            Assert.AreEqual(3, result.Count);
        }
    }
}
