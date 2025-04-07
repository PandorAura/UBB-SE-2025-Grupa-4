using System;
using System.Collections.Generic;
using System.Linq;
using App1.Models;
using App1.Repositories;

namespace App1.Services
{
    public class ReviewsService: IReviewService
    {
        private readonly IReviewRepository _reviewRepository;

        public ReviewsService() { }
        public ReviewsService(IReviewRepository reviewRepository)
        {
            this._reviewRepository = reviewRepository;
        }

        public void resetReviewFlags(int reviewID)
        {
            _reviewRepository.GetReviewByID(reviewID).NumberOfFlags = 0;
        }

        public void HideReview(int reviewID)
        {
            _reviewRepository.UpdateHiddenReview(reviewID, true);
        }

        public List<Review> GetFlaggedReviews()
        {
            return _reviewRepository.GetReviews().Where(r => r.NumberOfFlags > 0).ToList();
        }

        public List<Review> GetHiddenReviews()
        {
            return _reviewRepository.GetReviews().Where(r => r.IsHidden == true).ToList();
        }

        public List<Review> GetReviews()
        {
            return _reviewRepository.GetReviews();
        }

        public List<Review> GetReviewsSince(DateTime date)
        {
            return _reviewRepository.GetReviewsSince(date);
        }

        public double GetAverageRating()
        {
            return _reviewRepository.GetAverageRating();
        }

        public List<Review> GetRecentReviews(int count)
        {
            return _reviewRepository.GetRecentReviews(count);
        }

        public int GetReviewCountSince(DateTime date)
        {
            return _reviewRepository.GetReviewCountSince(date);
        }

        public List<Review> GetReviewsByUser(int userId)
        {
            return _reviewRepository.GetReviewsByUser(userId);
        }
    }
}

