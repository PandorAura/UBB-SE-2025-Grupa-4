using App1.Models;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.Users
{
    public class UserTests
    {
        [Fact]
        public void Constructor_ShouldInitializePropertiesCorrectly()
        {
            int userId = 1;
            string emailAddress = "test@example.com";
            string fullName = "Test User";
            int numberOfDeletedReviews = 2;
            bool hasSubmittedAppeal = true;
            List<Role> assignedRoles = new List<Role>
            {
                new Role(RoleType.User, "User")
            };
            var exception = Record.Exception(() => new User(userId, emailAddress, fullName, numberOfDeletedReviews, hasSubmittedAppeal, assignedRoles));
            Assert.Null(exception);

            User user = new User(userId, emailAddress, fullName, numberOfDeletedReviews, hasSubmittedAppeal, assignedRoles);

            Assert.Equal(userId, user.UserId);
            Assert.Equal(emailAddress, user.EmailAddress);
            Assert.Equal(fullName, user.FullName);
            Assert.Equal(numberOfDeletedReviews, user.NumberOfDeletedReviews);
            Assert.True(user.HasSubmittedAppeal);
            Assert.Equal(assignedRoles, user.AssignedRoles);
        }

        [Fact]
        public void ToString_ShouldReturnCorrectStringRepresentation()
        {
            // Arrange
            User user = new User
            {
                UserId = 1,
                EmailAddress = "test@example.com"
            };

            // Act
            string result = user.ToString();

            // Assert
            Assert.Equal("Id: 1, email: test@example.com", result);
        }
    }
}
