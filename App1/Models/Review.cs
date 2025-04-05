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
        public Review() { }

        public Review(int reviewId, int userId, string userName, int rating,
                     string content, DateTime createdDate, int numberOfFlags = 0,
                     bool isHidden = false)
        {
            ReviewID = reviewId;
            UserID = userId;
            UserName = userName;
            Rating = rating;
            Content = content;
            CreatedDate = createdDate;
            NumberOfFlags = numberOfFlags;
            IsHidden = isHidden;
        }

        public int ReviewID { get; }
        public int UserID { get; }
        public string UserName { get; }
        public int Rating { get; }  
        public string Content { get; set; }
        public DateTime CreatedDate { get; }  
        public int NumberOfFlags { get; set; }
        public bool IsHidden { get; set; }
    }
}