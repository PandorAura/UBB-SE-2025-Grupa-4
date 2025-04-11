using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Models;
using App1.Repositories;
using Moq;
using Xunit;

namespace UnitTests.Roles
{
    public class RolesRepositoryTests
    {
        private readonly RolesRepository _repository;

        public RolesRepositoryTests()
        {
            _repository = new RolesRepository();
        }

        /// <summary>
        /// Tests that GetAllRoles returns the complete set of predefined roles in the system.
        /// This test ensures that the repository correctly initializes and maintains the role hierarchy:
        /// - Banned (0)
        /// - User (1)
        /// - Admin (2)
        /// - Manager (3)
        /// </summary>
        [Fact]
        public void GetAllRoles_WhenCalled_ReturnsAllRoles()
        {
            var expectedRoles = new List<Role>
            {
                new Role(RoleType.Banned, "Banned"),
                new Role(RoleType.User, "User"),
                new Role(RoleType.Admin, "Admin"),
                new Role(RoleType.Manager, "Manager")
            };

            var result = _repository.GetAllRoles();

            Assert.Equal(expectedRoles.Count, result.Count);
            for (int i = 0; i < expectedRoles.Count; i++)
            {
                Assert.Equal(expectedRoles[i].RoleType, result[i].RoleType);
                Assert.Equal(expectedRoles[i].RoleName, result[i].RoleName);
            }
        }

        /// <summary>
        /// Tests the role promotion functionality by verifying that GetNextRoleInHierarchy returns the correct next role
        /// in the hierarchy for each valid current role. This ensures the role promotion system works correctly:
        /// - Banned -> User
        /// - User -> Admin
        /// - Admin -> Manager
        /// </summary>
        [Theory]
        [InlineData(RoleType.Banned, RoleType.User)]
        [InlineData(RoleType.User, RoleType.Admin)]
        [InlineData(RoleType.Admin, RoleType.Manager)]
        public void GetNextRoleInHierarchy_WhenValidCurrentRole_ReturnsNextRole(RoleType currentRole, RoleType expectedNextRole)
        {
            var result = _repository.GetNextRoleInHierarchy(currentRole);

            Assert.Equal(expectedNextRole, result.RoleType);
        }

        /// <summary>
        /// Tests that attempting to get the next role for a Manager throws an exception.
        /// Manager is the highest role in the hierarchy and
        /// there should be no role above it to promote to.
        /// </summary>
        [Fact]
        public void GetNextRoleInHierarchy_WhenManagerRole_ThrowsException()
        {
            var currentRole = RoleType.Manager;

            Assert.Throws<InvalidOperationException>(() => _repository.GetNextRoleInHierarchy(currentRole));
        }

        /// <summary>
        /// Tests that attempting to get the next role for an invalid role type throws an exception.
        /// This ensures that the role promotion system properly handles invalid role types.
        /// </summary>
        [Fact]
        public void GetNextRoleInHierarchy_WhenInvalidRoleType_ThrowsException()
        {
            var invalidRoleType = (RoleType)999; // A role type that doesn't exist

            Assert.Throws<InvalidOperationException>(() => _repository.GetNextRoleInHierarchy(invalidRoleType));
        }

        /// <summary>
        /// Tests that attempting to get the next role with a negative role type throws an ArgumentException.
        /// This ensures that the role promotion system properly validates role types.
        /// </summary>
        [Fact]
        public void GetNextRoleInHierarchy_WhenNegativeRoleType_ThrowsArgumentException()
        {
            var negativeRoleType = (RoleType)(-1);

            try
            {
                _repository.GetNextRoleInHierarchy(negativeRoleType);
                
                Assert.True(false, "Expected ArgumentException was not thrown");
            }
            catch (ArgumentException ex)
            {
                Assert.Contains("Role type must be non-negative", ex.Message);
            }
        }
    }
}
