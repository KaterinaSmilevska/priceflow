using DataAccess.Models;
using DataAccess.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Moq;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;
using PriceFlowSecurity;
using System.Security.Claims;

namespace Tests.Unit.Services
{
    [TestClass]
    public class AuthServiceTests
    {
        private Mock<IAuthRepository> _authRepositoryMock = null!;
        private Mock<IRolesRepository> _rolesRepositoryMock = null!;
        private Mock<IUsersRolesRepository> _usersRolesRepositoryMock = null!;
        private Mock<IEmailService> _emailServiceMock = null!;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock = null!;
        private AuthService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _rolesRepositoryMock = new Mock<IRolesRepository>();
            _usersRolesRepositoryMock = new Mock<IUsersRolesRepository>();
            _emailServiceMock = new Mock<IEmailService>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            _service = new AuthService(
                _authRepositoryMock.Object,
                _rolesRepositoryMock.Object,
                _usersRolesRepositoryMock.Object,
                _emailServiceMock.Object,
                _httpContextAccessorMock.Object);
        }

        [TestMethod]
        public void ValidateEmail_EmptyEmail_ReturnsInvalid()
        {
            var request = new EmailValidationRequest
            {
                Email = ""
            };

            var result = _service.ValidateEmail(request);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("EMAIL_VALIDATION_REQUIRED", result.Code);
        }

        [TestMethod]
        public void ValidateEmail_InvalidFormat_ReturnsInvalid()
        {
            var request = new EmailValidationRequest
            {
                Email = "invalid-email"
            };

            var result = _service.ValidateEmail(request);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("EMAIL_FORMAT", result.Code);
        }

        [TestMethod]
        public void ValidateEmail_ValidEmail_ReturnsValid()
        {
            var request = new EmailValidationRequest
            {
                Email = "test@example.com"
            };

            var result = _service.ValidateEmail(request);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual("EMAIL_VALIDATION_SUCCESS", result.Code);
        }

        [TestMethod]
        public void ValidatePassword_EmptyPassword_ReturnsInvalid()
        {
            var request = new PasswordValidationRequest
            {
                Password = "",
                ConfirmPassword = ""
            };

            var result = _service.ValidatePassword(request);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("PASSWORD_VALIDATION_REQUIRED", result.Code);
        }

        [TestMethod]
        public void ValidatePassword_DifferentPasswords_ReturnsInvalid()
        {
            var request = new PasswordValidationRequest
            {
                Password = "Password123!",
                ConfirmPassword = "Password321!"
            };

            var result = _service.ValidatePassword(request);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("PASSWORD_MISMATCH", result.Code);
        }

        [TestMethod]
        public void ValidatePassword_WeakPassword_ReturnsInvalid()
        {
            var request = new PasswordValidationRequest
            {
                Password = "password",
                ConfirmPassword = "password"
            };

            var result = _service.ValidatePassword(request);

            Assert.IsFalse(result.IsValid);
            Assert.AreEqual("PASSWORD_STRENGTH", result.Code);
        }

