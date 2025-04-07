using App1.Models;
using App1.Repositories;
using System.Collections.Generic;
using System.Linq;

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
            this.CheckForBannedUserRequests();
        }

        public void CheckForBannedUserRequests()
        {
            List<UpgradeRequest> requests = this.GetAllRequests();
            for(int i=0; i<requests.Count; i++) 
            {
                int userId = requests[i].RequestingUserId;
                if (this.userRepo.getHighestRoleIdBasedOnUserId(userId) == 0)
                {
                    this.upgradeRequestsRepository.deleteRequestBasedOnRequestId(requests[i].RequestId);
                    i--;
                }
            }
        }
        public string GetRoleNameBasedOnID(int roleId)
        {
            List<Role> roles = this.rolesRepository.getRoles();
            Role role = roles.First(role => role.RoleId == roleId);
            return role.RoleName;
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
