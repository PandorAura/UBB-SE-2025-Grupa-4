using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;

namespace App1.Models
{
    public class Review
    {
        public int reviewID { get; }
        
        public int numberOfFlags { get; set; }

        public string content { get; set; }
        
        public bool isHidden { get; set; }

        public int userID { get; set; }

        public Review() 
        {
            NumberOfFlags = 0;
            Content = "Lorem ipsum";
            UserId = 0;
        }

        public String toString()
        {
            //return getUserName + Content + "\n";
            return  Content + "\n";
        }

    }
}
