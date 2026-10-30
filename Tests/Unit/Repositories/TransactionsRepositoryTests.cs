using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class TransactionsRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private TransactionsRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new TransactionsRepository(_context);

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
                Id = 2,
                Ime = "Banking",
                DateModified = DateTime.Today
            };

            var issuer1 = new Izdavachi
            {
                Id = 1,
                Ime = "Komercijalna banka AD Skopje",
                Grad = "Skopje",
                Drzava = "Makedonija",
                SektorId = 1,
                Sektor = sector,
                DateModified = DateTime.Today
            };

            var issuer2 = new Izdavachi
            {
                Id = 2,
                Ime = "Stopanska banka AD Skopje",
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
                Kod = "KMB",
                Isin = "MKKMBS101019",
                VkupenBrojAkcii = 100000,
                TipHvid = 1,
                IzdavachId = 1,
                Izdavach = issuer1,
                TipHv = securityType,
                DateModified = DateTime.Today
            };

            var security2 = new HartiiOdVrednost
            {
                Id = 2,
                Kod = "STB",
                Isin = "MKSTBS101014",
                VkupenBrojAkcii = 200000,
                TipHvid = 1,
                IzdavachId = 2,
                Izdavach = issuer2,
                TipHv = securityType,
                DateModified = DateTime.Today
            };

            var portfolio1 = new Portfolija
            {
                Id = 1,
                Ime = "User 10 Portfolio",
                KorisnikId = 10,
                Korisnik = user10,
                Opis = "Real and virtual transactions",
                DateModified = DateTime.Today
            };

            var portfolio2 = new Portfolija
            {
                Id = 2,
                Ime = "User 20 Portfolio",
                KorisnikId = 20,
                Korisnik = user20,
                Opis = "Second user's portfolio",
                DateModified = DateTime.Today
            };

            _context.Korisnici.AddRange(user10, user20);
            _context.Sektori.Add(sector);
            _context.Izdavachi.AddRange(issuer1, issuer2);
            _context.TipHv.Add(securityType);
            _context.HartiiOdVrednost.AddRange(security1, security2);
            _context.Portfolija.AddRange(portfolio1, portfolio2);

            var transactions = new List<Transakcii>
            {
                new Transakcii
                {
                    Id = 1,
                    PortfolioId = 1,
                    Portfolio = portfolio1,
                    Hvid = 1,
                    Hv = security1,
                    Datum = new DateOnly(2026, 10, 10),
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 100,
                    Realna = true,
                    Iznos = 10000m
                },
                new Transakcii
                {
                    Id = 2,
                    PortfolioId = 1,
                    Portfolio = portfolio1,
                    Hvid = 1,
                    Hv = security1,
                    Datum = new DateOnly(2026, 10, 12),
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 30,
                    Realna = true,
                    Iznos = 3300m
                },
                new Transakcii
                {
                    Id = 3,
                    PortfolioId = 1,
                    Portfolio = portfolio1,
                    Hvid = 1,
                    Hv = security1,
                    Datum = new DateOnly(2026, 10, 15),
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 50,
                    Realna = false,
                    Iznos = 5000m
                },
                new Transakcii
                {
                    Id = 4,
                    PortfolioId = 1,
                    Portfolio = portfolio1,
                    Hvid = 2,
                    Hv = security2,
                    Datum = new DateOnly(2026, 10, 11),
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 40,
                    Realna = true,
                    Iznos = 4000m
                },
                new Transakcii
                {
                    Id = 5,
                    PortfolioId = 1,
                    Portfolio = portfolio1,
                    Hvid = 2,
                    Hv = security2,
                    Datum = new DateOnly(2026, 10, 18),
                    TipTransakcija = "Продавање",
                    KolicinaAkcii = 10,
                    Realna = true,
                    Iznos = 1100m
                },
                new Transakcii
                {
                    Id = 6,
                    PortfolioId = 2,
                    Portfolio = portfolio2,
                    Hvid = 1,
                    Hv = security1,
                    Datum = new DateOnly(2026, 10, 13),
                    TipTransakcija = "Купување",
                    KolicinaAkcii = 25,
                    Realna = true,
                    Iznos = 2500m
                }
            };

            _context.Transakcii.AddRange(transactions);
            _context.SaveChanges();
        }

        [TestMethod]
        public void GetById_WhenTransactionExists_ReturnsTransaction()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual(1, result.PortfolioId);
            Assert.AreEqual(1, result.Hvid);
            Assert.IsNotNull(result.Hv);
            Assert.AreEqual("KMB", result.Hv.Kod);
        }

        [TestMethod]
        public void GetById_WhenTransactionDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByPortfolioId_ReturnsTransactionsOrderedByDateDescending()
        {
            var result = _repository.GetByPortfolioId(1).ToList();

            Assert.AreEqual(5, result.Count);
            Assert.IsTrue(result.All(t => t.PortfolioId == 1));
            Assert.IsTrue(result.All(t => t.Hv != null));

            CollectionAssert.AreEqual(
                new[] { 5, 3, 2, 4, 1 },
                result.Select(t => t.Id).ToArray());
        }

        [TestMethod]
        public void GetByPortfolioId_WhenPortfolioHasNoTransactions_ReturnsEmptyCollection()
        {
            var result = _repository.GetByPortfolioId(1000).ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetByPortfolioId_WithRealFlag_ReturnsOnlyMatchingTransactions()
        {
            var real = _repository.GetByPortfolioId(1, true).ToList();
            var virtualTransactions = _repository.GetByPortfolioId(1, false).ToList();

            Assert.AreEqual(4, real.Count);
            Assert.IsTrue(real.All(t => t.Realna));

            Assert.AreEqual(1, virtualTransactions.Count);
            Assert.IsTrue(virtualTransactions.All(t => !t.Realna));
            Assert.AreEqual(3, virtualTransactions[0].Id);
        }

        [TestMethod]
        public void GetByPortfolioIdUntilDate_ReturnsRealTransactionsUpToAndIncludingDate()
        {
            var result = _repository.GetByPortfolioIdUntilDate(1, new DateOnly(2026, 10, 12)).ToList();

            Assert.AreEqual(3, result.Count);
            Assert.IsTrue(result.All(t => t.PortfolioId == 1));
            Assert.IsTrue(result.All(t => t.Realna));
            Assert.IsTrue(result.All(t => t.Datum <= new DateOnly(2026, 10, 12)));

            CollectionAssert.AreEqual(
                new[] { 1, 4, 2 },
                result.Select(t => t.Id).ToArray());
        }

        [TestMethod]
        public void GetByPortfolioIdUntilDate_ExcludesVirtualTransactions()
        {
            var result = _repository.GetByPortfolioIdUntilDate(1, new DateOnly(2026, 10, 20)).ToList();

            Assert.IsFalse(result.Any(t => t.Id == 3));
        }

        [TestMethod]
        public void GetOwnedSecuritiesIds_ReturnsOnlySecuritiesWithPositiveRealBalance()
        {
            var result = _repository.GetOwnedSecuritiesIds(10);

            CollectionAssert.AreEquivalent(new[] { 1, 2 }, result);
        }

        [TestMethod]
        public void GetOwnedSecuritiesIds_DoesNotIncludeAnotherUsersSecurities()
        {
            var result = _repository.GetOwnedSecuritiesIds(20);

            CollectionAssert.AreEqual(new[] { 1 }, result);
        }

        [TestMethod]
        public void GetOwnedSecuritiesIds_WhenUserHasNoPortfolioTransactions_ReturnsEmptyList()
        {
            var result = _repository.GetOwnedSecuritiesIds(1000);

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void GetOwnedShares_ReturnsBoughtMinusSoldForSelectedTransactionType()
        {
            var real = _repository.GetOwnedShares(1, 1, true);
            var virtualShares = _repository.GetOwnedShares(1, 1, false);

            Assert.AreEqual(70, real);
            Assert.AreEqual(50, virtualShares);
        }

        [TestMethod]
        public void GetOwnedShares_WhenNoTransactionsExist_ReturnsZero()
        {
            var result = _repository.GetOwnedShares(1, 1000, true);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetOwnedSharesAtDate_ReturnsBalanceUpToAndIncludingDate()
        {
            var result = _repository.GetOwnedSharesAtDate(1, 1, true, new DateOnly(2026, 10, 12), null);

            Assert.AreEqual(70, result);
        }

        [TestMethod]
        public void GetOwnedSharesAtDate_ExcludesSpecifiedTransaction()
        {
            var result = _repository.GetOwnedSharesAtDate(1, 1, true, new DateOnly(2026, 10, 20), 2);

            Assert.AreEqual(100, result);
        }

        [TestMethod]
        public void GetOwnedSharesAtDate_ExludesTransactionsAfterDate()
        {
            var result = _repository.GetOwnedSharesAtDate(1, 1, true, new DateOnly(2026, 10, 11), null);

            Assert.AreEqual(100, result);
        }

        [TestMethod]
        public void GetOwnedSharesByUser_ReturnsRealSharesAcrossUsersPortfolios()
        {
            var user10 = _repository.GetOwnedSharesByUser(10, 1);
            var user20 = _repository.GetOwnedSharesByUser(20, 1);

            Assert.AreEqual(70, user10);
            Assert.AreEqual(25, user20);
        }

        [TestMethod]
        public void GetOwnedSharesByUser_WhenNoMatchingTransactions_ReturnsZero()
        {
            var result = _repository.GetOwnedSharesByUser(1000, 1);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void Add_WhenTransactionIsValid_SavesAndReturnsTransaction()
        {
            var transaction = new Transakcii
            {
                Id = 7,
                PortfolioId = 1,
                Hvid = 2,
                Datum = new DateOnly(2026, 10, 20),
                TipTransakcija = "Купување",
                KolicinaAkcii = 15,
                Realna = true,
                Iznos = 1500m
            };

            var result = _repository.Add(transaction);

            Assert.IsNotNull(result);
            Assert.AreEqual(7, result.Id);

            var saved = _context.Transakcii.Find(7);

            Assert.IsNotNull(saved);
            Assert.AreEqual(15, saved.KolicinaAkcii);
            Assert.AreEqual(1, saved.PortfolioId);
            Assert.AreEqual(2, saved.Hvid);
        }

        [TestMethod]
        public void Update_WhenTransactionExists_UpdatesAndReturnsTransaction()
        {
            var transaction = _context.Transakcii.Find(1);
            _context.Entry(transaction).State = EntityState.Detached;

            transaction.KolicinaAkcii = 120;
            transaction.Iznos = 12000m;

            var result = _repository.Update(transaction);

            Assert.AreEqual(120, result.KolicinaAkcii);
            Assert.AreEqual(12000m, result.Iznos);

            var saved = _context.Transakcii.Find(1);

            Assert.IsNotNull(saved);
            Assert.AreEqual(120, saved.KolicinaAkcii);
            Assert.AreEqual(12000m, saved.Iznos);
        }

        [TestMethod]
        public void Delete_WhenTransactionExists_RemovesAndReturnsTransaction()
        {
            var transaction = _context.Transakcii.Find(2);

            var result = _repository.Delete(transaction);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Id);
            Assert.IsNull(_context.Transakcii.Find(2));
        }
    }
}
