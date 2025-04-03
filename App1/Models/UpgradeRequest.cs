using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App1.Models
{
    public class UpgradeRequest
    {
        public UpgradeRequest(int newRequestId, int newRequestingUserId) { 
            RequestId = newRequestId;
            this.RequestingUserId = newRequestingUserId;
        }
        public int RequestId { get; set; }

        public int RequestingUserId { get; set; }
    }
}
