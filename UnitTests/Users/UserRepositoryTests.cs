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
        public void Constructor_ShouldInitializeUsersListCorrectly()
        {
            // Arrange
            var userRepo = new UserRepo();

            // Act
            var users = userRepo.GetAllUsers();

            // Assert
            Assert.NotNull(users);
            Assert.Equal(3, users.Count); // Verify the number of users

            // Verify the first user
            var user1 = users.FirstOrDefault(u => u.UserId == 1);
            Assert.NotNull(user1);
            Assert.Equal("bianca.georgiana.cirnu@gmail.com", user1.EmailAddress);
            Assert.Equal("Bianca Georgiana Cirnu", user1.FullName);
            Assert.Equal(2, user1.NumberOfDeletedReviews);
            Assert.True(user1.HasSubmittedAppeal);
            Assert.Single(user1.AssignedRoles);
            Assert.Contains(user1.AssignedRoles, role => role.RoleType == RoleType.User);

            // Verify the second user
            var user2 = users.FirstOrDefault(u => u.UserId == 3);
            Assert.NotNull(user2);
            Assert.Equal("admin.one@example.com", user2.EmailAddress);
            Assert.Equal("Admin One", user2.FullName);
            Assert.Equal(0, user2.NumberOfDeletedReviews);
            Assert.False(user2.HasSubmittedAppeal);
            Assert.Equal(2, user2.AssignedRoles.Count);
            Assert.Contains(user2.AssignedRoles, role => role.RoleType == RoleType.Admin);
            Assert.Contains(user2.AssignedRoles, role => role.RoleType == RoleType.User);

            // Verify the third user
            var user3 = users.FirstOrDefault(u => u.UserId == 5);
            Assert.NotNull(user3);
            Assert.Equal("admin.two@example.com", user3.EmailAddress);
            Assert.Equal("Admin Two", user3.FullName);
            Assert.Equal(0, user3.NumberOfDeletedReviews);
            Assert.False(user3.HasSubmittedAppeal);
            Assert.Equal(2, user3.AssignedRoles.Count);
            Assert.Contains(user3.AssignedRoles, role => role.RoleType == RoleType.Admin);
            Assert.Contains(user3.AssignedRoles, role => role.RoleType == RoleType.User);
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
        public void GetUsersWhoHaveSubmittedAppeals_ShouldReturnEmptyList_WhenNoUsersHaveSubmittedAppeals()
        {
            var userRepo = new UserRepo();
            userRepo.GetAllUsers().ForEach(user => user.HasSubmittedAppeal = false);

            var users = userRepo.GetUsersWhoHaveSubmittedAppeals();

            Assert.NotNull(users);
            Assert.Empty(users);
        }

        [Fact]
        public void GetUsersWhoHaveSubmittedAppeals_ShouldThrowRepositoryException_WhenExceptionOccurs()
        {
            // Arrange
            var userRepo = new UserRepo();
            var usersListField = typeof(UserRepo).GetField("_usersList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            usersListField.SetValue(userRepo, null); // Simulate a null list

            // Act & Assert
            var repoException = Assert.Throws<UserRepo.RepositoryException>(() => userRepo.GetUsersWhoHaveSubmittedAppeals());
            Assert.Equal("Failed to retrieve users who have submitted appeals.", repoException.Message);
            Assert.IsType<NullReferenceException>(repoException.InnerException);
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
        public void GetUsersByRoleType_ShouldReturnEmptyList_WhenNoUsersHaveRole()
        {
            var userRepo = new UserRepo();

            var bannedUsers = userRepo.GetUsersByRoleType(RoleType.Banned);

            Assert.NotNull(bannedUsers);
            Assert.Empty(bannedUsers);
        }

        [Fact]
        public void GetUsersByRoleType_ShouldReturnCorrectUsers_ForManagerRole()
        {
            var userRepo = new UserRepo();

            var managerUsers = userRepo.GetUsersByRoleType(RoleType.Manager);

            Assert.NotNull(managerUsers);
            Assert.All(managerUsers, user => Assert.Contains(user.AssignedRoles, role => role.RoleType == RoleType.Manager));
        }

        [Fact]
        public void GetUsersByRoleType_ShouldThrowRepositoryException_WhenExceptionOccurs()
        {
            // Arrange
            var userRepo = new UserRepo();
            var usersListField = typeof(UserRepo).GetField("_usersList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            usersListField.SetValue(userRepo, null); // Simulate a null list

            // Act & Assert
            var repoException = Assert.Throws<UserRepo.RepositoryException>(() => userRepo.GetUsersByRoleType(RoleType.Admin));
            Assert.Equal("Failed to retrieve users with role type 'Admin'.", repoException.Message);
            Assert.IsType<NullReferenceException>(repoException.InnerException);
        }

        [Fact]
        public void GetUsersByRoleType_ShouldSkipUsers_WhenAssignedRolesIsNull()
        {
            var userRepo = new UserRepo();
            var user = new User
            {
                UserId = 6,
                AssignedRoles = null 
            };

            var usersListField = typeof(UserRepo).GetField("_usersList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var usersList = (List<User>)usersListField.GetValue(userRepo);
            usersList.Add(user);

            var result = userRepo.GetUsersByRoleType(RoleType.Admin);

            Assert.NotNull(result);
            Assert.DoesNotContain(result, u => u.UserId == 6);
        }

        [Fact]
        public void GetUsersByRoleType_ShouldSkipUsers_WhenRoleTypeDoesNotMatch()
        {
            var userRepo = new UserRepo();
            var user = new User
            {
                UserId = 7,
                AssignedRoles = new List<Role> { new Role(RoleType.User, "User") } 
            };

            var usersListField = typeof(UserRepo).GetField("_usersList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var usersList = (List<User>)usersListField.GetValue(userRepo);
            usersList.Add(user);

            var result = userRepo.GetUsersByRoleType(RoleType.Admin);

            Assert.NotNull(result);
            Assert.DoesNotContain(result, u => u.UserId == 7);
        }


        [Fact]
        public void GetHighestRoleTypeForUser_ShouldReturnCorrectRoleType()
        {
            var highestRole = _userRepo.GetHighestRoleTypeForUser(3); 

            Assert.Equal(RoleType.Admin, highestRole);
        }

        [Fact]
        public void GetHighestRoleTypeForUser_ShouldThrowException_WhenUserHasNoRoles()
        {
            var userRepo = new UserRepo();
            var user = new User
            {
                UserId = 4,
                AssignedRoles = new List<Role>() // No roles assigned
            };
            userRepo.GetAllUsers().Add(user);

            var exception = Assert.Throws<UserRepo.RepositoryException>(() => userRepo.GetHighestRoleTypeForUser(4));
            Assert.Equal("User has no roles assigned.", exception.Message);
        }


        [Fact]
        public void GetHighestRoleTypeForUser_ShouldThrowException_WhenUserDoesNotExist()
        {
            var exception = Assert.Throws<UserRepo.RepositoryException>(() => _userRepo.GetHighestRoleTypeForUser(999));
            Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Equal("No user found with ID 999", exception.InnerException.Message);
        }

        [Fact]
        public void GetHighestRoleTypeForUser_ShouldThrowException_WhenAssignedRolesIsNull()
        {
            var userRepo = new UserRepo();
            var user = new User
            {
                UserId = 4,
                AssignedRoles = null 
            };
            userRepo.GetAllUsers().Add(user);

            var exception = Assert.Throws<UserRepo.RepositoryException>(() => userRepo.GetHighestRoleTypeForUser(4));
            Assert.Equal("User has no roles assigned.", exception.Message);
            Assert.IsType<ArgumentException>(exception.InnerException);
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
        public void GetBannedUsersWhoHaveSubmittedAppeals_ShouldThrowRepositoryException_WhenExceptionOccurs()
        {
            // Arrange
            var userRepo = new UserRepo();
            var usersListField = typeof(UserRepo).GetField("_usersList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            usersListField.SetValue(userRepo, null); // Simulate a null list

            // Act & Assert
            var repoException = Assert.Throws<UserRepo.RepositoryException>(() => userRepo.GetBannedUsersWhoHaveSubmittedAppeals());
            Assert.Equal("Failed to retrieve banned users who have submitted appeals.", repoException.Message);
            Assert.IsType<NullReferenceException>(repoException.InnerException);
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

        [Fact]
        public void GetAllUsers_ShouldThrowRepositoryException_WhenExceptionOccurs()
        {
            // Arrange
            var userRepo = new UserRepo();
            var usersListField = typeof(UserRepo).GetField("_usersList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            usersListField.SetValue(userRepo, null); // Simulate a null list

            // Act & Assert
            var repoException = Assert.Throws<UserRepo.RepositoryException>(() => userRepo.GetAllUsers());
            Assert.Equal("Failed to retrieve all users.", repoException.Message);
            Assert.IsType<NullReferenceException>(repoException.InnerException);
        }


    }
}
