using App1.Models;
using System.Collections.Generic;

namespace App1.Services
{
    public interface IUserService
    {
        public List<User> GetActiveUsersByRoleType(RoleType roleType);
        public List<User> GetAllUsers();
        public List<User> GetBannedUsersWhoHaveSubmittedAppeals();
        public List<User> GetBannedUsers();
        public List<User> GetAdminUsers();
        public List<User> GetRegularUsers();
        public List<User> GetManagers();
        List<User> GetUsersByRoleType(RoleType roleType);
        public User GetUserById(int ID);
        public RoleType GetHighestRoleTypeForUser(int ID);

        void UpdateUserRole(int userId, RoleType roleType);
    }
}
