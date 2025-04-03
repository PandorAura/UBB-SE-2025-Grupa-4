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
        private readonly List<Role> _roles;
        public RolesRepository() {

            List<Role> roles = new List<Role>();
            roles.Add(new Role(1, "user"));
            roles.Add(new Role(2, "admin"));
            roles.Add(new Role(3, "manager"));
        }

        public List<Role> getRoles()
        {
            return _roles;
        }

        public Role getUpgradedRoleBasedOnCurrentId(int currentRoleId)
        {
            Role upgradedRole = _roles.First( role => role.RoleId ==  currentRoleId + 1 );
            return upgradedRole;
        }
    }
}
