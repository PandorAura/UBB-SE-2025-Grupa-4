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
        public int reviewID { get; }
        
        public int numberOfFlags { get; set; }

        public string content { get; set; }
        
        public bool isHidden { get; set; }

        public int userID { get; set; }

        public Review(int id, string text, int user)
        {
            reviewID = id;
            content = text;
            userID = user;
            numberOfFlags = 0;
            isHidden = false;
        }
    }
}
