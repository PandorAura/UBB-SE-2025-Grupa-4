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
          this.users = new List<User> { 
    new User(101, "alice@example.com", "Alice", true, 2, 1),
    new User(102, "bob@example.com", "Bob", false, 0, 0),
    new User(103, "charlie@example.com", "Charlie", true, 5, 1),
    new User(104, "david@example.com", "David", false, 1, 0),
    new User(105, "emma@example.com", "Emma", true, 3, 1),
    new User(106, "frank@example.com", "Frank", false, 0, 0),
    new User(107, "grace@example.com", "Grace", true, 2, 1),
    new User(108, "henry@example.com", "Henry", false, 4, 0),
    new User(109, "ivy@example.com", "Ivy", true, 1, 1),
    new User(110, "jack@example.com", "Jack", false, 2, 0),
    new User(111, "kate@example.com", "Kate", true, 0, 1),
    new User(112, "leo@example.com", "Leo", false, 3, 0),
    new User(113, "mia@example.com", "Mia", true, 4, 1),
    new User(114, "nathan@example.com", "Nathan", false, 1, 0),
    new User(115, "olivia@example.com", "Olivia", true, 5, 1),
    new User(116, "peter@example.com", "Peter", false, 0, 0),
    new User(117, "quinn@example.com", "Quinn", true, 2, 1),
    new User(118, "rachel@example.com", "Rachel", false, 3, 0),
    new User(119, "steve@example.com", "Steve", true, 1, 1),
    new User(120, "tina@example.com", "Tina", false, 4, 0),
    new User(121, "ursula@example.com", "Ursula", true, 2, 1),
    new User(122, "victor@example.com", "Victor", false, 0, 0),
    new User(123, "wendy@example.com", "Wendy", true, 3, 1),
    new User(124, "xander@example.com", "Xander", false, 5, 0),
    new User(125, "yara@example.com", "Yara", true, 1, 1),
    new User(126, "zane@example.com", "Zane", false, 2, 0)
};


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
