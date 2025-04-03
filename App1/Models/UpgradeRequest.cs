using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace App1.Models
{
    public class UpgradeRequest
    {
        public int RequestId { get; set; }
        public int RequestingUserId { get; set; }

        public int HighestRoleId { get; set; }
    }
}
