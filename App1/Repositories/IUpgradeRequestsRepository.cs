namespace App1.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using App1.Models;

    public interface IUpgradeRequestsRepository
    {
        List<UpgradeRequest> RetrieveAllUpgradeRequests();

        void RemoveUpgradeRequestByIdentifier(int upgradeRequestIdentifier);

        UpgradeRequest RetrieveUpgradeRequestByIdentifier(int upgradeRequestIdentifier);
    }
}
