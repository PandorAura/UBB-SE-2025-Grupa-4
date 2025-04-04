using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Repositories
{
    public interface IReviewRepository
    {
        List<Review> GetReviewsSince(DateTime date);
        List<Review> GetReviews();
        public int GetReviewCountSince(DateTime date);
        public double GetAverageRating();
        public List<Review> GetRecentReviews(int count);

        public void UpdateHiddenReview(int reviewID, bool isHidden);
        public void UpdateFlaggedReview(int reviewID, int numberOfFlags);
    }
}
