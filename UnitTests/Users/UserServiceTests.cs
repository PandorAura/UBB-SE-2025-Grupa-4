using App1.Models;
using App1.Repositories;
using App1.Services;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;
using static App1.Repositories.UserRepo;

namespace UnitTests.Users
{
    /// <summary>
    /// Unit tests for the <see cref="UserService"/> class.
    /// </summary>
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        /// <summary>
        /// Initializes a mock of the <see cref="UserRepo"/> and a new instance of the <see cref="UserServiceTests"/> class.
        /// </summary>
        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

        /// <summary>
        /// Verifies that the <see cref="UserService"/> constructor initializes correctly when a valid <see cref="IUserRepository"/> is provided.
        /// </summary>
        [Fact]
        public void Constructor_ShouldInitialize_WhenUserRepositoryIsValid()
        {
            var mockUserRepository = new Mock<IUserRepository>();

            var userService = new UserService(mockUserRepository.Object);

            Assert.NotNull(userService);
        }

        /// <summary>
        /// Verifies that the <see cref="UserService"/> constructor throws an <see cref="ArgumentNullException"/> when the <see cref="IUserRepository"/> is null.
        /// </summary>
        [Fact]
        public void Constructor_ShouldThrowArgumentNullException_WhenUserRepositoryIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new UserService(null));
            Assert.Equal("Value cannot be null. (Parameter 'userRepository')", exception.Message);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetAllUsers"/> retrieves all users successfully.
        /// </summary>
        [Fact]
        public void GetAllUsers_ShouldReturnAllUsers()
        {
            var users = new List<User>
            {
                new User { UserId = 1, FullName = "User One" },
                new User { UserId = 2, FullName = "User Two" }
            };
            _mockUserRepository.Setup(repo => repo.GetAllUsers()).Returns(users);

            
            var result = _userService.GetAllUsers();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("User One", result[0].FullName);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetAllUsers"/> throws a <see cref="UserServiceException"/> when the repository throws an exception.
        /// </summary>
        [Fact]
        public void GetAllUsers_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetAllUsers()).Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetAllUsers());
            Assert.Equal("Failed to retrieve all users.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetAllUsers"/> returns an empty list when the repository returns no users.
        /// </summary>
        [Fact]
        public void GetAllUsers_ShouldReturnEmptyList_WhenRepositoryReturnsNoUsers()
        {
            _mockUserRepository.Setup(repo => repo.GetAllUsers()).Returns(new List<User>());

            var result = _userService.GetAllUsers();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetActiveUsersByRoleType"/> retrieves active users by role type successfully.
        /// </summary>
        [Fact]
        public void GetActiveUsersByRoleType_ShouldReturnCorrectUsers()
        {
            var users = new List<User>
            {
                new User { UserId = 1, FullName = "Active User" }
            };
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.User)).Returns(users);

            var result = _userService.GetActiveUsersByRoleType(RoleType.User);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Active User", result[0].FullName);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetActiveUsersByRoleType"/> throws a <see cref="UserServiceException"/> when the repository throws an exception.
        /// </summary>
        [Fact]
        public void GetActiveUsersByRoleType_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.User)).Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetActiveUsersByRoleType(RoleType.User));
            Assert.Equal("Failed to get active users", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetActiveUsersByRoleType"/> throws an <see cref="ArgumentException"/> when the role type is invalid.
        /// </summary>
        [Fact]
        public void GetActiveUsersByRoleType_ShouldThrowArgumentException_WhenRoleTypeIsInvalid()
        {
            var exception = Assert.Throws<ArgumentException>(() => _userService.GetActiveUsersByRoleType(0));
            Assert.Equal("Permission ID must be positive", exception.Message);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetUserById"/> retrieves the correct user by ID.
        /// </summary>
        [Fact]
        public void GetUserById_ShouldReturnCorrectUser()
        {
            var user = new User { UserId = 1, FullName = "User One" };
            _mockUserRepository.Setup(repo => repo.GetUserByID(1)).Returns(user);

            var result = _userService.GetUserById(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.Equal("User One", result.FullName);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetUserById"/> throws a <see cref="UserServiceException"/> when the repository throws an exception.
        /// </summary>
        [Fact]
        public void GetUserById_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetUserByID(1)).Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetUserById(1));
            Assert.Equal("Failed to retrieve user with ID 1.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetUserById"/> throws a <see cref="UserServiceException"/> when the repository returns null.
        /// </summary>
        [Fact]
        public void GetUserById_ShouldThrowUserServiceException_WhenRepositoryReturnsNull()
        {
            _mockUserRepository.Setup(repo => repo.GetUserByID(1)).Returns((User)null);

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetUserById(1));
            Assert.Equal("Failed to retrieve user with ID 1.", exception.Message);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetBannedUsers"/> retrieves all banned users successfully.
        /// </summary>
        [Fact]
        public void GetBannedUsers_ShouldReturnBannedUsers()
        {
            var users = new List<User>
            {
                new User { UserId = 1, FullName = "Banned User" }
            };
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.Banned)).Returns(users);

            var result = _userService.GetBannedUsers();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Banned User", result[0].FullName);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetBannedUsers"/> throws a <see cref="UserServiceException"/> when the repository throws an exception.
        /// </summary>
        [Fact]
        public void GetBannedUsers_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.Banned)).Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetBannedUsers());
            Assert.Equal("Failed to get banned users", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetBannedUsers"/> returns an empty list when the repository returns no users.
        /// </summary>

        [Fact]
        public void GetBannedUsers_ShouldReturnEmptyList_WhenRepositoryReturnsNoUsers()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.Banned)).Returns(new List<User>());

            var result = _userService.GetBannedUsers();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetHighestRoleTypeForUser"/> retrieves the correct highest role type for a user.
        /// </summary>
        [Fact]
        public void GetHighestRoleTypeForUser_ShouldReturnCorrectRoleType()
        {
            _mockUserRepository.Setup(repo => repo.GetHighestRoleTypeForUser(1)).Returns(RoleType.Admin);

            var result = _userService.GetHighestRoleTypeForUser(1);

            Assert.Equal(RoleType.Admin, result);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetHighestRoleTypeForUser"/> throws a <see cref="UserServiceException"/> when the repository throws an exception.
        /// </summary>
        [Fact]
        public void GetHighestRoleTypeForUser_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetHighestRoleTypeForUser(1)).Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetHighestRoleTypeForUser(1));
            Assert.Equal("Failed to retrieve the highest role type for user with ID 1.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetHighestRoleTypeForUser"/> returns the default role type when the repository returns a default value.
        /// </summary>
        [Fact]
        public void GetHighestRoleTypeForUser_ShouldReturnDefaultRole_WhenRepositoryReturnsDefault()
        {
            _mockUserRepository.Setup(repo => repo.GetHighestRoleTypeForUser(1)).Returns(RoleType.Banned);

            var result = _userService.GetHighestRoleTypeForUser(1);

            Assert.Equal(RoleType.Banned, result);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetUsersByRoleType"/> retrieves users by role type successfully.
        /// </summary>
        [Fact]
        public void GetUsersByRoleType_ShouldReturnCorrectUsers()
        {
            var users = new List<User>
            {
                new User { UserId = 1, FullName = "User One" }
            };
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.User)).Returns(users);

            var result = _userService.GetUsersByRoleType(RoleType.User);

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("User One", result[0].FullName);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetUsersByRoleType"/> throws a <see cref="UserServiceException"/> when the repository throws an exception.
        /// </summary>
        [Fact]
        public void GetUsersByRoleType_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.User))
                .Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetUsersByRoleType(RoleType.User));
            Assert.Equal("Failed to retrieve users by role type 'User'.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetUsersByRoleType"/> returns an empty list when the repository returns no users.
        /// </summary>
        [Fact]
        public void GetUsersByRoleType_ShouldReturnEmptyList_WhenRepositoryReturnsNoUsers()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.User)).Returns(new List<User>());

            var result = _userService.GetUsersByRoleType(RoleType.User);

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetAdminUsers"/> retrieves all admin users successfully.
        /// </summary>
        [Fact]
        public void GetAdminUsers_ShouldReturnCorrectUsers()
        {
            var users = new List<User>
            {
                new User { UserId = 1, FullName = "Admin One"}
            };
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.Admin)).Returns(users);

            var result = _userService.GetAdminUsers();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Admin One", result[0].FullName);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetAdminUsers"/> throws a <see cref="UserServiceException"/> when the repository throws an exception.
        /// </summary>
        [Fact]
        public void GetAdminUsers_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.Admin)).
                Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetAdminUsers());
            Assert.Equal("Failed to retrieve admin users.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetRegularUsers"/> retrieves all regular users successfully.
        /// </summary>
        [Fact]
        public void GetRegularUsers_ShouldReturnCorrectUsers()
        {
            var users = new List<User>
            {
                new User { UserId = 1, FullName = "Regular User" }
            };
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.User)).Returns(users);

            var result = _userService.GetRegularUsers();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Regular User", result[0].FullName);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetRegularUsers"/> throws a <see cref="UserServiceException"/> when the repository throws an exception.
        /// </summary>
        [Fact]
        public void GetRegularUsers_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.User))
                .Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetRegularUsers());
            Assert.Equal("Failed to retrieve regular users.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetManagers"/> retrieves all manager users successfully.
        /// </summary>
        [Fact]
        public void GetManagers_ShouldReturnCorrectUsers()
        {
            var users = new List<User>
            {
                new User { UserId = 1, FullName = "Manager User" }
            };
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.Manager)).Returns(users);

            var result = _userService.GetManagers();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Manager User", result[0].FullName);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetManagers"/> throws a <see cref="UserServiceException"/> when the repository throws an exception.
        /// </summary>
        [Fact]
        public void GetManagers_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.Manager))
                .Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetManagers());
            Assert.Equal("Failed to retrieve manager users.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetBannedUsersWhoHaveSubmittedAppeals"/> retrieves all banned users who have submitted appeals successfully.
        /// </summary>
        [Fact]
        public void GetBannedUsersWhoHaveSubmittedAppeals_ShouldReturnCorrectUsers()
        {
            var users = new List<User>
            {
                new User { UserId = 1, FullName = "Banned User", HasSubmittedAppeal = true }
            };
            _mockUserRepository.Setup(repo => repo.GetBannedUsersWhoHaveSubmittedAppeals()).Returns(users);

            var result = _userService.GetBannedUsersWhoHaveSubmittedAppeals();

            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Banned User", result[0].FullName);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetBannedUsersWhoHaveSubmittedAppeals"/> returns an empty list when the repository returns no users.
        /// </summary>
        [Fact]
        public void GetBannedUsersWhoHaveSubmittedAppeals_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetBannedUsersWhoHaveSubmittedAppeals())
                .Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetBannedUsersWhoHaveSubmittedAppeals());
            Assert.Equal("Failed to retrieve banned users who have submitted appeals.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetBannedUsersWhoHaveSubmittedAppeals"/> returns an empty list when the repository returns no users.
        /// </summary>
        [Fact]
        public void GetBannedUsersWhoHaveSubmittedAppeals_ShouldReturnEmptyList_WhenRepositoryReturnsNoUsers()
        {
            _mockUserRepository.Setup(repo => repo.GetBannedUsersWhoHaveSubmittedAppeals()).Returns(new List<User>());

            var result = _userService.GetBannedUsersWhoHaveSubmittedAppeals();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetUserFullNameById"/> retrieves the correct full name of a user by ID.
        /// </summary>
        [Fact]
        public void GetUserFullNameById_ShouldReturnCorrectFullName()
        { 
            var user = new User { UserId = 1, FullName = "User One" };
            _mockUserRepository.Setup(repo => repo.GetUserByID(1)).Returns(user);

            var result = _userService.GetUserFullNameById(1);

            Assert.Equal("User One", result);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetUserFullNameById"/> throws a <see cref="UserServiceException"/> when the repository throws an exception.
        /// </summary>
        [Fact]
        public void GetUserFullNameById_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetUserByID(1))
                .Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetUserFullNameById(1));
            Assert.Equal("Failed to retrieve the full name of the user with ID 1.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.GetUserFullNameById"/> throws a <see cref="UserServiceException"/> when the repository returns null.
        /// </summary>
        [Fact]
        public void GetUserFullNameById_ShouldThrowUserServiceException_WhenRepositoryReturnsNull()
        {
            _mockUserRepository.Setup(repo => repo.GetUserByID(1)).Returns((User)null);

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetUserFullNameById(1));
            Assert.Equal("Failed to retrieve the full name of the user with ID 1.", exception.Message);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.UpdateUserRole"/> does nothing when the user does not exist.
        /// </summary>
        [Fact]
        public void UpdateUserRole_ShouldDoNothing_WhenUserDoesNotExist()
        {
            _mockUserRepository.Setup(repo => repo.GetUserByID(1)).Returns((User)null);

            _userService.UpdateUserRole(1, RoleType.Banned);

            _mockUserRepository.Verify(repo => repo.AddRoleToUser(It.IsAny<int>(), It.IsAny<Role>()), Times.Never);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.UpdateUserRole"/> does not add the banned role when the user already has it.
        /// </summary>
        [Fact]
        public void UpdateUserRole_ShouldNotAddBannedRole_WhenUserAlreadyHasBannedRole()
        {
            var user = new User
            {
                UserId = 1,
                AssignedRoles = new List<Role> { new Role(RoleType.Banned, "Banned") }
            };
            _mockUserRepository.Setup(repo => repo.GetUserByID(1)).Returns(user);

            _userService.UpdateUserRole(1, RoleType.Banned);

            Assert.Single(user.AssignedRoles);
            Assert.Equal(RoleType.Banned, user.AssignedRoles[0].RoleType);
            _mockUserRepository.Verify(repo => repo.AddRoleToUser(It.IsAny<int>(), It.IsAny<Role>()), Times.Never);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.UpdateUserRole"/> sets the role to banned when the user does not already have it.
        /// </summary>
        [Fact]
        public void UpdateUserRole_ShouldSetRoleToBanned_WhenUserDoesNotHaveBannedRole()
        {
            var user = new User
            {
                UserId = 1,
                AssignedRoles = new List<Role> { new Role(RoleType.User, "User") }
            };
            _mockUserRepository.Setup(repo => repo.GetUserByID(1)).Returns(user);

            _userService.UpdateUserRole(1, RoleType.Banned);

            _mockUserRepository.Verify(repo => repo.AddRoleToUser(1, It.Is<Role>(r => r.RoleType == RoleType.Banned && r.RoleName == "Banned")), Times.Once);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.UpdateUserRole"/> sets the role to user when the role type is user.
        /// </summary>
        [Fact]
        public void UpdateUserRole_ShouldSetRoleToUser_WhenRoleTypeIsUser()
        {
            var user = new User
            {
                UserId = 1,
                AssignedRoles = new List<Role> { new Role(RoleType.Banned, "Banned") }
            };
            _mockUserRepository.Setup(repo => repo.GetUserByID(1)).Returns(user);

            _userService.UpdateUserRole(1, RoleType.User);

            _mockUserRepository.Verify(repo => repo.AddRoleToUser(1, It.Is<Role>(r => r.RoleType == RoleType.User && r.RoleName == "User")), Times.Once);
        }

        /// <summary>
        /// Verifies that <see cref="UserService.UpdateUserRole"/> throws a <see cref="UserServiceException"/> when the repository throws an exception.
        /// </summary>
        [Fact]
        public void UpdateUserRole_ShouldThrowUserServiceException_WhenRepositoryThrowsException()
        {
            _mockUserRepository.Setup(repo => repo.GetUserByID(1)).Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.UpdateUserRole(1, RoleType.Banned));
            Assert.Equal("Failed to update user role", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

    }
}
