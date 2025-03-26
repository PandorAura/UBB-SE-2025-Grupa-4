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
        public int ReviewId { get; }
        
        public int NumberOfFlags { get; set; }

        public string Content { get; set; }
        
        public bool IsHidden { get; set; }

        public int UserId { get; set; }

    }
}
