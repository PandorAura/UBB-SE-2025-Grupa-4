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
                new Role(1, "user"),
                new Role(2, "admin"),
                new Role(3, "manager")
            };
            List<Role> roles4 = new List<Role>
            {
                new Role(0, "banned")
            };
            _users = new List<User>
            {
                new User(
                    userId: 1,
                    email: "bianca.georgiana.cirnu@gmail.com",
                    name: "bianca",
                    numberOfDeletedReviews: 2,
                    hasAppealed: true,
                    roles: roles1
                    ),
                new User(
                    userId: 2,
                    email: "ciobanueduarda77@gmail.com",
                    name: "eduarda",
                    numberOfDeletedReviews: 2,
                    hasAppealed: true,
                    roles: roles1
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
            User? user = _users.FirstOrDefault(user => user.UserId == userId);

            if (user == null)
            {
                return 0;
            }

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