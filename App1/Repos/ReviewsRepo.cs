using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Repos
{
    internal class ReviewsRepo
    {
        private List<Review> reviews;

        public ReviewsRepo(List<Review> reviews)
        {
            this.reviews = reviews;
        }

        public List<Review> getReviews() { return reviews; }

        public List<Review> getReviewsByUser(int userId)
        {
            List<Review> reviewsByUser = new List<Review>();
            foreach (var review in getReviews())
            {
                if (review.UserId == userId) { reviewsByUser.Add(review); }
            }
            return reviewsByUser;
        }
    }
}
