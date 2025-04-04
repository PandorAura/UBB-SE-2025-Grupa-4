﻿using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Repositories
{
    public interface IRolesRepository
    {
        public Role getUpgradedRoleBasedOnCurrentId(int currentRoleId);
        public List<Role> getRoles();

    }
}
