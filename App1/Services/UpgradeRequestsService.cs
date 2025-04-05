﻿using App1.Models;
using App1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace App1.Services
{

    public class UpgradeRequestsService: IUpgradeRequestsService
    {
        private IUpgradeRequestsRepository upgradeRequestsRepository;
        private IRolesRepository rolesRepository;
        private IUserRepository userRepo;

        public UpgradeRequestsService(IUpgradeRequestsRepository upgradeRequestsRepository, IRolesRepository rolesRepository, IUserRepository newUserRepo)
        {
            this.upgradeRequestsRepository = upgradeRequestsRepository;
            this.rolesRepository = rolesRepository;
            this.userRepo = newUserRepo;
        }

        public List<UpgradeRequest> GetAllRequests()
        {
            return this.upgradeRequestsRepository.getAllRequests();
        }
        public void HandleRequest(bool accepted, int requestId)
        {
            if (accepted)
            {
                UpgradeRequest currentRequest =  this.upgradeRequestsRepository.getUpgradeRequest(requestId);

                int requestingUserId = currentRequest.RequestingUserId;

                int highestRoleId = this.userRepo.getHighestRoleIdBasedOnUserId(requestingUserId);

                Role upgradedRole = rolesRepository.getUpgradedRoleBasedOnCurrentId(highestRoleId);

                this.userRepo.addRoleToUser(requestingUserId, upgradedRole);
            }
            this.upgradeRequestsRepository.deleteRequestBasedOnRequestId(requestId);
        }
    }
}
