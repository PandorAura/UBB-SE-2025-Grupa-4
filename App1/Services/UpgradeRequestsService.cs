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
                if (this.userRepo.GetHighestRoleTypeForUser(userId) == 0)
                {
                    this.upgradeRequestsRepository.deleteRequestBasedOnRequestId(requests[i].RequestId);
                    i--;
                }
            }
        }
        public string GetRoleNameBasedOnID(RoleType roleType)
        {
            List<Role> roles = this.rolesRepository.GetAllRoles();
            Role role = roles.First(role => role.RoleType == roleType);
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

                int highestRoleId = (int)this.userRepo.GetHighestRoleTypeForUser(requestingUserId);

                Role upgradedRole = rolesRepository.GetNextRoleInHierarchy((RoleType)highestRoleId);

                this.userRepo.AddRoleToUser(requestingUserId, upgradedRole);
            }
            this.upgradeRequestsRepository.deleteRequestBasedOnRequestId(requestId);
        }
    }
}
