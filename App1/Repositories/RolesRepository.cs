using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Repositories
{
    public class RolesRepository
    {
        private List<Role> roles;
        public RolesRepository() {

            List<Role> roles = new List<Role>();
            roles.Add(new Role(1, "user"));
            roles.Add(new Role(2, "admin"));
            roles.Add(new Role(3, "manager"));
        }

        public List<Role> getRoles()
        {
            return roles;
        }

        public Role getUpgradedRoleBasedOnCurrentId(int currentRoleId)
        {
            Role upgradedRole = roles.First( role => role.RoleId ==  currentRoleId + 1 );
            return upgradedRole;
        }
    }
}
