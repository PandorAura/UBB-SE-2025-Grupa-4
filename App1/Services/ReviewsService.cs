using System.Collections.Generic;
using System.Linq;
using App1.Models;
using App1.Repositories;

namespace App1.Services
{
    internal class ReviewsService
    {
        private readonly ReviewsRepo reviewRepo;

        public ReviewsService()
        {
            reviewRepo = new ReviewsRepo();
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
    }
}
