using System.Collections.Generic;
using System.Linq;
using App1.Models;

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
    }
}
