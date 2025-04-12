using App1.Models;
using App1.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace App1.Services
{
    public class UpgradeRequestsService : IUpgradeRequestsService
    {
        private readonly IUpgradeRequestsRepository upgradeRequestsRepository;
        private readonly IRolesRepository rolesRepository;
        private readonly IUserRepository userRepository;

        public UpgradeRequestsService(
            IUpgradeRequestsRepository upgradeRequestsRepository,
            IRolesRepository rolesRepository,
            IUserRepository userRepository)
        {
            this.upgradeRequestsRepository = upgradeRequestsRepository;
            this.rolesRepository = rolesRepository;
            this.userRepository = userRepository;
            this.RemoveUpgradeRequestsFromBannedUsers();
        }

        protected void RemoveUpgradeRequestsFromBannedUsers()
        {
            List<UpgradeRequest> pendingUpgradeRequests = this.RetrieveAllUpgradeRequests();
            for (int requestIndex = 0; requestIndex < pendingUpgradeRequests.Count; requestIndex++)
            {
                int requestingUserIdentifier = pendingUpgradeRequests[requestIndex].RequestingUserIdentifier;
                if (this.userRepository.GetHighestRoleTypeForUser(requestingUserIdentifier) == RoleType.Banned)
                {
                    this.upgradeRequestsRepository.RemoveUpgradeRequestByIdentifier(pendingUpgradeRequests[requestIndex].UpgradeRequestId);
                    requestIndex--;
                }
            }
        }

        public string GetRoleNameBasedOnIdentifier(RoleType roleType)
        {
            List<Role> availableRoles = this.rolesRepository.GetAllRoles();
            Role matchingRole = availableRoles.First(role => role.RoleType == roleType);
            return matchingRole.RoleName;
        }

        public List<UpgradeRequest> RetrieveAllUpgradeRequests()
        {
            return this.upgradeRequestsRepository.RetrieveAllUpgradeRequests();
        }

        public void ProcessUpgradeRequest(bool isRequestAccepted, int upgradeRequestIdentifier)
        {
            if (isRequestAccepted)
            {
                UpgradeRequest currentUpgradeRequest = this.upgradeRequestsRepository.RetrieveUpgradeRequestByIdentifier(upgradeRequestIdentifier);
                int requestingUserIdentifier = currentUpgradeRequest.RequestingUserIdentifier;
                RoleType currentHighestRoleType = this.userRepository.GetHighestRoleTypeForUser(requestingUserIdentifier);
                Role nextRoleLevel = rolesRepository.GetNextRole(currentHighestRoleType);
                this.userRepository.AddRoleToUser(requestingUserIdentifier, nextRoleLevel);
            }
            this.upgradeRequestsRepository.RemoveUpgradeRequestByIdentifier(upgradeRequestIdentifier);
        }
    }
}