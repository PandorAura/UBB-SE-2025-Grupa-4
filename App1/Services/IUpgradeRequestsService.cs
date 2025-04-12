using App1.Models;
using System.Collections.Generic;

namespace App1.Services
{
    public interface IUpgradeRequestsService
    {
        List<UpgradeRequest> RetrieveAllUpgradeRequests();
        void ProcessUpgradeRequest(bool isRequestAccepted, int upgradeRequestIdentifier);
        string GetRoleNameBasedOnIdentifier(RoleType roleType);
    }
}
