using App1.Repos;
using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services
{
    internal class ReviewsService
    {
        private ReviewsRepo reviewsRepo;

        public ReviewsService(ReviewsRepo reviewsRepo)
        {
            this.reviewsRepo = reviewsRepo;
        }

        public List<Review> getReviews()
        {
            return reviewsRepo.getReviews();
        }
    }
}
