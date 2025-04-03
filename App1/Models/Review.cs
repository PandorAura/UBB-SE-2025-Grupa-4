﻿using System;
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

        public Review()
        {
            numberOfFlags = 0;
            content = "Lorem ipsum";
            userID = 0;
        }

        public int reviewID { get; set; }
        
        public int numberOfFlags { get; set; }

        public string content { get; set; }
        
        public bool isHidden { get; set; }

        public int userID { get; set; }

    }
}
