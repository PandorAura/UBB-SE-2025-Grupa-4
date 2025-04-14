using App1.Converters;
using App1.Models;
using App1.Services;
using Moq;
using Xunit;
using System;
using System.Reflection;

namespace UnitTests.Converters
{
    public class UserIdToNameConverterTests
    {
        // Use a class fixture to ensure tests run sequentially
        [Collection("Sequential")]
        public class UserIdToNameConverterTestsSequential
        {
            private static readonly FieldInfo _userServiceField = typeof(UserIdToNameConverter)
                .GetField("_userService", BindingFlags.NonPublic | BindingFlags.Static);

            // Reset the static field before each test
            private void ResetUserService()
            {
                _userServiceField.SetValue(null, null);
            }

            [Fact]
            public void Initialize_SetsUserService()
            {
                try
                {
                    // Arrange
                    ResetUserService();
                    var mockUserService = new Mock<IUserService>();

                    // Act
                    UserIdToNameConverter.Initialize(mockUserService.Object);

                    // Assert
                    var userService = _userServiceField.GetValue(null);
                    Assert.Same(mockUserService.Object, userService);
                }
                finally
                {
                    ResetUserService();
                }
            }

            [Fact]
            public void Convert_WithValidUserId_ReturnsUserName()
            {
                try
                {
                    // Arrange
                    ResetUserService();
                    int userId = 1;
                    string expectedName = "John Doe";
                    var user = new User { FullName = expectedName };

                    var mockUserService = new Mock<IUserService>();
                    mockUserService.Setup(s => s.GetUserById(userId)).Returns(user);
                    UserIdToNameConverter.Initialize(mockUserService.Object);
                    var converter = new UserIdToNameConverter();

                    // Act
                    var result = converter.Convert(userId, null, null, null);

                    // Assert
                    Assert.Equal(expectedName, result);
                    mockUserService.Verify(s => s.GetUserById(userId), Times.Once);
                }
                finally
                {
                    ResetUserService();
                }
            }

            [Fact]
            public void Convert_WithNullUserService_ReturnsUnknownUser()
            {
                try
                {
                    // Arrange
                    ResetUserService(); // This ensures _userService is null
                    int userId = 1;
                    var converter = new UserIdToNameConverter();

                    // Act
                    var result = converter.Convert(userId, null, null, null);

                    // Assert
                    Assert.Equal("Unknown User", result);
                }
                finally
                {
                    ResetUserService();
                }
            }

            [Fact]
            public void Convert_WithUserNotFound_ReturnsDefaultName()
            {
                try
                {
                    // Arrange
                    ResetUserService();
                    int userId = 1;

                    var mockUserService = new Mock<IUserService>();
                    mockUserService.Setup(s => s.GetUserById(userId)).Returns((User)null);
                    UserIdToNameConverter.Initialize(mockUserService.Object);
                    var converter = new UserIdToNameConverter();

                    // Act
                    var result = converter.Convert(userId, null, null, null);

                    // Assert
                    Assert.Equal($"User {userId}", result);
                    mockUserService.Verify(s => s.GetUserById(userId), Times.Once);
                }
                finally
                {
                    ResetUserService();
                }
            }

            [Fact]
            public void Convert_WithUserServiceThrowingException_ReturnsDefaultName()
            {
                try
                {
                    // Arrange
                    ResetUserService();
                    int userId = 1;

                    var mockUserService = new Mock<IUserService>();
                    mockUserService.Setup(s => s.GetUserById(userId)).Throws(new Exception("Test exception"));
                    UserIdToNameConverter.Initialize(mockUserService.Object);
                    var converter = new UserIdToNameConverter();

                    // Act
                    var result = converter.Convert(userId, null, null, null);

                    // Assert
                    Assert.Equal($"User {userId}", result);
                    mockUserService.Verify(s => s.GetUserById(userId), Times.Once);
                }
                finally
                {
                    ResetUserService();
                }
            }

            [Fact]
            public void Convert_WithNullValue_ReturnsUnknownUser()
            {
                try
                {
                    // Arrange
                    ResetUserService();
                    var mockUserService = new Mock<IUserService>();
                    UserIdToNameConverter.Initialize(mockUserService.Object);
                    var converter = new UserIdToNameConverter();

                    // Act
                    var result = converter.Convert(null, null, null, null);

                    // Assert
                    Assert.Equal("Unknown User", result);
                    // No need to verify since service shouldn't be called
                }
                finally
                {
                    ResetUserService();
                }
            }

            [Fact]
            public void Convert_WithNonIntegerValue_ReturnsUnknownUser()
            {
                try
                {
                    // Arrange
                    ResetUserService();
                    var mockUserService = new Mock<IUserService>();
                    UserIdToNameConverter.Initialize(mockUserService.Object);
                    var converter = new UserIdToNameConverter();

                    // Act
                    var result = converter.Convert("not an integer", null, null, null);

                    // Assert
                    Assert.Equal("Unknown User", result);
                    // No need to verify since service shouldn't be called
                }
                finally
                {
                    ResetUserService();
                }
            }

            [Fact]
            public void Convert_WithUserFoundButNullFullName_ReturnsDefaultName()
            {
                try
                {
                    // Arrange
                    ResetUserService();
                    int userId = 1;
                    var user = new User { FullName = null };

                    var mockUserService = new Mock<IUserService>();
                    mockUserService.Setup(s => s.GetUserById(userId)).Returns(user);
                    UserIdToNameConverter.Initialize(mockUserService.Object);
                    var converter = new UserIdToNameConverter();

                    // Act
                    var result = converter.Convert(userId, null, null, null);

                    // Assert
                    Assert.Equal($"User {userId}", result);
                    mockUserService.Verify(s => s.GetUserById(userId), Times.Once);
                }
                finally
                {
                    ResetUserService();
                }
            }

            [Fact]
            public void Convert_WithUserFoundButEmptyFullName_ReturnsDefaultName()
            {
                try
                {
                    // Arrange
                    ResetUserService();
                    int userId = 1;
                    var user = new User { FullName = string.Empty };

                    var mockUserService = new Mock<IUserService>();
                    mockUserService.Setup(s => s.GetUserById(userId)).Returns(user);
                    UserIdToNameConverter.Initialize(mockUserService.Object);
                    var converter = new UserIdToNameConverter();

                    // Act
                    var result = converter.Convert(userId, null, null, null);

                    // Assert
                    Assert.Equal($"User {userId}", result);
                    mockUserService.Verify(s => s.GetUserById(userId), Times.Once);
                }
                finally
                {
                    ResetUserService();
                }
            }

            [Fact]
            public void Convert_WithIntValueButNotBoxedAsInt_ReturnsUnknownUser()
            {
                try
                {
                    // Arrange
                    ResetUserService();
                    var mockUserService = new Mock<IUserService>();
                    UserIdToNameConverter.Initialize(mockUserService.Object);
                    var converter = new UserIdToNameConverter();

                    // This is specifically to test type checking branch
                    object value = 1L; // Long instead of int

                    // Act
                    var result = converter.Convert(value, null, null, null);

                    // Assert
                    Assert.Equal("Unknown User", result);
                    // No need to verify since service shouldn't be called
                }
                finally
                {
                    ResetUserService();
                }
            }

            [Fact]
            public void ConvertBack_ThrowsNotImplementedException()
            {
                try
                {
                    // Arrange
                    ResetUserService();
                    var converter = new UserIdToNameConverter();

                    // Act & Assert
                    Assert.Throws<NotImplementedException>(() =>
                        converter.ConvertBack(null, null, null, null));
                }
                finally
                {
                    ResetUserService();
                }
            }
        }
    }
}