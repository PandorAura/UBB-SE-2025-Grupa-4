using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Models;
using App1.Repositories;

namespace App1.Services
{
    internal class ReviewsService
    {
        private readonly ReviewsRepo reviewRepo;

        public ReviewsService() {
            reviewRepo = new ReviewsRepo();
            reviewRepo.generateReviews();
        }
        public ReviewsService(ReviewsRepo reviewsRepo)
        {
            reviewRepo = new ReviewsRepo();
            this.reviewRepo = reviewsRepo;
        }

        public void resetReviewFlags(int reviewID)
        {
            reviewRepo.GetReviewByID(reviewID).numberOfFlags = 0;
        }

        public void HideReview(int reviewID)
        {
            reviewRepo.UpdateHiddenReview(reviewID, true);
        }

        public List<Review> GetFlaggedReviews()
        {
            return reviewRepo.GetReviews().Where(r => r.numberOfFlags > 0).ToList();
        }

        public List<Review> GetReviews()
        {
            return reviewRepo.GetReviews();
        }

        public List<Review> GetReviewsByUser(int userId)
        {
            return reviewRepo.GetReviewsByUser(userId);
        }
    }
}
