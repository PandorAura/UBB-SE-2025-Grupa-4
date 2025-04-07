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
        public List<UpgradeRequest> getAllRequests();

        public void deleteRequestBasedOnRequestId(int requestId);

        public UpgradeRequest getUpgradeRequest(int requestId);

    }
}
