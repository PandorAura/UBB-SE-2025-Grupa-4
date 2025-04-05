using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Repositories
{
    public class RolesRepository: IRolesRepository
    {
        private readonly List<Role> _roles;
        public RolesRepository() {
            _roles = new List<Role>();

            _roles.Add(new Role(0, "banned"));
            _roles.Add(new Role(1, "user"));
            _roles.Add(new Role(2, "admin"));
            _roles.Add(new Role(3, "manager"));
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
