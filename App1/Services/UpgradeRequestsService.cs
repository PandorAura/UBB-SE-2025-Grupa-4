using App1.Models;
using App1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services
{

    public class UpgradeRequestsService
    {
        private UpgradeRequestsRepository upgradeRequestsRepository;
        private IRolesRepository rolesRepository;

        public UpgradeRequestsService(UpgradeRequestsRepository upgradeRequestsRepository, IRolesRepository rolesRepository)
        {
            this.upgradeRequestsRepository = upgradeRequestsRepository;
            this.rolesRepository = rolesRepository;
        }

        public List<UpgradeRequest> getAllRequests()
        {
            return this.upgradeRequestsRepository.getAllRequests();
        }

        public void handleRequest(bool accepted, int requestId)
        {
            if (accepted)
            {
                UpgradeRequest currentRequest =  this.upgradeRequestsRepository.getUpgradeRequest(requestId);

                Role upgradedRole = rolesRepository.getUpgradedRoleBasedOnCurrentId(currentRequest.HighestRoleId);

                // add role to the user's roles list

            }
            this.upgradeRequestsRepository.deleteRequestBasedOnRequestId(requestId);
        }
    }
}
