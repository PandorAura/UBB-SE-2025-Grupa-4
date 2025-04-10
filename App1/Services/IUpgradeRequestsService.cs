using App1.Models;
using System.Collections.Generic;

namespace App1.Services
{
    public interface IUpgradeRequestsService
    {
        public List<UpgradeRequest> GetAllRequests();
        public void HandleRequest(bool accepted, int requestId);
        public string GetRoleNameBasedOnID(RoleType roleType);
    }
}
