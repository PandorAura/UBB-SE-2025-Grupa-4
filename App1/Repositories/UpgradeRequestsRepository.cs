using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Repositories
{
    public class UpgradeRequestsRepository
    {
        public UpgradeRequestsRepository() { }

        public List<UpgradeRequest> getAllRequests()
        {
            List<UpgradeRequest> requests = new List<UpgradeRequest>();
            // get all the requests from the database
            return requests;
        }

        public void deleteRequestBasedOnRequestId(int requestId)
        {
            // delete from the database
        }
        public UpgradeRequest getUpgradeRequest(int requestId)
        {
            // get from database based on id
            UpgradeRequest request = new UpgradeRequest(1, 1, "");
            return request;
        }
    }
}
