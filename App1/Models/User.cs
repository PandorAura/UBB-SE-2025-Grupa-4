using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.System;

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

        public User(int userId, string email, string name, int numberOfDeletedReviews, int permissionID, bool hasAppealed) : this(userId)
        {
            this.email = email;
            this.name = name;
            this.numberOfDeletedReviews = numberOfDeletedReviews;
            this.permissionID = permissionID;
            this.hasAppealed = hasAppealed;
        }

        public int userId { get; }
        public string email { get; }

        public string name { get; }

        public int numberOfDeletedReviews { get; }

        public int permissionID { get; set; }

        public List<Role> roles { get; set; }

        public bool hasAppealed { get; set; }

        public User(int userId, string email, string name, int numberOfDeletedReviews, int permissionID, bool hasAppealed, List<Role> roles)
        {
            this.userId = userId;
            this.email = email;
            this.name = name;
            this.numberOfDeletedReviews = numberOfDeletedReviews;
            this.permissionID = permissionID;
            this.hasAppealed = hasAppealed;
            this.roles = roles;
        }

        public override string ToString()
        {
            return "Id: " + userId.ToString() + ", email: " + email;
        }

    }
}
