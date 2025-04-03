using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    public class User
    {
        public User()
        {
            this.userId = 0;
            this.email = "example@iss.com";
        }

        public User(int userId)
        {
            this.userId = userId;
            this.email = "example" + userId.ToString() + "@iss.com";
        }

        public int userId { get; }
        public string email { get; }

        public string name { get; }

        public int numberOfDeletedReviews { get; }

        public int permissionID { get; set; }

        public bool hasAppealed { get; }  

        public override string ToString()
        {
            return "Id: " + userId.ToString() + ", email: " + email;
        }

    }
}
