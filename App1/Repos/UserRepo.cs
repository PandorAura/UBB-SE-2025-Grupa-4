using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Models;

namespace App1.Repos
{
    internal class UserRepo
    {
        private List<User> userList;

        public UserRepo(List<User> userList)
        {
            this.userList = userList;
        }

        public List<User> getUsers()
        {
            return userList;
        }
    }
}
