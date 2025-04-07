using System.Collections.Generic;

namespace App1.Services
{
    public interface IUserService
    {
        public List<User> GetActiveUsers(int permissionId);
        public List<User> GetAllUsers();
        public List<User> GetAppealingUsers();
        public List<User> GetBannedUsers();
        List<User> GetUsersByPermission(int permissionId);
        public User GetUserBasedOnID(int ID);
        public int GetHighestRoleBasedOnUserID(int ID);
    }
}
