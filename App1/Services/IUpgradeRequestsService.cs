using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services
{
    public interface IUpgradeRequestsService
    {
        public List<UpgradeRequest> GetAllRequests();
        public void HandleRequest(bool accepted, int requestId);

    }
}
