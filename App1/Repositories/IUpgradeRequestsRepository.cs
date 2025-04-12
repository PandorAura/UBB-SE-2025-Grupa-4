using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Repositories
{
    public interface IUpgradeRequestsRepository
    {
        List<UpgradeRequest> RetrieveAllUpgradeRequests();
       
        void RemoveUpgradeRequestByIdentifier(int upgradeRequestIdentifier);
        UpgradeRequest RetrieveUpgradeRequestByIdentifier(int upgradeRequestIdentifier);
    }
}
