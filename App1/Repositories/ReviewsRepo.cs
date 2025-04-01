using System.Collections.Generic;
using System.Linq;
using App1.Models;

namespace App1.Repositories
{
    internal class ReviewsRepo
    {
        private List<Review> reviews;

        public ReviewsRepo()
        {
            reviews = new List<Review>();
        }

        public List<Review> GetReviews()
        {
            return reviews;
        }

        public List<Review> GetReviewsByUser(int userID)
        {
            return reviews.Where(r => r.userID == userID).ToList();
        }

        public void UpdateNumberOfFlags(int reviewID, int numberOfFlags)
        {
            var review = reviews.FirstOrDefault(r => r.reviewID == reviewID);
            if (review != null)
            {
                review.numberOfFlags = numberOfFlags;
            }
        }

        public void UpdateHiddenReview(int reviewID, bool isHidden)
        {
            var review = reviews.FirstOrDefault(r => r.reviewID == reviewID);
            if (review != null)
            {
                review.isHidden = isHidden;
            }
        }

        public Review GetReviewByID(int reviewID)
        {
            return reviews.FirstOrDefault(r => r.reviewID == reviewID);
        }
    }
}
