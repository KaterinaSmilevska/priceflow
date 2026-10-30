using DataAccess.Models;
using DataAccess.Repositories;
using Moq;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class RolesServiceTests
    {
        private Mock<IRolesRepository> _rolesRepositoryMock = null!;
        private Mock<IAuthRepository> _authRepositoryMock = null!;
        private RolesService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _rolesRepositoryMock = new Mock<IRolesRepository>();
            _authRepositoryMock = new Mock<IAuthRepository>();
            _service = new RolesService(_rolesRepositoryMock.Object, _authRepositoryMock.Object);
        }

        [TestMethod]
        public void FindByName_ValidRole_ReturnsMappedRole()
        {
           var role = new Ulogi
           {
                Id = 1,
                Ime = "Admin"
           };

            _rolesRepositoryMock
                 .Setup(r => r.GetByName("Admin"))
                 .Returns(role);

            var result = _service.FindByName("Admin");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Admin", result.Name);

            _rolesRepositoryMock.Verify(
                r => r.GetByName("Admin"),
                Times.Once);
        }

        [TestMethod]
        public void FindByName_EmptyName_ThrowsValidationException()
        {
            var exception = Assert.ThrowsException<ValidationException>(
                () => _service.FindByName(""));

            Assert.AreEqual("NAME_VALIDATION_REQUIRED", exception.Code);

            _rolesRepositoryMock.Verify(
                r => r.GetByName(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void FindByName_RoleNotFound_ThrowsNotFoundException()
        {
            _rolesRepositoryMock
                .Setup(r => r.GetByName("Admin"))
                .Returns((Ulogi?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
               () => _service.FindByName("Admin"));

            Assert.AreEqual("ROLE_NOT_FOUND", exception.Code);

            _rolesRepositoryMock.Verify(
                r => r.GetByName("Admin"),
                Times.Once);
        }

        [TestMethod]
        public void FindByUserId_ValidUser_ReturnsRoles()
        {
            var user = new Korisnici
            {
                Id = 1
            };

            var roles = new List<string>
            {
                "Admin",
                "User"
            };

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            _rolesRepositoryMock
                .Setup(r => r.GetByUserId(1))
                .Returns(roles);

            var result = _service.FindByUserId(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            CollectionAssert.AreEqual(roles, result);

            _rolesRepositoryMock.Verify(
                r => r.GetByUserId(1),
                Times.Once);
        }

        [TestMethod]
        public void FindByUserId_UserNotFound_ThrowsNotFoundException()
        {
            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns((Korisnici?)null);

            var exception = Assert.ThrowsException<NotFoundException>(
                () => _service.FindByUserId(1));

            Assert.AreEqual("USER_NOT_FOUND", exception.Code);

            _rolesRepositoryMock.Verify(
                r => r.GetByUserId(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public void FindNames_ReturnsRoleNames()
        {
            var roles = new List<string>
            {
                "Admin",
                "User",
                "Investor"
            };

            _rolesRepositoryMock
                .Setup(r => r.GetNames())
                .Returns(roles);

            var result = _service.FindNames();

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(roles, result);

            _rolesRepositoryMock.Verify(
                r => r.GetNames(),
                Times.Once);
        }

        [TestMethod]
        public void FindRoles_NoRoles_ReturnsEmptyList()
        {
            _rolesRepositoryMock
                .Setup(r => r.GetNames())
                .Returns(new List<string>());

            var result = _service.FindNames();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);

            _rolesRepositoryMock.Verify(
                r => r.GetNames(),
                Times.Once);
        }
    }
}
