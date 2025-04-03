using System.Collections.Generic;
using System.Data;
using System.Linq;
using App1.Models;
//using Windows.System;

namespace App1.Repositories
{
    internal class UserRepo
    {
        private List<User> users;

        public UserRepo()
        {
            users = new List<User>();
        }

        public void UpdatePermission(int userID, int permissionID)
        {
            var user = users.FirstOrDefault(u => u.userId == userID);
            if (user != null)
            {
                user.permissionID = permissionID;
            }
        }

        public List<User> GetAppealedUsers()
        {
            return users.Where(u => u.hasAppealed).ToList();
        }

        public List<User> GetUsersByPermission(int permissionID)
        {
            return users.Where(u => u.permissionID == permissionID).ToList();
        }

        public int getHighestRoleIdBasedOnUserId(int userId)
        {
            User user = users.First(user => user.userId == userId);

            List<Role> roles = user.roles;

            Role maxRole = roles.MaxBy(role => role.RoleId);

            int maxId = maxRole.RoleId;

            return maxId;
        }

        public void addRoleToUser(int userID, Role roleToAdd)
        {
            User user = users.First(user => user.userId == userID);
            user.roles.Add(roleToAdd);
        }
    }
}
