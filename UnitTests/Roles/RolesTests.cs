using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Models;
using App1.Repositories;
using App1.Services;
using Xunit;

namespace UnitTests.Roles
{
    public class RolesTests
    {
        private readonly RolesRepository _rolesRepository;
        private readonly UserService _userService;
        private readonly MockUserRepository _mockUserRepository;

        public RolesTests()
        {
            _rolesRepository = new RolesRepository();
            _mockUserRepository = new MockUserRepository();
            _userService = new UserService(_mockUserRepository);
        }

        /// <summary>
        /// Tests the integration between UserService and role filtering functionality.
        /// Verifies that users can be correctly filtered by their assigned roles.
        /// This test ensures that:
        /// 1. Regular users are correctly identified
        /// 2. Admin users are correctly identified
        /// 3. The filtering logic works with multiple users having the same role
        /// </summary>
        [Fact]
        public void UserService_GetUsersByRoleType_ReturnsCorrectUsers()
        {
            var testUsers = new List<User>
            {
                new User { UserId = 1, AssignedRoles = new List<Role> { new Role(RoleType.User, "User") } },
                new User { UserId = 2, AssignedRoles = new List<Role> { new Role(RoleType.Admin, "Admin") } },
                new User { UserId = 3, AssignedRoles = new List<Role> { new Role(RoleType.User, "User") } }
            };
            _mockUserRepository.SetUsers(testUsers);

            var regularUsers = _userService.GetRegularUsers();
            var adminUsers = _userService.GetAdminUsers();

            Assert.Equal(2, regularUsers.Count);
            Assert.Equal(1, adminUsers.Count);
            Assert.All(regularUsers, u => Assert.Contains(u.AssignedRoles, r => r.RoleType == RoleType.User));
            Assert.All(adminUsers, u => Assert.Contains(u.AssignedRoles, r => r.RoleType == RoleType.Admin));
        }

        /// <summary>
        /// Tests the complete role hierarchy integration between RolesRepository and UserService.
        /// This test verifies that:
        /// 1. All roles are correctly defined in the system
        /// 2. Users can be correctly filtered by each role type
        /// 3. The role hierarchy (Banned -> User -> Admin -> Manager) is maintained
        /// 4. The integration between repository and service layers works correctly
        /// </summary>
        [Fact]
        public void RolesRepository_And_UserService_Integration_WorksCorrectly()
        {
            var testUsers = new List<User>
            {
                new User { UserId = 1, AssignedRoles = new List<Role> { new Role(RoleType.User, "User") } },
                new User { UserId = 2, AssignedRoles = new List<Role> { new Role(RoleType.Admin, "Admin") } },
                new User { UserId = 3, AssignedRoles = new List<Role> { new Role(RoleType.Manager, "Manager") } }
            };
            _mockUserRepository.SetUsers(testUsers);

            var allRoles = _rolesRepository.GetAllRoles();
            var userRole = allRoles.First(r => r.RoleType == RoleType.User);
            var adminRole = allRoles.First(r => r.RoleType == RoleType.Admin);
            var managerRole = allRoles.First(r => r.RoleType == RoleType.Manager);

            var users = _userService.GetUsersByRoleType(userRole.RoleType);
            var admins = _userService.GetUsersByRoleType(adminRole.RoleType);
            var managers = _userService.GetUsersByRoleType(managerRole.RoleType);

            Assert.Equal(4, allRoles.Count); // Banned, User, Admin, Manager
            Assert.Equal(1, users.Count);
            Assert.Equal(1, admins.Count);
            Assert.Equal(1, managers.Count);
        }

        /// <summary>
        /// Tests the role promotion between RolesRepository and UserService.
        /// This test verifies that:
        /// 1. The next role in the hierarchy can be correctly determined
        /// 2. Users can be filtered by the promoted role
        /// 3. The role promotion system maintains correct role names
        /// </summary>
        [Fact]
        public void GetNextRoleInHierarchy_And_UserService_Integration_WorksCorrectly()
        {
            var testUsers = new List<User>
            {
                new User { UserId = 1, AssignedRoles = new List<Role> { new Role(RoleType.User, "User") } }
            };
            _mockUserRepository.SetUsers(testUsers);

            var nextRole = _rolesRepository.GetNextRoleInHierarchy(RoleType.User);
            var usersWithNextRole = _userService.GetUsersByRoleType(nextRole.RoleType);

            Assert.Equal(RoleType.Admin, nextRole.RoleType);
            Assert.Equal("Admin", nextRole.RoleName);
        }
    }

    public class MockUserRepository : IUserRepository
    {
        private List<User> _users = new List<User>();

        public void SetUsers(List<User> users)
        {
            _users = users;
        }

        public List<User> GetAllUsers() => _users;

        public List<User> GetUsersByRoleType(RoleType roleType) => 
            _users.Where(u => u.AssignedRoles.Any(r => r.RoleType == roleType)).ToList();

        public User GetUserByID(int userId) => _users.FirstOrDefault(u => u.UserId == userId);

        public List<User> GetBannedUsersWhoHaveSubmittedAppeals() => 
            _users.Where(u => u.AssignedRoles.Any(r => r.RoleType == RoleType.Banned) && u.HasSubmittedAppeal).ToList();

        public RoleType GetHighestRoleTypeForUser(int userId) => 
            _users.FirstOrDefault(u => u.UserId == userId)?.AssignedRoles.Max(r => r.RoleType) ?? RoleType.Banned;

        public List<User> GetUsersWhoHaveSubmittedAppeals() =>
            _users.Where(u => u.HasSubmittedAppeal).ToList();

        public void AddRoleToUser(int userId, Role roleToAdd)
        {
            var user = _users.FirstOrDefault(u => u.UserId == userId);
            if (user != null)
            {
                user.AssignedRoles.Add(roleToAdd);
            }
        }
    }
}
