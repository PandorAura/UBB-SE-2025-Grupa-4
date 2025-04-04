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
        public List<User> GetAppealedUsers();
        List<User> GetAppealingUsers();
        User getUserByID(int iD);
        public List<User> GetUsersByPermission(int permissionID);
        public void UpdatePermission(int userID, int permissionID);
    }
}
