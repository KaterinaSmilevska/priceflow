using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    [TestClass]
    public class BrokersRepositoryTests
    {
        private PriceFlowDbContext _context = null!;
        private BrokersRepository _repository = null!;
        private string _databaseName = null!;

        [TestInitialize]
        public void Setup()
        {
            _databaseName = Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<PriceFlowDbContext>()
                .UseInMemoryDatabase(_databaseName)
                .Options;

            _context = new TestPriceFlowDbContext(options);
            _repository = new BrokersRepository(_context);

            Data();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Dispose();
        }

        private void Data()
        {
            _context.Brokeri.AddRange(
                new Brokeri
                {
                    Id = 1,
                    Kompanija = "Broker 1"
                },
                new Brokeri
                {
                    Id = 2,
                    Kompanija = "Broker 2"
                },
                new Brokeri
                {
                    Id = 3,
                    Kompanija = "Broker 3"
                });

            _context.SaveChanges();
        }

        [TestMethod]
        public void GetById_WhenBrokerExists_ReturnsBroker()
        {
            var result = _repository.GetById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Broker 1", result.Kompanija);
        }

        [TestMethod]
        public void GetById_WhenBrokerDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetById(1000);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetByCompany_WhenCompanyExists_ReturnsBroker()
        {
            var result = _repository.GetByCompany("Broker 2");

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Id);
            Assert.AreEqual("Broker 2", result.Kompanija);
        }

        [TestMethod]
        public void GetByCompany_WhenCompanyDoesNotExist_ReturnsNull()
        {
            var result = _repository.GetByCompany("Unknown broker");

            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetAll_WhenBrokersExist_ReturnsAllBrokers()
        {
            var result = _repository.GetAll().ToList();

            Assert.AreEqual(3, result.Count);
            CollectionAssert.AreEquivalent(
                new[] { "Broker 1", "Broker 2", "Broker 3" },
                result.Select(b => b.Kompanija).ToArray());
        }

        [TestMethod]
        public void GetAll_WhenNoBrokersExist_ReturnsEmptyCollection()
        {
            _context.Brokeri.RemoveRange(_context.Brokeri);
            _context.SaveChanges();

            var result = _repository.GetAll().ToList();

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Add_WhenBrokerIsValid_SavesAndReturnsBroker()
        {
            var broker = new Brokeri
            {
                Id = 4,
                Kompanija = "New broker"
            };

            var result = _repository.Add(broker);

            Assert.AreSame(broker, result);
            Assert.AreEqual(4, _context.Brokeri.Count());
            Assert.IsNotNull(_context.Brokeri.Find(4));
            Assert.AreEqual("New broker", _context.Brokeri.Find(4)!.Kompanija);
        }

        [TestMethod]
        public void Update_WhenBrokerExists_UpdatesBroker()
        {
            var broker = _context.Brokeri.Find(1)!;
            broker.Kompanija = "Updated broker";

            var result = _repository.Update(broker);

            Assert.AreSame(broker, result);
            Assert.AreEqual("Updated broker", _context.Brokeri.Find(1)!.Kompanija);
        }

        [TestMethod]
        public void Delete_WhenBrokerExists_RemovesBrokerAndReturnsIt()
        {
            var broker = _context.Brokeri.Find(2)!;

            var result = _repository.Delete(broker);

            Assert.AreSame(broker, result);
            Assert.IsNull(_context.Brokeri.Find(2));
            Assert.AreEqual(2, _context.Brokeri.Count());
        }
    }
}
