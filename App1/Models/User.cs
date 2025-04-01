using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    public class User
    {
        public int userId { get; }
        public string email { get; }

        public string name { get; }

        public int numberOfDeletedReviews { get; }

        public int permissionID { get; set; }

        public bool hasAppealed { get; }  

    }
}
