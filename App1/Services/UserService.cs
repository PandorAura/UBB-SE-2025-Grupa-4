using System.Collections.Generic;
using App1.Models;
using App1.Repositories;

namespace App1.Services
{
    internal class UserService
    {
        private readonly UserRepo userRepo;

        public UserService()
        {
            userRepo = new UserRepo();
        }

        public void ChangeUserPermission(int userID, int permissionID)
        {
            userRepo.UpdatePermission(userID, permissionID);
        }

        public List<User> GetActiveUsers(int permissionID)
        {
            return userRepo.GetUsersByPermission(permissionID);
        }

        public List<User> GetBannedUsers()
        {   //aici sa puneti voi cat inseamna banned
            return userRepo.GetUsersByPermission(-1);
        }
    }
}

