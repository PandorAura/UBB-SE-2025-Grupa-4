using System;
using System.Collections.Generic;
using App1.Models;
using App1.Repositories;

namespace App1.Services
{
    public class UserService: IUserService
    {
        private readonly IUserRepository _userRepo;
        private const int BANNED_PERMISSION_ID = 0;

        public UserService(IUserRepository userRepository)
        {
            _userRepo = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }

        public List<User> GetActiveUsers(int permissionId)
        {
            try
            {
                return permissionId switch
                {
                    > 0 => _userRepo.GetUsersByRole(permissionId),
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
                return _userRepo.GetUsersByRole(BANNED_PERMISSION_ID);
            }
            catch (Exception ex)
            {
                throw new UserServiceException("Failed to get banned users", ex);
            }
        }

        public List<User> GetUsersByPermission(int permissionId)
        {
            return  _userRepo.GetUsersByRole(permissionId); 
        }
    
        public string GetUserName(int ID) { 
            return _userRepo.getUserByID(ID).Name;
        }

        public List<User> GetAppealingUsers()
        {
            return _userRepo.GetAppealingUsers();
        }

        public User GetUserBasedOnID(int ID)
        {
            return _userRepo.getUserByID(ID);
        }

        public int GetHighestRoleBasedOnUserID(int ID)
        {
            return this._userRepo.getHighestRoleIdBasedOnUserId(ID);
        }
    }


    public class UserServiceException : Exception
    {
        public UserServiceException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}