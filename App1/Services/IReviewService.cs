﻿using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services
{
    public interface IReviewService
    {
        public void HideReview(int reviewID);
        public List<Review> GetFlaggedReviews();
        public List<Review> GetReviews();

        List<Review> GetReviewsSince(DateTime date);
        double GetAverageRating();
        List<Review> GetRecentReviews(int count);
        int GetReviewCountSince(DateTime date);
    }
}
