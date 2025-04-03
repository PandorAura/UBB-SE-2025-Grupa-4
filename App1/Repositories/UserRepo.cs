using System;
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

        public void generateUsers()
        {
            users.Add(new User(1,"name@email","Flavius Razvan",0,1,false));
            users.Add(new User(2, "john.doe@email.com", "John Doe", 0, 1, false));
            users.Add(new User(3, "jane.smith@email.com", "Jane Smith", 0, 0, true));
            users.Add(new User(4, "mike.johnson@email.com", "Mike Johnson", 0, 0, false));
            users.Add(new User(5, "emily.davis@email.com", "Emily Davis", 0, 1, false));
            users.Add(new User(6, "chris.martin@email.com", "Chris Martin", 0, 0, true));
            users.Add(new User(7, "lucy.brown@email.com", "Lucy Brown", 0, 1, false));
            users.Add(new User(8, "peter.white@email.com", "Peter White", 0, 1, false));
            users.Add(new User(9, "susan.green@email.com", "Susan Green", 0, 1, false));
            users.Add(new User(10, "robert.blue@email.com", "Robert Blue", 0, 1, false));
            users.Add(new User(11, "lisa.wilson@email.com", "Lisa Wilson", 0, 1, false));

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

        public User getUserByID(int ID) { 
            return users[ID];
        }

        internal List<User> GetAppealingUsers()
        {
            return users.Where(u => u.hasAppealed==true && u.permissionID==0).ToList();

        }


    }
}
