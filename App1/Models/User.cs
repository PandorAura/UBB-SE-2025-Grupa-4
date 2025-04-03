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
            this.name = string.Empty;
            this.permissionID = 1;
            

            
        }

        public User(int userId)
        {
            this.userId = userId;
            this.email = "example" + userId.ToString() + "@iss.com";
        }

        public User(int userId,string email,string name, bool appeal,int deleted,int permisionid) 
        {

            this.userId = userId;
            this.email = email;
            this.hasAppealed = appeal;
            this.numberOfDeletedReviews = deleted;
            this.name = name;
            this.permissionID = permisionid;


        }

        public int userId { get; }
        public string email { get;  }

        public string name { get;  }

        public int numberOfDeletedReviews { get; }

        public int permissionID { get; set; }

        public bool hasAppealed { get; set; }  

        public override string ToString()
        {
            return "Id: " + userId.ToString() + ", email: " + email;
        }

    }
}
