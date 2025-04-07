using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Repositories
{
    public interface IUserRepository
    {
        //public void UpdateRole(int userID, int permissionID);

        public List<User> GetAppealedUsers();
        List<User> GetAppealingUsers();
        User getUserByID(int iD);
        public List<User> GetUsersByRole(int permissionID);

        public int getHighestRoleIdBasedOnUserId(int userId);

        public void addRoleToUser(int userID, Role roleToAdd);

        public List<User> GetUsers();

    }
}
