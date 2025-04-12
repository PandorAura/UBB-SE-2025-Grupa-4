using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Repositories
{
    public class RolesRepository : IRolesRepository
    {
        private readonly List<Role> _roles;
        public RolesRepository()
        {
            _roles = new List<Role>();

            _roles.Add(new Role(RoleType.Banned, "Banned"));
            _roles.Add(new Role(RoleType.User, "User"));
            _roles.Add(new Role(RoleType.Admin, "Admin"));
            _roles.Add(new Role(RoleType.Manager, "Manager"));
        }

        public List<Role> GetAllRoles()
        {
            return _roles;
        }

        public Role GetNextRole(RoleType currentRoleType)
        {
            Role nextRole = _roles.First(role => role.RoleType == currentRoleType + 1);
            return nextRole;
        }
    }
}