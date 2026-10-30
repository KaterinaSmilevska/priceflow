using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using PriceFlowApp.Controllers;
using PriceFlowApp.DTOs;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;
using System.Security.Claims;


namespace Tests.Unit.Controllers
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _authServiceMock = null!;
        private Mock<IRolesService> _rolesServiceMock = null!;
        private AuthController _controller = null!;

        private const int UserId = 10;

        [TestInitialize]
        public void Setup()
        {
            _authServiceMock = new Mock<IAuthService>();
            _rolesServiceMock = new Mock<IRolesService>();
            _controller = new AuthController(_authServiceMock.Object, _rolesServiceMock.Object);

            var authenticationServiceMock = new Mock<IAuthenticationService>();

            authenticationServiceMock
                .Setup(s => s.SignOutAsync(
                    It.IsAny<HttpContext>(),
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    It.IsAny<AuthenticationProperties?>()))
                .Returns(Task.CompletedTask);

            var services = new ServiceCollection();

            services.AddSingleton(
                authenticationServiceMock.Object);

            var serviceProvider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProvider
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        private void SetUser(bool isAuthenticated, string username = "testuser")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, UserId.ToString()),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User")
            };

            var identity = new ClaimsIdentity(
                isAuthenticated ? claims : new List<Claim>(),
                isAuthenticated ? "TestAuthentication" : null);

            _controller.HttpContext.User = new ClaimsPrincipal(identity);
        }

        [TestMethod]
        public void Register_WhenSuccessful_ReturnsOkWithResponse()
        {
            var request = new RegisterRequest();
            var response = new RegisterResponse();

            _authServiceMock
                .Setup(s => s.Register(request))
                .Returns(response);

            var result = _controller.Register(request);

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(response, ok.Value);
        }

        [TestMethod]
        public void Register_WhenServiceThrowsPriceFlowException_ReturnsMappedError()
        {
            var request = new RegisterRequest();

            _authServiceMock
                .Setup(s => s.Register(request))
                .Throws(new ValidationException(
                    "VALIDATION_REQUIRED_FIELD_REGISTER",
                    "All fieldsa are required."));

            var result = _controller.Register(request);

            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(400, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task Login_WhenSuccessful_ReturnsOkWithResponse()
        {
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "TestUser123."
            };
            var response = new LoginResponse
            {
                Message = "Logged in successfully!"
            };

            _authServiceMock
                .Setup(s => s.Login(request))
                .ReturnsAsync(response);

            var result = await _controller.Login(request);

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(response, ok.Value);
        }

        [TestMethod]
        public async Task Login_WhenServiceThrowsUnexpectedException_ReturnsInternalServerError()
        {
            var request = new LoginRequest
            {
                Username = "testuser",
                Password = "TestUser123."
            };

            _authServiceMock
                .Setup(s => s.Login(request))
                .ThrowsAsync(new Exception("Unexpected error"));

            var result = await _controller.Login(request);

            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode);
        }

        [TestMethod]
        public async Task Logout_WhenCalled_ReturnsSuccessfulResponse()
        {
            var result = await _controller.Logout();

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);

            var response = ok.Value as LogoutResponse;

            Assert.IsNotNull(response);
            Assert.IsTrue(response.Success);
            Assert.AreEqual("Logged out successfully!", response.Message);
        }

        [TestMethod]
        public void GetRoleNames_WhenRolesExist_ReturnsOkWithRoles()
        {
            var roles = new List<string>
            {
                "Admin",
                "User"
            };

            _rolesServiceMock
                .Setup(s => s.FindNames())
                .Returns(roles);

            var result = _controller.GetRolesNames();

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
            Assert.AreSame(roles, ok.Value);
        }

        [TestMethod]
        public void CheckUsername_WhenUsernameExists_ReturnsTrue()
        {
            _authServiceMock
                .Setup(s => s.UsernameExists("testuser"))
                .Returns(true);

            var result = _controller.CheckUsername("testuser");

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(true, ok.Value);
        }

        [TestMethod]
        public void CheckUsername_WhenUsernameDoesNotExist_ReturnsFalse()
        {
            _authServiceMock
           .Setup(s => s.UsernameExists("newuser"))
           .Returns(false);

            var result = _controller.CheckUsername("newuser");

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(false, ok.Value);
        }

        [TestMethod]
        public void ValidatePassword_WhenServiceReturnsResult_ReturnsOk()
        {
            var request = new PasswordValidationRequest
            {
                Password = "TestUser123.",
                ConfirmPassword = "TestUser123."
            };

            var response = new PasswordValidationResponse
            {
                Message = "Password validation successful!",
                IsValid = true
            };

            _authServiceMock
                .Setup(s => s.ValidatePassword(request))
                .Returns(response);

            var result = _controller.ValidatePassword(request);

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreSame(response, ok.Value);
        }

        [TestMethod]
        public void ValidateEmail_WhenServiceReturnsResult_ReturnsOk()
        {
            var request = new EmailValidationRequest
            {
                Email = "test@example.com"
            };

            var response = new EmailValidationResponse
            {
                Message = "Email validation successful!",
                IsValid = true
            };

            _authServiceMock
                .Setup(s => s.ValidateEmail(request))
                .Returns(response);

            var result = _controller.ValidateEmail(request);

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreSame(response, ok.Value);
        }

        [TestMethod]
        public void ForgotPassword_WhenSuccessful_ReturnsOk()
        {
            var request = new ForgotPasswordRequest
            {
                Username = "testuser"
            };

            _authServiceMock
                .Setup(s => s.ForgotPassword("testuser"));

            var result = _controller.ForgotPassword(request);

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);

            var response = ok.Value as MessageResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual("Reset link has been sent to your email.", response.Message);

            _authServiceMock.Verify(
                r => r.ForgotPassword("testuser"),
                Times.Once);
        }

        [TestMethod]
        public void ResetPassword_WhenSuccessful_ReturnsOk()
        {
            var token = Guid.NewGuid();

            var request = new ResetPasswordRequest
            {
                Token = token,
                NewPassword = "NewPassword123!"
            };

            _authServiceMock
                .Setup(s => s.ResetPassword(token, "NewPassword123!"));

            var result = _controller.ResetPassword(request);

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);

            var response = ok.Value as MessageResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual("Password reset successful.", response.Message);

            _authServiceMock.Verify(
                r => r.ResetPassword(token, "NewPassword123!"),
                Times.Once);
        }

        [TestMethod]
        public void Status_WhenUserIsUnauthenticated_ReturnsLoggedOutStatus()
        {
            SetUser(false);

            var result = _controller.Status();

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);

            var response = ok.Value as SessionStatusResponse;

            Assert.IsNotNull(response);
            Assert.IsFalse(response.IsLoggedIn);
        }

        [TestMethod]
        public void Status_WhenUserIsAuthenticated_ReturnsUserAndRoles()
        {
            SetUser(true, "testuser");

            var result = _controller.Status();

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);

            var response = ok.Value as SessionStatusResponse;

            Assert.IsNotNull(response);
            Assert.IsTrue(response.IsLoggedIn);
            Assert.AreEqual("testuser", response.Username);
            Assert.AreEqual(UserId.ToString(), response.UserId);
            CollectionAssert.AreEqual(new List<string> { "User" }, response.Roles);
        }

        [TestMethod]
        public void GetUsers_WhenUsersExist_ReturnsOkWithUsers()
        {
            var users = new List<User>();

            _authServiceMock
                .Setup(s => s.FindAll())
                .Returns(users);

            var result = _controller.GetUsers();

            var ok = result.Result as OkObjectResult;
            
            Assert.IsNotNull(ok);
            Assert.AreSame(users, ok.Value);
        }

        [TestMethod]
        public void GetUser_WhenUserExists_ReturnsOkWithUser()
        {
            var user = new User
            {
                Id = UserId
            };

            _authServiceMock
                .Setup(s => s.FindById(UserId))
                .Returns(user);

            var result = _controller.GetUser(UserId);

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreSame(user, ok.Value);
        }

        [TestMethod]
        public void UpdateUser_WhenIdsDoNotMatch_ReturnsBadRequestWithoutCallingService()
        {
            var user = new User
            {
                Id = 1000
            };

            var result = _controller.UpdateUser(UserId, user);

            var badRequest = result.Result as BadRequestObjectResult;

            Assert.IsNotNull(badRequest);
            Assert.AreEqual(400, badRequest.StatusCode);

            _authServiceMock.Verify(
                s => s.Update(It.IsAny<int>(),
                It.IsAny<User>()),
                Times.Never);
        }

        [TestMethod]
        public void UpdateUser_WhenIdsMatch_ReturnsOkWithUpdatedUser()
        {
            var user = new User
            {
                Id = UserId
            };

            _authServiceMock
                .Setup(s => s.Update(UserId, user))
                .Returns(user);

            var result = _controller.UpdateUser(UserId, user);

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreSame(user, ok.Value);
        }

        [TestMethod]
        public void DeleteUser_WhenUserExists_ReturnsOkayWithDeletedUser()
        {
            var user = new User
            {
                Id = UserId
            };

            _authServiceMock
                .Setup(s => s.Delete(UserId))
                .Returns(user);

            var result = _controller.DeleteUser(UserId);

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);
        }

        [TestMethod]
        public void VerifyEmail_WhenSuccessful_ReturnsOk()
        {
            var token = Guid.NewGuid();

            _authServiceMock
                .Setup(s => s.VerifyEmail(token));

            var result = _controller.VerifyEmail(token);

            var ok = result.Result as OkObjectResult;

            Assert.IsNotNull(ok);
            Assert.AreEqual(200, ok.StatusCode);

            var response = ok.Value as MessageResponse;

            Assert.IsNotNull(response);
            Assert.AreEqual("Email verification successful.", response.Message);

            _authServiceMock.Verify(
                s => s.VerifyEmail(token),
                Times.Once);
        }
    }
}
