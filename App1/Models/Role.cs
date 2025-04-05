﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public Role(int roleId, string roleName)
        {
            RoleId = roleId;
            RoleName = roleName;
        }
    }
}
