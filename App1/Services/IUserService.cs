using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services
{
    public interface IUserService
    {
        public void ChangeUserPermission(int userId, int permissionId);
        public List<User> GetActiveUsers(int permissionId);
        public List<User> GetBannedUsers();
        List<User> GetUsersByPermission(int permissionId);
    }
}
