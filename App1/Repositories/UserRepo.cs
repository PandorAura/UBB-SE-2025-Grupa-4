// In UserRepo.cs
using System.Collections.Generic;
using System.Linq;
using App1.Models;

namespace App1.Repositories
{
    internal class UserRepo : IUserRepository
    {
        private readonly List<User> _users;

        public UserRepo()
        {
            _users = new List<User>
            {
                new User(
                    userId: 1,
                    email: "mkhenike@gmail.com",
                    name: "Admin One",
                    numberOfDeletedReviews: 3,
                    permissionID: 2,
                    hasAppealed: false
                ),
                 new User(
                    userId: 1,
                    email: "aurapandor@gmail.com",
                    name: "Admin One",
                    numberOfDeletedReviews: 3,
                    permissionID: 2,
                    hasAppealed: false
                )
            };
        }

        public void UpdatePermission(int userID, int permissionID)
        {
            var user = _users.FirstOrDefault(u => u.UserId == userID);
            if (user != null)
            {
                user.PermissionID = permissionID;
            }
        }

        public List<User> GetAppealedUsers()
        {
            return _users.Where(u => u.HasAppealed).ToList();
        }

        public List<User> GetUsersByPermission(int permissionID)
        {
            return _users.Where(u => u.PermissionID == permissionID).ToList();
        }
    }
}