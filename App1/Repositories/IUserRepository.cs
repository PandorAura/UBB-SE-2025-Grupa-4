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
        public void UpdatePermission(int userID, int permissionID);

        public List<User> GetAppealedUsers();

        public List<User> GetUsersByPermission(int permissionID);

        public int getHighestRoleIdBasedOnUserId(int userId);

        public void addRoleToUser(int userID, Role roleToAdd);

        public List<User> GetUsers();

    }
}
