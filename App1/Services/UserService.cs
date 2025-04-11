using System;
using System.Collections.Generic;
using App1.Models;
using App1.Repositories;
using static App1.Repositories.UserRepo;

namespace App1.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public List<User> GetAllUsers()
        {
            try
            {
                return _userRepository.GetAllUsers();
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to retrieve all users.", ex);
            }
        }

        public List<User> GetActiveUsersByRoleType(RoleType roleType)
        {
            try
            {
                return roleType switch
                {
                    > 0 => _userRepository.GetUsersByRoleType(roleType),
                    _ => throw new ArgumentException("Permission ID must be positive")
                };
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to get active users", ex);
            }
        }

        public List<User> GetBannedUsers()
        {
            try
            {
                return _userRepository.GetUsersByRoleType(RoleType.Banned);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to get banned users", ex);
            }
        }

        public List<User> GetUsersByRoleType(RoleType roleType)
        {
            try
            {
                return _userRepository.GetUsersByRoleType(roleType);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException($"Failed to retrieve users by role type '{roleType}'.", ex);
            }
        }
    
        public string GetUserFullNameById(int userId) 
        {
            try
            {
                return _userRepository.GetUserByID(userId).FullName;
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException($"Failed to retrieve the full name of the user with ID {userId}.", ex);
            }
        }

        public List<User> GetBannedUsersWhoHaveSubmittedAppeals()
        {
            try
            {
                return _userRepository.GetBannedUsersWhoHaveSubmittedAppeals();
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to retrieve banned users who have submitted appeals.", ex);
            }
        }

        public User GetUserById(int userId)
        {
            try
            {
                return _userRepository.GetUserByID(userId);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException($"Failed to retrieve user with ID {userId}.", ex);
            }
        }

        public RoleType GetHighestRoleTypeForUser(int userId)
        {
            try
            {
                return _userRepository.GetHighestRoleTypeForUser(userId);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException($"Failed to retrieve the highest role type for user with ID {userId}.", ex);
            }
        }

        public List<User> GetAdminUsers()
        {
            try
            {
                return _userRepository.GetUsersByRoleType(RoleType.Admin);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to retrieve admin users.", ex);
            }
        }

        public List<User> GetRegularUsers()
        {
            try
            {
                return _userRepository.GetUsersByRoleType(RoleType.User);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to retrieve regular users.", ex);
            }
        }

        public List<User> GetManagers()
        {
            try
            {
                return _userRepository.GetUsersByRoleType(RoleType.Manager);
            }
            catch (RepositoryException ex)
            {
                throw new UserServiceException("Failed to retrieve manager users.", ex);
            }
        }
    }


    public class UserServiceException : Exception
    {
        public UserServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}