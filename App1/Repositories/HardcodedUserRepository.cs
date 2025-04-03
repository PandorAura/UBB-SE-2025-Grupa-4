// In UserRepo.cs
using System.Collections.Generic;
using System.Linq;
using App1.Models;
//using Windows.System;

namespace App1.Repositories
{
    public class HardcodedUserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public HardcodedUserRepository()
        {
            List<Role> roles1, roles2;
            roles1 = new List<Role>
            {
                new Role(
                    roleId: 1,
                    roleName: "User"
                )
            };
            roles2 = new List<Role>
            {
                new Role(
                    roleId: 1,
                    roleName: "user"
                ),
                new Role(
                    roleId: 2,
                    roleName: "admin"
                )
            };


            _users = new List<User>
            {
                new User(
                    userId: 1,
                    email: "mkhenike@gmail.com",
                    name: "user12",
                    numberOfDeletedReviews: 3,
                    permissionID: 2,
                    hasAppealed: false,
                    roles: roles1
                ),
                 new User(
                    userId: 2,
                    email: "aurapandor@gmail.com",
                    name: "user123",
                    numberOfDeletedReviews: 3,
                    permissionID: 2,
                    hasAppealed: false,
                    roles: roles2
                )
            };
        }

        public void UpdatePermission(int userID, int permissionID)
        {
            var user = _users.FirstOrDefault(u => u.userId == userID);
            if (user != null)
            {
                user.permissionID = permissionID;
            }
        }

        public List<User> GetAppealedUsers()
        {
            return _users.Where(u => u.hasAppealed).ToList();
        }

        public List<User> GetUsersByPermission(int permissionID)
        {
            return _users.Where(u => u.permissionID == permissionID).ToList();
        }
        public int getHighestRoleIdBasedOnUserId(int userId)
        {
            User user = _users.First(user => user.userId == userId);

            List<Role> roles = user.roles;

            Role maxRole = roles.MaxBy(role => role.RoleId);

            int maxId = maxRole.RoleId;

            return maxId;
        }

        public void addRoleToUser(int userID, Role roleToAdd)
        {
            User user = _users.First(user => user.userId == userID);
            user.roles.Add(roleToAdd);
        }

        public List<User> GetUsers()
        {
            return _users;
        } 
    }
}