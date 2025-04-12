using System;
using System.Collections.Generic;
using App1.Models;
using App1.Repositories;

namespace App1.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public List<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
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
            catch (Exception ex)
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
            catch (Exception ex)
            {
                throw new UserServiceException("Failed to get banned users", ex);
            }
        }

        public List<User> GetUsersByRoleType(RoleType roleType)
        {
            return _userRepository.GetUsersByRoleType(roleType);
        }

        public string GetUserFullNameById(int userId)
        {
            return _userRepository.GetUserByID(userId).FullName;
        }

        public List<User> GetBannedUsersWhoHaveSubmittedAppeals()
        {
            return _userRepository.GetBannedUsersWhoHaveSubmittedAppeals();
        }

        public User GetUserById(int userId)
        {
            return _userRepository.GetUserByID(userId);
        }

        public RoleType GetHighestRoleTypeForUser(int userId)
        {
            return this._userRepository.GetHighestRoleTypeForUser(userId);
        }

        public List<User> GetAdminUsers()
        {
            return _userRepository.GetUsersByRoleType(RoleType.Admin);
        }

        public List<User> GetRegularUsers()
        {
            return _userRepository.GetUsersByRoleType(RoleType.User);
        }

        public List<User> GetManagers()
        {
            return _userRepository.GetUsersByRoleType(RoleType.Manager);
        }
    }


    public class UserServiceException : Exception
    {
        public UserServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}