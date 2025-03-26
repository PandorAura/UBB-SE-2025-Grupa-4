using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    public class User
    {
        public int UserId { get; }
        public string Email { get; }

        public string Name { get; }

        public int NumberOfDeletedReviews { get; }

        public int PermissionId { get; }

        public bool HasAppealed { get; }  

    }
}
