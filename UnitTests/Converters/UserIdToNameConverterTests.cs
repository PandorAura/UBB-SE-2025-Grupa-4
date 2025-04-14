using App1.Converters;
using App1.Models;
using App1.Services;
using Moq;
using Xunit;
using System.Reflection;

namespace UnitTests.Converters
{
    public class UserIdToNameConverterTests : IDisposable
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserIdToNameConverter _converter;
        private static readonly FieldInfo _userServiceField = typeof(UserIdToNameConverter)
            .GetField("_userService", BindingFlags.NonPublic | BindingFlags.Static);

        public UserIdToNameConverterTests()
        {
            _mockUserService = new Mock<IUserService>();
            _converter = new UserIdToNameConverter();
            ResetUserService();
        }

        public void Dispose()
        {
            ResetUserService();
        }

        private void ResetUserService()
        {
            _userServiceField.SetValue(null, null);
        }

        [Fact]
        public void Initialize_SetsUserService()
        {
            // Act
            UserIdToNameConverter.Initialize(_mockUserService.Object);

            // Assert
            Assert.NotNull(_mockUserService.Object);
        }

        [Fact]
        public void Convert_WithValidUserId_ReturnsUserName()
        {
            // Arrange
            int userId = 1;
            string expectedName = "John Doe";
            var user = new User { FullName = expectedName };
            _mockUserService.Setup(s => s.GetUserById(userId)).Returns(user);
            UserIdToNameConverter.Initialize(_mockUserService.Object);

            // Act
            var result = _converter.Convert(userId, null, null, null);

            // Assert
            Assert.Equal(expectedName, result);
            _mockUserService.Verify(s => s.GetUserById(userId), Times.Once);
        }

        [Fact]
        public void Convert_WithNullUserService_ReturnsUnknownUser()
        {
            // Arrange
            UserIdToNameConverter.Initialize(null);
            int userId = 1;

            // Act
            var result = _converter.Convert(userId, null, null, null);

            // Assert
            Assert.Equal("Unknown User", result);
        }

        [Fact]
        public void Convert_WithUserNotFound_ReturnsDefaultName()
        {
            // Arrange
            int userId = 1;
            _mockUserService.Setup(s => s.GetUserById(userId)).Returns((User)null);
            UserIdToNameConverter.Initialize(_mockUserService.Object);

            // Act
            var result = _converter.Convert(userId, null, null, null);

            // Assert
            Assert.Equal($"User {userId}", result);
            _mockUserService.Verify(s => s.GetUserById(userId), Times.Once);
        }

        [Fact]
        public void Convert_WithUserServiceThrowingException_ReturnsDefaultName()
        {
            // Arrange
            int userId = 1;
            _mockUserService.Setup(s => s.GetUserById(userId))
                .Throws(new Exception("Test exception"));
            UserIdToNameConverter.Initialize(_mockUserService.Object);

            // Act
            var result = _converter.Convert(userId, null, null, null);

            // Assert
            Assert.Equal($"User {userId}", result);
            _mockUserService.Verify(s => s.GetUserById(userId), Times.Once);
        }

        [Fact]
        public void Convert_WithNullValue_ReturnsUnknownUser()
        {
            // Arrange
            UserIdToNameConverter.Initialize(_mockUserService.Object);

            // Act
            var result = _converter.Convert(null, null, null, null);

            // Assert
            Assert.Equal("Unknown User", result);
        }

        [Fact]
        public void Convert_WithNonIntegerValue_ReturnsUnknownUser()
        {
            // Arrange
            UserIdToNameConverter.Initialize(_mockUserService.Object);

            // Act
            var result = _converter.Convert("not an integer", null, null, null);

            // Assert
            Assert.Equal("Unknown User", result);
        }

        [Fact]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            // Arrange
            UserIdToNameConverter.Initialize(_mockUserService.Object);

            // Act & Assert
            Assert.Throws<NotImplementedException>(() => 
                _converter.ConvertBack(null, null, null, null));
        }
    }
} 