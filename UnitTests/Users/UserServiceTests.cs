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
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _userService = new UserService(_mockUserRepository.Object);
        }

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

        [Fact]
        public void GetAllUsers_ShouldThrowUserServiceException_WhenRepositoryThrows()
        {
            _mockUserRepository.Setup(repo => repo.GetAllUsers()).Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetAllUsers());
            Assert.Equal("Failed to retrieve all users.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

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

        [Fact]
        public void GetActiveUsersByRoleType_ShouldThrowUserServiceException_WhenRepositoryThrows()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.User)).Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetActiveUsersByRoleType(RoleType.User));
            Assert.Equal("Failed to get active users", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

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

        [Fact]
        public void GetUserById_ShouldThrowUserServiceException_WhenRepositoryThrows()
        {
            _mockUserRepository.Setup(repo => repo.GetUserByID(1)).Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetUserById(1));
            Assert.Equal("Failed to retrieve user with ID 1.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

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

        [Fact]
        public void GetBannedUsers_ShouldThrowUserServiceException_WhenRepositoryThrows()
        {
            _mockUserRepository.Setup(repo => repo.GetUsersByRoleType(RoleType.Banned)).Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetBannedUsers());
            Assert.Equal("Failed to get banned users", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }

        [Fact]
        public void GetHighestRoleTypeForUser_ShouldReturnCorrectRoleType()
        {
            _mockUserRepository.Setup(repo => repo.GetHighestRoleTypeForUser(1)).Returns(RoleType.Admin);

            var result = _userService.GetHighestRoleTypeForUser(1);

            Assert.Equal(RoleType.Admin, result);
        }

        [Fact]
        public void GetHighestRoleTypeForUser_ShouldThrowUserServiceException_WhenRepositoryThrows()
        {
            _mockUserRepository.Setup(repo => repo.GetHighestRoleTypeForUser(1)).Throws(new RepositoryException("Repository error", new Exception("Inner exception")));

            var exception = Assert.Throws<UserServiceException>(() => _userService.GetHighestRoleTypeForUser(1));
            Assert.Equal("Failed to retrieve the highest role type for user with ID 1.", exception.Message);
            Assert.IsType<RepositoryException>(exception.InnerException);
        }
    }
}
