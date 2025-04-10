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

        public List<User> GetUsersWhoHaveSubmittedAppeals();
        List<User> GetBannedUsersWhoHaveSubmittedAppeals();
        User GetUserByID(int iD);
        public List<User> GetUsersByRoleType(RoleType roleType);

        public RoleType GetHighestRoleTypeForUser(int userId);

        public void AddRoleToUser(int userID, Role roleToAdd);

        public List<User> GetAllUsers();

    }
}
