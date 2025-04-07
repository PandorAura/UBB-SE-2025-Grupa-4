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

        //public void UpdateRole(int userID, int roleID)
        //{
        //    var user = _users.FirstOrDefault(u => u.UserId == userID);
        //    if (user != null)
        //    {
        //        Role role = user.Roles.FirstOrDefault(r => r.RoleId == roleID);
        //        user.Roles.Add(new Role(roleID, "New Role"));
        //    }
        //}
        public List<User> GetAppealedUsers()
        {
            return _users.Where(u => u.HasAppealed).ToList();
        }

        public List<User> GetAppealingUsers()
        {
            return _users.Where(u => u.HasAppealed).ToList();
        }

        public List<User> GetUsersByRole(int roleID)
        {
            return _users.Where(u => u.Roles.Any(r => r.RoleId == roleID)).ToList();
        }

        public int getHighestRoleIdBasedOnUserId(int userId)
        {
            User user = _users.First(user => user.UserId == userId);

            List<Role> roles = user.Roles;

            Role maxRole = roles.MaxBy(role => role.RoleId);

            int maxId = maxRole.RoleId;

            return maxId;
        }

        public void addRoleToUser(int userID, Role roleToAdd)
        {
            User user = _users.First(user => user.UserId == userID);
            user.Roles.Add(roleToAdd);
        }

        public List<User> GetUsers()
        {
            return _users;
        }
        public User getUserByID(int iD)
        {
            return _users[iD];
        }

    }
}