using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Repositories
{
    public class HardcodedUpgradeRequestsRepository
    {
        private List<UpgradeRequest> _requests;
        public HardcodedUpgradeRequestsRepository() {
            _requests = new List<UpgradeRequest>();
            _requests.Add(new UpgradeRequest(1, 1));
            _requests.Add(new UpgradeRequest(2, 2));
            _requests.Add(new UpgradeRequest(3, 3));
            _requests.Add(new UpgradeRequest(4, 4));
        }

        public List<UpgradeRequest> getAllRequests()
        {
            return _requests;
        }

        public void deleteRequestBasedOnRequestId(int requestId)
        {
            _requests.RemoveAll(r =>  r.RequestId == requestId);
        }
        public UpgradeRequest getUpgradeRequest(int requestId)
        {
            UpgradeRequest request = _requests.First(request => request.RequestId == requestId);
            return request;
        }
    }
}
