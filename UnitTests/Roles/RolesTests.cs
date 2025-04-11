using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Models;
using App1.Repositories;
using Xunit;

namespace UnitTests.Roles
{
    public class RolesTests
    {
        /// <summary>
        /// Tests that the Role model correctly initializes with valid RoleType and RoleName.
        /// This test ensures that:
        /// 1. RoleType is correctly assigned
        /// 2. RoleName is correctly assigned
        /// 3. The model can be created with all valid role types
        /// </summary>
        [Fact]
        public void Role_WhenCreated_InitializesCorrectly()
        {
            var testCases = new[]
            {
                new { Type = RoleType.Banned, Name = "Banned" },
                new { Type = RoleType.User, Name = "User" },
                new { Type = RoleType.Admin, Name = "Admin" },
                new { Type = RoleType.Manager, Name = "Manager" }
            };

            foreach (var testCase in testCases)
            {
                var role = new Role(testCase.Type, testCase.Name);

                Assert.Equal(testCase.Type, role.RoleType);
                Assert.Equal(testCase.Name, role.RoleName);
            }
        }

        /// <summary>
        /// Tests that the Role model handles null or empty role names correctly.
        /// This ensures that the model can be created with empty role names,
        /// though this might not be a valid business case.
        /// </summary>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Role_WhenCreatedWithNullOrEmptyName_InitializesCorrectly(string roleName)
        {
            // Act
            var role = new Role(RoleType.User, roleName);

            // Assert
            Assert.Equal(RoleType.User, role.RoleType);
            Assert.Equal(roleName, role.RoleName);
        }

        /// <summary>
        /// Tests that the Role model properties can be modified after creation.
        /// This verifies that the properties are properly settable.
        /// </summary>
        [Fact]
        public void Role_WhenPropertiesModified_UpdatesCorrectly()
        {
            // Arrange
            var role = new Role(RoleType.User, "User");

            // Act
            role.RoleType = RoleType.Admin;
            role.RoleName = "Administrator";

            // Assert
            Assert.Equal(RoleType.Admin, role.RoleType);
            Assert.Equal("Administrator", role.RoleName);
        }
    }
}
