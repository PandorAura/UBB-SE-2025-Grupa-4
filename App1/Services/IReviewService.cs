﻿using App1.Models;
using System;
using System.Collections.Generic;

namespace App1.Services
{
    public interface IReviewService
    {
        public void HideReview(int reviewID);
        public List<Review> GetFlaggedReviews();
        public List<Review> GetHiddenReviews();
        public List<Review> GetReviews();

        List<Review> GetReviewsSince(DateTime date);
        double GetAverageRating();
        List<Review> GetRecentReviews(int count);
        List<Review> GetReviewsForReport();
        int GetReviewCountSince(DateTime date);
        List<Review> GetReviewsByUser(int userId);
        void resetReviewFlags(int userID);

        // Add new filter methods
        public List<Review> FilterReviewsByContent(string content);
        public List<Review> FilterReviewsByUser(string userFilter);
    }
}