        [TestMethod]
        public void ValidatePassword_ValidPassword_ReturnsValid()
        {
            var request = new PasswordValidationRequest
            {
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var result = _service.ValidatePassword(request);

            Assert.IsTrue(result.IsValid);
            Assert.AreEqual("PASSWORD_VALIDATION_SUCCESS", result.Code);
        }

        [TestMethod]
        public void UsernameExists_ExistingUsername_ReturnsTrue()
        {
            string username = "test";

            _authRepositoryMock
                .Setup(r => r.UsernameExists(username))
                .Returns(true);

            var result = _service.UsernameExists(username);

            Assert.IsTrue(result);

            _authRepositoryMock.Verify(
                r => r.UsernameExists(username),
                Times.Once);
        }

        [TestMethod]
        public void UsernameExists_NonExistingUsername_ReturnsFalse()
        {
            string username = "unknown";

            _authRepositoryMock
                .Setup(r => r.UsernameExists(username))
                .Returns(false);

            var result = _service.UsernameExists(username);

            Assert.IsFalse(result);

            _authRepositoryMock.Verify(
                r => r.UsernameExists(username),
                Times.Once);
        }

        [TestMethod]
        public void FindById_ExistingUser_ReturnsUser()
        {
            var user = new Korisnici
            {
                Id = 1,
                Ime = "Test",
                Username = "test",
                Email = "test@example.com",
                IsEmailVerified = true,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            _authRepositoryMock
                .Setup(r => r.GetById(1))
                .Returns(user);

            var result = _service.FindById(1);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Test", result.Name);
            Assert.AreEqual("test", result.Username);
            Assert.AreEqual("test@example.com", result.Email);
            Assert.IsTrue(result.IsEmailVerified);

            _authRepositoryMock.Verify(
                r => r.GetById(1),
            Times.Once);
        }

        [TestMethod]
        public void FindById_UserNotFound_ThrowsNotFoundException()
        {
            _authRepositoryMock
                .Setup(r => r.GetById(1000))
                .Returns((Korisnici?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.FindById(1000));

            _authRepositoryMock.Verify(
                r => r.GetById(1000),
                Times.Once);
        }

        [TestMethod]
        public void FindByUsername_ExistingUser_ReturnsUser()
        {
            var user = new Korisnici
            {
                Id = 1,
                Ime = "Test",
                Username = "test",
                Email = "test@example.com",
                IsEmailVerified = true,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername("test"))
                .Returns(user);

            var result = _service.FindByUsername("test");

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("Test", result.Name);
            Assert.AreEqual("test", result.Username);
            Assert.AreEqual("test@example.com", result.Email);

            _authRepositoryMock.Verify(
                r => r.GetByUsername("test"),
                Times.Once);
        }

        [TestMethod]
        public void FindByUsername_EmptyUsername_ThrowsValidationException()
        {
            Assert.ThrowsException<ValidationException>(
                () => _service.FindByUsername(""));

            _authRepositoryMock.Verify(
                r => r.GetByUsername(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void FindByUsername_UserNotFound_ThrowsNotFoundException()
        {
            _authRepositoryMock
                .Setup(r => r.GetByUsername("unknown"))
                .Returns((Korisnici?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.FindByUsername("unknown"));

            _authRepositoryMock.Verify(
             r => r.GetByUsername("unknown"),
             Times.Once);
        }

        [TestMethod]
        public void FindByVerificationToken_ExistingUser_ReturnsUser()
        {
            Guid token = Guid.NewGuid();

            var user = new Korisnici
            {
                Id = 1,
                Ime = "Test",
                Username = "test",
                Email = "test@example.com",
                IsEmailVerified = false,
                EmailVerificationToken = token,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            _authRepositoryMock
                .Setup(r => r.GetByVerificationToken(token))
                .Returns(user);

            var result = _service.FindByVerificationToken(token);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
            Assert.AreEqual("test", result.Username);
            Assert.IsFalse(result.IsEmailVerified);

            _authRepositoryMock.Verify(
             r => r.GetByVerificationToken(token),
             Times.Once);
        }

        [TestMethod]
        public void FindByVerificationToken_InvalidToken_ThrowsValidationException()
        {
            Guid token = new Guid();

            _authRepositoryMock
                .Setup(r => r.GetByVerificationToken(token))
                .Returns((Korisnici?)null);

            Assert.ThrowsException<ValidationException>(
               () => _service.FindByVerificationToken(token));

            _authRepositoryMock.Verify(
            r => r.GetByVerificationToken(token),
            Times.Once);
        }

        [TestMethod]
        public void VerifyEmail_UnverifiedUser_VerifiesAndUpdatesUser()
        {
            Guid token = Guid.NewGuid();

            var user = new Korisnici
            {
                Id = 1,
                Ime = "Test",
                Username = "test",
                Email = "test@example.com",
                IsEmailVerified = false,
                EmailVerificationToken = token
            };

            _authRepositoryMock
                .Setup(r => r.GetByVerificationToken(token))
                .Returns(user);

            _authRepositoryMock
                .Setup(r => r.Update(It.IsAny<Korisnici>()))
                .Returns((Korisnici u) => u);

            _service.VerifyEmail(token);

            Assert.IsTrue(user.IsEmailVerified);
            Assert.IsNull(user.EmailVerificationToken);

            _authRepositoryMock.Verify(
                r => r.Update(It.Is<Korisnici>(u =>
                u.Id == 1 &&
                u.IsEmailVerified == true &&
                u.EmailVerificationToken == null)),
                Times.Once);
        }

        [TestMethod]
        public void VerifyEmail_AlreadyVerified_DoesNotUpdate()
        {
            Guid token = new Guid();

            var user = new Korisnici
            {
                Id = 1,
                Ime = "Test",
                Username = "test",
                Email = "test@example.com",
                IsEmailVerified = true,
                EmailVerificationToken = token
            };

            _authRepositoryMock
                .Setup(r => r.GetByVerificationToken(token))
                .Returns(user);

            _service.VerifyEmail(token);

            Assert.IsTrue(user.IsEmailVerified);
            Assert.AreEqual(token, user.EmailVerificationToken);

            _authRepositoryMock.Verify(
                r => r.Update(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_ValidUser_UpdatesAndReturnsUser()
        {
            int userId = 1;

            var existingUser = new Korisnici
            {
                Id = userId,
                Ime = "Old Name",
                Username = "olduser",
                Email = "old@example.com",
                IsEmailVerified = false,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            var updatedUser = new Korisnici
            {
                Id = userId,
                Ime = "New Name",
                Username = "newuser",
                Email = "new@example.com",
                IsEmailVerified = true,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            var userRequest = new User
            {
                Id = userId,
                Name = "New Name",
                Username = "newuser",
                Email = "new@example.com",
                IsEmailVerified = true
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername("newuser"))
                .Returns((Korisnici?)null);

            _authRepositoryMock
                .Setup(r => r.GetById(userId))
                .Returns(existingUser);

            _authRepositoryMock
                .Setup(r => r.Update(It.IsAny<Korisnici>()))
                .Returns(updatedUser);

            var result = _service.Update(userId, userRequest);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.Id);
            Assert.AreEqual("New Name", result.Name);
            Assert.AreEqual("newuser", result.Username);
            Assert.AreEqual("new@example.com", result.Email);
            Assert.IsTrue(result.IsEmailVerified);

            _authRepositoryMock.Verify(
                r => r.Update(It.Is<Korisnici>(u =>
                u.Id == userId &&
                u.Ime == "New Name" &&
                u.Username == "newuser" &&
                u.Email == "new@example.com" &&
                u.IsEmailVerified)),
                Times.Once);
        }

        [TestMethod]
        public void Update_DuplicateUsername_ThrowsAlreadyExistsException()
        {
            int userId = 1;

            var existingUsernameUser = new Korisnici
            {
                Id = 2,
                Ime = "Other User",
                Username = "takenuser",
                Email = "other@example.com",
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            var userRequest = new User
            {
                Id = userId,
                Name = "Test",
                Username = "takenuser",
                Email = "test@example.com",
                IsEmailVerified = true
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername("takenuser"))
                .Returns(existingUsernameUser);

            Assert.ThrowsException<AlreadyExistsException>(
                () => _service.Update(userId, userRequest));

            _authRepositoryMock.Verify(
                r => r.Update(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_UserNotFound_ThrowsNotFoundException()
        {
            int userId = 1000;

            var userRequest = new User
            {
                Id = userId,
                Name = "Test",
                Username = "test",
                Email = "test@example.com",
                IsEmailVerified = true
            };

            _authRepositoryMock
                .Setup(r => r.GetById(userId))
                .Returns((Korisnici?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.Update(userId, userRequest));

            _authRepositoryMock.Verify(
                r => r.Update(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void Update_InvalidEmail_ThrowsValidationException()
        {
            int userId = 1;

            var userRequest = new User
            {
                Id = userId,
                Name = "Test",
                Username = "test",
                Email = "invalid-email",
                IsEmailVerified = true
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername("test"))
                .Returns((Korisnici?)null);

            Assert.ThrowsException<ValidationException>(
                () => _service.Update(userId, userRequest));

            _authRepositoryMock.Verify(
                r => r.GetById(It.IsAny<int>()),
                Times.Never);

            _authRepositoryMock.Verify(
                r => r.Update(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void Delete_ExistingUser_DeletesAndReturnsUser()
        {
            int userId = 1;

            var existingUser = new Korisnici
            {
                Id = userId,
                Ime = "Test",
                Username = "test",
                Email = "test@example.com",
                IsEmailVerified = true,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            var deletedUser = new Korisnici
            {
                Id = userId,
                Ime = "Test",
                Username = "test",
                Email = "test@example.com",
                IsEmailVerified = true,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            _authRepositoryMock
                .Setup(r => r.GetById(userId))
                .Returns(existingUser);

            _authRepositoryMock
                .Setup(r => r.Delete(existingUser))
                .Returns(deletedUser);

            var result = _service.Delete(userId);

            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.Id);
            Assert.AreEqual("test", result.Username);

            _authRepositoryMock.Verify(
                r => r.Delete(existingUser), 
                Times.Once);
        }

        [TestMethod]
        public void Delete_UserNotFound_ThrowsNotFoundException()
        {
            int userId = 1000;

            _authRepositoryMock
                .Setup(r => r.GetById(userId))
                .Returns((Korisnici?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.Delete(userId));

            _authRepositoryMock.Verify(
                r => r.Delete(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void Register_ValidRequest_ReturnsSuccess()
        {
            var request = new RegisterRequest
            {
                Name = "Test User",
                Username = "testuser",
                Email = "test@example.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                RoleNames = new List<string> { "User" }
            };

            var addedUser = new Korisnici
            {
                Id = 10,
                Ime = request.Name,
                Username = request.Username,
                Email = request.Email,
                PasswordHash = new byte[] { 1, 2, 3 },
                IsEmailVerified = false,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername(request.Username))
                .Returns((Korisnici?)null);

            _rolesRepositoryMock
                .Setup(r => r.GetNames())
                .Returns(new List<string> { "User", "Admin" });

            _rolesRepositoryMock
                .Setup(r => r.GetIdsByNames(request.RoleNames))
                .Returns(new List<int> { 1 });

            _authRepositoryMock
                .Setup(r => r.Add(It.IsAny<Korisnici>()))
                .Returns(addedUser);

            RegisterResponse result = _service.Register(request);

            Assert.AreEqual(10, result.Id);
            Assert.AreEqual("testuser", result.Username);
            Assert.AreEqual("test@example.com", result.Email);
            CollectionAssert.AreEqual(request.RoleNames.ToList(), result.Roles.ToList());
            Assert.AreEqual("Registration successful", result.Message);
        }

        [TestMethod]
        public void Register_MissingName_ThrowsValidationException()
        {
            var request = new RegisterRequest
            {
                Name = "",
                Username = "testuser",
                Email = "test@example.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                RoleNames = new List<string> { "User" }
            };

            Assert.ThrowsException<ValidationException>(
                () => _service.Register(request));

            _authRepositoryMock.Verify(
                r => r.Add(It.IsAny<Korisnici>()),
                Times.Never);
        }


        [TestMethod]
        public void Register_MissingUsername_ThrowsValidationException()
        {
            var request = new RegisterRequest
            {
                Name = "Test",
                Username = "",
                Email = "test@example.com",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                RoleNames = new List<string> { "User" }
            };

            Assert.ThrowsException<ValidationException>(
                () => _service.Register(request));

            _authRepositoryMock.Verify(
                r => r.Add(It.IsAny<Korisnici>()),
                Times.Never);
        }


        [TestMethod]
        public void Register_MissingEmail_ThrowsValidationException()
        {
            var request = new RegisterRequest
            {
                Name = "Test",
                Username = "testuser",
                Email = "",
                Password = "Test123!",
                ConfirmPassword = "Test123!",
                RoleNames = new List<string> { "User" }
            };

            Assert.ThrowsException<ValidationException>(
                () => _service.Register(request));

            _authRepositoryMock.Verify(
                r => r.Add(It.IsAny<Korisnici>()),
                Times.Never);
        }


        [TestMethod]
        public void Register_MissingPassword_ThrowsValidationException()
        {
            var request = new RegisterRequest
            {
                Name = "Test",
                Username = "testuser",
                Email = "test@example.com",
                Password = "",
                ConfirmPassword = "",
                RoleNames = new List<string> { "User" }
            };

            Assert.ThrowsException<ValidationException>(
                () => _service.Register(request));

            _authRepositoryMock.Verify(
                r => r.Add(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void Register_InvalidPassword_ThrowsValidationException()
        {
            var request = new RegisterRequest
            {
                Name = "Test User",
                Username = "testuser",
                Email = "test@example.com",
                Password = "password",
                ConfirmPassword = "password",
                RoleNames = new List<string> { "User" }
            };

            Assert.ThrowsException<ValidationException>(
                () => _service.Register(request));

            _authRepositoryMock.Verify(
                r => r.Add(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void Register_InvalidEmail_ThrowsValidationException()
        {
            var request = new RegisterRequest
            {
                Name = "Test User",
                Username = "testuser",
                Email = "invalid-email",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                RoleNames = new List<string> { "User" }
            };

            Assert.ThrowsException<ValidationException>(
                () => _service.Register(request));

            _authRepositoryMock.Verify(
                r => r.Add(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void Register_DuplicateUsername_ThrowsAlreadyExistsException()
        {
            var request = new RegisterRequest
            {
                Name = "Test User",
                Username = "existinguser",
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                RoleNames = new List<string> { "User" }
            };

            var existingUser = new Korisnici
            {
                Id = 5,
                Username = "existinguser",
                Email = "existing@example.com",
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername("existinguser"))
                .Returns(existingUser);

            Assert.ThrowsException<AlreadyExistsException>(
                () => _service.Register(request));

            _authRepositoryMock.Verify(
                r => r.Add(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void Register_InvalidRole_ThrowsValidationException()
        {
            var request = new RegisterRequest
            {
                Name = "Test User",
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                RoleNames = new List<string> { "InvalidRole" }
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername(request.Username))
                .Returns((Korisnici?)null);

            _rolesRepositoryMock
                .Setup(r => r.GetNames())
                .Returns(new List<string> { "User", "Admin" });

            Assert.ThrowsException<ValidationException>(
                () => _service.Register(request));

            _authRepositoryMock.Verify(
                r => r.Add(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void Register_ValidRequest_AddsUserCorrectly()
        {
            var request = new RegisterRequest
            {
                Name = "Test User",
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                RoleNames = new List<string> { "User" }
            };

            var addedUser = new Korisnici
            {
                Id = 10,
                Ime = request.Name,
                Username = request.Username,
                Email = request.Email,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername(request.Username))
                .Returns((Korisnici?)null);

            _rolesRepositoryMock
                .Setup(r => r.GetNames())
                .Returns(new List<string> { "User" });

            _rolesRepositoryMock
                .Setup(r => r.GetIdsByNames(request.RoleNames))
                .Returns(new List<int> { 1 });

            _authRepositoryMock
                .Setup(r => r.Add(It.IsAny<Korisnici>()))
                .Returns(addedUser);

            _service.Register(request);

            _authRepositoryMock.Verify(
                r => r.Add(It.Is<Korisnici>(u =>
                u.Ime == "Test User" &&
                u.Username == "testuser" &&
                u.Email == "test@example.com" &&
                u.IsEmailVerified == false &&
                u.EmailVerificationToken != null)),
                Times.Once);
        }

        [TestMethod]
        public void Register_ValidRequest_AssignsRoles()
        {
            var request = new RegisterRequest
            {
                Name = "Test User",
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                RoleNames = new List<string> { "User", "Admin" }
            };

            var addedUser = new Korisnici
            {
                Id = 10,
                Ime = request.Name,
                Username = request.Username,
                Email = request.Email,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            _authRepositoryMock
               .Setup(r => r.GetByUsername(request.Username))
               .Returns((Korisnici?)null);

            _rolesRepositoryMock
                .Setup(r => r.GetNames())
                .Returns(new List<string> { "User", "Admin" });

            _rolesRepositoryMock
                .Setup(r => r.GetIdsByNames(request.RoleNames))
                .Returns(new List<int> { 1, 2 });

            _authRepositoryMock
                .Setup(r => r.Add(It.IsAny<Korisnici>()))
                .Returns(addedUser);

            _service.Register(request);

            _usersRolesRepositoryMock.Verify(
                r => r.Add(It.Is<KorisniciUlogi>(ur =>
                ur.KorisnikId == 10 &&
                (ur.UlogaId == 1 || ur.UlogaId == 2))),
                Times.Exactly(2));
        }

        [TestMethod]
        public void Register_ValidRequest_SendsVerificationEmail()
        {
            var request = new RegisterRequest
            {
                Name = "Test User",
                Username = "testuser",
                Email = "test@example.com",
                Password = "Password123!",
                ConfirmPassword = "Password123!",
                RoleNames = new List<string> { "User" }
            };

            var addedUser = new Korisnici
            {
                Id = 10,
                Ime = request.Name,
                Username = request.Username,
                Email = request.Email,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            _authRepositoryMock
               .Setup(r => r.GetByUsername(request.Username))
               .Returns((Korisnici?)null);

            _rolesRepositoryMock
                .Setup(r => r.GetNames())
                .Returns(new List<string> { "User" });

            _rolesRepositoryMock
                .Setup(r => r.GetIdsByNames(request.RoleNames))
                .Returns(new List<int> { 1 });

            _authRepositoryMock
                .Setup(r => r.Add(It.IsAny<Korisnici>()))
                .Returns(addedUser);

            _service.Register(request);

            _emailServiceMock.Verify(
                e => e.SendEmail(
                    "test@example.com",
                    "Verify your PriceFlow account",
                    It.Is<string>(body =>
                    body.Contains("Welcome to PriceFlow") &&
                    body.Contains("Verify Email"))),
                Times.Once);
        }

        [TestMethod]
        public async Task Login_MissingUsername_ThrowsValidationException()
        {
            var request = new LoginRequest
            {
                Username = "",
                Password = "Password123!"
            };

            await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _service.Login(request));

            _authRepositoryMock.Verify(
                r => r.GetByUsername(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Login_MissingPassword_ThrowsValidationException()
        {
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = ""
            };

            await Assert.ThrowsExceptionAsync<ValidationException>(
                () => _service.Login(request));

            _authRepositoryMock.Verify(
                r => r.GetByUsername(It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Login_UserNotFound_ThrowsNotFoundException()
        {
            var request = new LoginRequest
            {
                Username = "unknown",
                Password = "Password123!"
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername(request.Username))
                .Returns((Korisnici?)null);

            await Assert.ThrowsExceptionAsync<NotFoundException>(
                () => _service.Login(request));
        }

        [TestMethod]
        public async Task Login_InvalidPassword_ThrowsUnauthorizedException()
        {
            var passwordHash = PasswordHelper.CalculateHashAndSalt("CorrectPassword123!");

            var user = new Korisnici
            {
                Id = 1,
                Ime = "Test User",
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                IsEmailVerified = true,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "WrongPassword123!"
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername(request.Username))
                .Returns(user);

            await Assert.ThrowsExceptionAsync<UnauthorizedException>(
                () => _service.Login(request));

            _rolesRepositoryMock.Verify(
                r => r.GetByUserId(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Login_EmailNotVerified_ThrowsUnauthorizedException()
        {
            var passwordHash = PasswordHelper.CalculateHashAndSalt("Password123!");

            var user = new Korisnici
            {
                Id = 1,
                Ime = "Test User",
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                IsEmailVerified = false,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "Password123!"
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername(request.Username))
                .Returns(user);

            await Assert.ThrowsExceptionAsync<UnauthorizedException>(
                () => _service.Login(request));

            _rolesRepositoryMock.Verify(
                r => r.GetByUserId(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Login_HttpContextNull_ThrowsException()
        {

            var passwordHash = PasswordHelper.CalculateHashAndSalt("Password123!");

            var user = new Korisnici
            {
                Id = 1,
                Ime = "Test User",
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                IsEmailVerified = true,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "Password123!"
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername(request.Username))
                .Returns(user);

            _httpContextAccessorMock
                .Setup(a => a.HttpContext)
                .Returns((HttpContext?)null);

            await Assert.ThrowsExceptionAsync<Exception>(
                () => _service.Login(request));

            _rolesRepositoryMock.Verify(
                r => r.GetByUserId(It.IsAny<int>()),
                Times.Never);
        }

        [TestMethod]
        public async Task Login_ValidCredentials_ReturnsSuccess()
        {
            var passwordHash = PasswordHelper.CalculateHashAndSalt("Password123!");

            var user = new Korisnici
            {
                Id = 1,
                Ime = "Test User",
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = passwordHash,
                IsEmailVerified = true,
                KorisniciUlogi = new List<KorisniciUlogi>()
            };

            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "Password123!"
            };

            var roles = new List<string> { "User", "Admin" };

            _authRepositoryMock
                .Setup(r => r.GetByUsername(request.Username))
                .Returns(user);

            _rolesRepositoryMock
                .Setup(r => r.GetByUserId(user.Id))
                .Returns(roles);

            var httpContext = new DefaultHttpContext();

            var authenticationServiceMock = new Mock<IAuthenticationService>();

            authenticationServiceMock
                .Setup(a => a.SignInAsync(
                    It.IsAny<HttpContext>(),
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);

            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IAuthenticationService)))
                .Returns(authenticationServiceMock.Object);

            httpContext.RequestServices = serviceProviderMock.Object;

            _httpContextAccessorMock
                .Setup(a => a.HttpContext)
                .Returns(httpContext);

            var result = await _service.Login(request);

            Assert.IsNotNull(result);
            Assert.AreEqual(user.Id, result.Id);
            Assert.AreEqual(user.Username, result.Username);
            Assert.AreEqual(user.Email, result.Email);
            CollectionAssert.AreEqual(roles, result.Roles);
            Assert.AreEqual("Login successful", result.Message);

            _rolesRepositoryMock.Verify(
                r => r.GetByUserId(user.Id),
                Times.Once);

            authenticationServiceMock.Verify(
                a => a.SignInAsync(
                    httpContext,
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    It.Is<ClaimsPrincipal>(p =>
                        p.Identity != null &&
                        p.Identity.Name == user.Username &&
                        p.Claims.Any(c =>
                            c.Type == ClaimTypes.NameIdentifier &&
                            c.Value == user.Id.ToString()) &&
                        p.Claims.Count(c =>
                            c.Type == ClaimTypes.Role) == 2),
                        It.Is<AuthenticationProperties>(p =>
                        p.IsPersistent)),
                    Times.Once);
        }

        [TestMethod]
        public void ForgotPassword_ExistingUser_UpdatesTokenAndSendsEmail()
        {
            var user = new Korisnici
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com"
            };

            _authRepositoryMock
                .Setup(r => r.GetByUsername(user.Username))
                .Returns(user);

            _authRepositoryMock
                .Setup(r => r.Update(It.IsAny<Korisnici>()))
                .Returns(user);

            _service.ForgotPassword(user.Username);

            Assert.IsNotNull(user.ResetPasswordToken);
            Assert.IsNotNull(user.ResetPasswordTokenExpiry);
            Assert.IsTrue(user.ResetPasswordTokenExpiry > DateTime.UtcNow);

            _authRepositoryMock.Verify(
                r => r.Update(It.Is<Korisnici>(u =>
                u.Id == user.Id &&
                u.ResetPasswordToken != null &&
                u.ResetPasswordTokenExpiry != null)),
                Times.Once);

            _emailServiceMock.Verify(
                e => e.SendEmail(
                    user.Email,
                    "Reset password",
                    It.Is<string>(body =>
                    body.Contains("Click") &&
                    body.Contains("forgot-password") &&
                    body.Contains("token="))),
                Times.Once);
        }

        [TestMethod]
        public void ForgotPassword_UserNotFound_ThrowsNotFoundException()
        {
            var username = "unknown";

            _authRepositoryMock
                .Setup(r => r.GetByUsername(username))
                .Returns((Korisnici?)null);

            Assert.ThrowsException<NotFoundException>(
                () => _service.ForgotPassword(username));

            _authRepositoryMock.Verify(
                r => r.Update(It.IsAny<Korisnici>()),
                Times.Never);

            _emailServiceMock.Verify(
                e => e.SendEmail(
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [TestMethod]
        public void ResetPassword_ValidToken_UpdatesPasswordAndClearsToken()
        {
            var token = Guid.NewGuid();

            var user = new Korisnici
            {
                Id = 1,
                Username = "testuser",
                Email = "test@example.com",
                ResetPasswordToken = token,
                ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(1),
                PasswordHash = PasswordHelper.CalculateHashAndSalt("OldPassword123!")
            };

            _authRepositoryMock
                .Setup(r => r.GetByResetPasswordToken(token))
                .Returns(user);

            _authRepositoryMock
                .Setup(r => r.Update(It.IsAny<Korisnici>()))
                .Returns(user);

            _service.ResetPassword(token, "NewPassword123!");

            Assert.IsNotNull(user.PasswordHash);
            Assert.IsNull(user.ResetPasswordToken);
            Assert.IsNull(user.ResetPasswordTokenExpiry);

            Assert.IsTrue(
                PasswordHelper.VerifyPassword(
                    "NewPassword123!",
                    user.PasswordHash));

            _authRepositoryMock.Verify(
                r => r.GetByResetPasswordToken(token),
                Times.Once);

            _authRepositoryMock.Verify(
                r => r.Update(It.Is<Korisnici>(u =>
                u.Id == user.Id &&
                u.ResetPasswordToken == null &&
                u.ResetPasswordTokenExpiry == null)),
                Times.Once);
        }

        [TestMethod]
        public void ResetPassword_Invalidoken_ThrowsValidationException()
        {
            var token = Guid.NewGuid();

            _authRepositoryMock
                .Setup(r => r.GetByResetPasswordToken(token))
                .Returns((Korisnici?)null);

            Assert.ThrowsException<ValidationException>(
                () => _service.ResetPassword(token, "NewPassword123!"));

            _authRepositoryMock.Verify(
                r => r.Update(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void ResetPassword_ExpiredToken_ThrowsValidationException()
        {
            var token = Guid.NewGuid();

            var user = new Korisnici
            {
                Id = 1,
                Username = "testuser",
                ResetPasswordToken = token,
                ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(-1)
            };

            _authRepositoryMock
             .Setup(r => r.GetByResetPasswordToken(token))
             .Returns(user);

            Assert.ThrowsException<ValidationException>(
                () => _service.ResetPassword(token, "NewPassword123!"));

            _authRepositoryMock.Verify(
                r => r.Update(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void ResetPassword_InvalidPassword_ThrowsValidationException()
        {
            var token = Guid.NewGuid();

            Assert.ThrowsException<ValidationException>(
                () => _service.ResetPassword(token, "password"));

            _authRepositoryMock.Verify(
                r => r.GetByResetPasswordToken(It.IsAny<Guid>()),
                Times.Never);

            _authRepositoryMock.Verify(
                r => r.Update(It.IsAny<Korisnici>()),
                Times.Never);
        }

        [TestMethod]
        public void FindAll_ExistingUsers_ReturnsMappedUsers()
        {
            var users = new List<Korisnici>
            {
                new Korisnici
                {
                    Id = 1,
                    Ime = "User 1",
                    Username = "user1",
                    Email = "user1@example.com",
                    KorisniciUlogi = new List<KorisniciUlogi>()
                },
                 new Korisnici
                {
                    Id = 2,
                    Ime = "User 2",
                    Username = "user2",
                    Email = "user2@example.com",
                    KorisniciUlogi = new List<KorisniciUlogi>()
                }
            };

            _authRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(users);

            var result = _service.FindAll().ToList();

            Assert.AreEqual(2, result.Count);

            Assert.AreEqual(1, result[0].Id);
            Assert.AreEqual("User 1", result[0].Name);
            Assert.AreEqual("user1", result[0].Username);
            Assert.AreEqual("user1@example.com", result[0].Email);

            Assert.AreEqual(2, result[1].Id);
            Assert.AreEqual("User 2", result[1].Name);
            Assert.AreEqual("user2", result[1].Username);
            Assert.AreEqual("user2@example.com", result[1].Email);

            _authRepositoryMock.Verify(
                r => r.GetAll(),
                Times.Once);
        }

        [TestMethod]
        public void FindAll_NoUsers_ReturnsEmptyCollection()
        {
            _authRepositoryMock
                .Setup(r => r.GetAll())
                .Returns(new List<Korisnici>());

            var result = _service.FindAll().ToList();

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Count);

            _authRepositoryMock.Verify(
                r => r.GetAll(),
                Times.Once);
        }
    }
}
