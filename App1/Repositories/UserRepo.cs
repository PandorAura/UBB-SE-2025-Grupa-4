﻿using System;
using System.Collections.Generic;
using System.Linq;
using App1.Models;

namespace App1.Repositories
{
    public class UserRepo : IUserRepository
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

        public void generateUsers()
        {
            //users.Add(new User(1,"name@email","Flavius Razvan",0,1,false));
            //users.Add(new User(2, "john.doe@email.com", "John Doe", 0, 1, false));
            //users.Add(new User(3, "jane.smith@email.com", "Jane Smith", 0, 0, true));
            //users.Add(new User(4, "mike.johnson@email.com", "Mike Johnson", 0, 0, false));
            //users.Add(new User(5, "emily.davis@email.com", "Emily Davis", 0, 1, false));
            //users.Add(new User(6, "chris.martin@email.com", "Chris Martin", 0, 0, true));
            //users.Add(new User(7, "lucy.brown@email.com", "Lucy Brown", 0, 1, false));
            //users.Add(new User(8, "peter.white@email.com", "Peter White", 0, 1, false));
            //users.Add(new User(9, "susan.green@email.com", "Susan Green", 0, 1, false));
            //users.Add(new User(10, "robert.blue@email.com", "Robert Blue", 0, 1, false));
            //users.Add(new User(11, "lisa.wilson@email.com", "Lisa Wilson", 0, 1, false));

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

        public User getUserByID(int ID) { 
            return _users[ID];
        }

        public List<User> GetAppealingUsers()
        {
            return _users.Where(u => u.HasAppealed==true && u.PermissionID==0).ToList();

        }


    }
}