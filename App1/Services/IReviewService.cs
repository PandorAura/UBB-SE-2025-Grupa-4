using App1.Models;
using System;
using System.Collections.Generic;

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
        List<Review> GetReviewsByUser(int userId);
        void resetReviewFlags(int userID);
    }
}
