using System.Collections.Generic;
using System.Linq;
using App1.Models;

namespace App1.Repositories
{
    internal class ReviewsRepo
    {
        private List<Review> reviews;

        public ReviewsRepo() => reviews = new List<Review>() {

            new Review { reviewID = 1, userID = 101, content = "Great product!", numberOfFlags = 0, isHidden = false },
                new Review { reviewID = 2, userID = 102, content = "Not as expected", numberOfFlags = 2, isHidden = false },
                new Review { reviewID = 3, userID = 101, content = "Would buy again!", numberOfFlags = 0, isHidden = false },
                new Review { reviewID = 4, userID = 103, content = "Terrible quality", numberOfFlags = 5, isHidden = true },
                new Review { reviewID = 5, userID = 104, content = "Fast shipping, good service.", numberOfFlags = 0, isHidden = false },
                new Review { reviewID = 6, userID = 105, content = "Defective item received.", numberOfFlags = 3, isHidden = false },
                new Review { reviewID = 7, userID = 106, content = "Customer support was helpful.", numberOfFlags = 1, isHidden = false },
                new Review { reviewID = 8, userID = 107, content = "The material feels cheap.", numberOfFlags = 4, isHidden = false },
                new Review { reviewID = 9, userID = 108, content = "Absolutely love it!", numberOfFlags = 0, isHidden = false },
                new Review { reviewID = 10, userID = 109, content = "Not worth the price.", numberOfFlags = 2, isHidden = false },
                new Review { reviewID = 11, userID = 110, content = "Would recommend!", numberOfFlags = 0, isHidden = false },
                new Review { reviewID = 12, userID = 111, content = "Came with missing parts.", numberOfFlags = 3, isHidden = false },
                new Review { reviewID = 13, userID = 112, content = "Works better than expected!", numberOfFlags = 0, isHidden = false },
                new Review { reviewID = 14, userID = 113, content = "Packaging was damaged.", numberOfFlags = 2, isHidden = false },
                new Review { reviewID = 15, userID = 114, content = "Looks amazing!", numberOfFlags = 0, isHidden = false },
                new Review { reviewID = 16, userID = 115, content = "Item never arrived.", numberOfFlags = 5, isHidden = true },
                new Review { reviewID = 17, userID = 116, content = "Too small, doesn't fit.", numberOfFlags = 3, isHidden = false },
                new Review { reviewID = 18, userID = 117, content = "Surprisingly good quality.", numberOfFlags = 0, isHidden = false },
                new Review { reviewID = 19, userID = 118, content = "False advertising.", numberOfFlags = 6, isHidden = true },
                new Review { reviewID = 20, userID = 119, content = "Best purchase I've made!", numberOfFlags = 0, isHidden = false }
        };

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
