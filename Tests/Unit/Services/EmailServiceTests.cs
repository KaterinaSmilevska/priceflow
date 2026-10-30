using Microsoft.Extensions.Configuration;
using Moq;
using PriceFlowApp.Exceptions;
using PriceFlowApp.Services;

namespace Tests.Unit.Services
{
    [TestClass]
    public class EmailServiceTests
    {
        private Mock<IConfiguration> _configurationMock = null!;
        private EmailService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();
            _service = new EmailService(_configurationMock.Object);
        }

        [TestMethod]
        public void SendEmail_EmptyRecipientEmail_ThrowsValidationException()
        {
            Assert.ThrowsException<ValidationException>(
                () => _service.SendEmail("", "Test Subject", "Test Body"));

            _configurationMock.Verify(
                c => c[It.IsAny<string>()],
                Times.Never);
        }

        [TestMethod]
        public void SendEmail_EmptySubject_ThrowsValidationException()
        {
            Assert.ThrowsException<ValidationException>(
               () => _service.SendEmail("test@example.com", "", "Test Body"));

            _configurationMock.Verify(
                c => c[It.IsAny<string>()],
                Times.Never);
        }

        [TestMethod]
        public void SendEmail_EmptyBody_ThrowsValidationException()
        {
            Assert.ThrowsException<ValidationException>(
               () => _service.SendEmail("test@example.com", "Test Subject", ""));

            _configurationMock.Verify(
                c => c[It.IsAny<string>()],
                Times.Never);
        }

        [TestMethod]
        public void SendEmail_MissingSmtpHost_ThrowsInvalidOperationException()
        {
            _configurationMock
                .Setup(c => c[It.IsAny<string>()])
                .Returns((string?)null);

            Assert.ThrowsException<InvalidOperationException>(
                () => _service.SendEmail(
                    "test@example.com",
                    "Test Subject",
                    "Test Body"));
        }

        [TestMethod]
        public void SendEmail_MissingSmtpPassword_ThrowsInvalidOperationException()
        {
            _configurationMock
                .Setup(c => c["Smtp:Host"])
                .Returns("smtp.example.com");

            _configurationMock
                .Setup(c => c["Smtp:Port"])
                .Returns("587");

            _configurationMock
                .Setup(c => c["Smtp:Username"])
                .Returns("username");

            _configurationMock
                .Setup(c => c["Smtp:Password"])
                .Returns("");

            _configurationMock
                .Setup(c => c["Smtp:Email"])
                .Returns("noreply@example.com");

            Assert.ThrowsException<InvalidOperationException>(
                () => _service.SendEmail(
                    "test@example.com",
                    "Test Subject",
                    "Test Body"));
        }
    }
}
