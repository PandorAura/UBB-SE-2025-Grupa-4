using System.Collections.Generic;
using System.Data;
using System.Linq;
using App1.Models;

namespace App1.Repositories
{
    public class UserRepo : IUserRepository
    {
        private readonly List<User> _users;

        public UserRepo()
        {
            List<Role> roles1 = new List<Role>
            {
                new Role(1, "user")
            };
            List<Role> roles2 = new List<Role>
            {
                new Role(1, "user"),
                new Role(2, "admin")
            };
            List<Role> roles3 = new List<Role>
            {
                new Role(0, "banned")
            };
            _users = new List<User>
            {
                new User(
                    userId: 1,
                    email: "mkhenike@gmail.com",
                    name: "Admin One",
                    numberOfDeletedReviews: 3,
                    permissionID: 1,
                    hasAppealed: false,
                    roles: roles1
                ),
                 new User(
                    userId: 2,
                    email: "aurapandor@gmail.com",
                    name: "Admin Two",
                    numberOfDeletedReviews: 3,
                    permissionID: 2,
                    hasAppealed: true,
                    roles: roles2

                ),
                  new User(
                    userId: 3,
                    email: "oanarares2004@gmail.com",
                    name: "Admin Two",
                    numberOfDeletedReviews: 3,
                    permissionID: 1,
                    hasAppealed: true,
                    roles: roles2

                ),
                   new User(
                    userId: 4,
                    email: "nimigeanvalentinoficial@gmail.com",
                    name: "Admin Two",
                    numberOfDeletedReviews: 3,
                    permissionID: 1,
                    hasAppealed: true,
                    roles: roles2

                ),
                    new User(
                    userId: 5,
                    email: "alinamoca25@gmail.com",
                    name: "Admin Two",
                    numberOfDeletedReviews: 3,
                    permissionID: 1,
                    hasAppealed: true,
                    roles: roles2

                ),
                    new User(
                    userId: 6,
                    email: "mkhenike@gmail.com",
                    name: "Banned User",
                    numberOfDeletedReviews: 3,
                    permissionID: 0,
                    hasAppealed: true,
                    roles: roles3
                )
            };
        }

        public List<User> GetAppealedUsers()
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

        public User getUserByID(int ID) { 
            return _users.First(user => user.UserId == ID);
        }

        public List<User> GetAppealingUsers()
        {
            return _users.Where(u => u.HasAppealed == true && u.Roles.Any(r => r.RoleId == 0)).ToList();
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

    }
}