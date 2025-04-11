using App1.Models;
using App1.Repositories;
using System;
using System.Collections.Generic;
using Xunit;

namespace UnitTests.Repositories
{
    public class UserRepoTests
    {
        private readonly UserRepo _userRepo;

        public UserRepoTests()
        {
            _userRepo = new UserRepo();
        }

        [Fact]
        public void GetUsersWhoHaveSubmittedAppeals_ShouldReturnCorrectUsers()
        {
            var users = _userRepo.GetUsersWhoHaveSubmittedAppeals();

            Assert.NotNull(users);
            Assert.Single(users); 
            Assert.True(users[0].HasSubmittedAppeal);
        }

        [Fact]
        public void GetUsersByRoleType_ShouldReturnCorrectUsers()
        {
            var adminUsers = _userRepo.GetUsersByRoleType(RoleType.Admin);

            Assert.NotNull(adminUsers);
            Assert.Equal(2, adminUsers.Count); 
            Assert.All(adminUsers, user => Assert.Contains(user.AssignedRoles, role => role.RoleType == RoleType.Admin));
        }

        [Fact]
        public void GetHighestRoleTypeForUser_ShouldReturnCorrectRoleType()
        {
            var highestRole = _userRepo.GetHighestRoleTypeForUser(3); 

            Assert.Equal(RoleType.Admin, highestRole);
        }

        [Fact]
        public void GetHighestRoleTypeForUser_ShouldThrowException_WhenUserDoesNotExist()
        {
            var exception = Assert.Throws<UserRepo.RepositoryException>(() => _userRepo.GetHighestRoleTypeForUser(999));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("No user found with ID 999", exception.InnerException.Message);
        }

        [Fact]
        public void GetUserByID_ShouldReturnCorrectUser()
        {
            var user = _userRepo.GetUserByID(1);

            Assert.NotNull(user);
            Assert.Equal(1, user.UserId);
            Assert.Equal("bianca.georgiana.cirnu@gmail.com", user.EmailAddress);
        }

        [Fact]
        public void GetUserByID_ShouldThrowException_WhenUserDoesNotExist()
        {
            var exception = Assert.Throws<UserRepo.RepositoryException>(() => _userRepo.GetUserByID(999));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("No user found with ID 999", exception.InnerException.Message);
        }

        [Fact]
        public void GetBannedUsersWhoHaveSubmittedAppeals_ShouldReturnCorrectUsers()
        {
            var bannedUsers = _userRepo.GetBannedUsersWhoHaveSubmittedAppeals();

            Assert.NotNull(bannedUsers);
            Assert.Empty(bannedUsers); 
        }

        [Fact]
        public void AddRoleToUser_ShouldAddRoleSuccessfully()
        {
            var newRole = new Role(RoleType.Manager, "Manager");

            _userRepo.AddRoleToUser(1, newRole);
            var user = _userRepo.GetUserByID(1);

            Assert.Contains(user.AssignedRoles, role => role.RoleType == RoleType.Manager);
        }

        [Fact]
        public void AddRoleToUser_ShouldThrowException_WhenUserDoesNotExist()
        {
            var newRole = new Role(RoleType.Manager, "Manager");

            var exception = Assert.Throws<UserRepo.RepositoryException>(() => _userRepo.AddRoleToUser(999, newRole));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("No user found with ID 999", exception.InnerException.Message);

        }

        [Fact]
        public void GetAllUsers_ShouldReturnAllUsers()
        {
            var users = _userRepo.GetAllUsers();

            Assert.NotNull(users);
            Assert.Equal(3, users.Count); // Default data contains 3 users
        }
    }
}
