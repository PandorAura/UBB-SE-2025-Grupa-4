using System;
using System.Collections.Generic;
using System.Linq;
using App1.Models;
using App1.Repositories;

namespace App1.Services
{
    public class ReviewsService: IReviewService
    {
        private readonly IReviewsRepository _reviewsRepository;

        public ReviewsService(IReviewsRepository reviewsRepository)
        {
            this._reviewsRepository = reviewsRepository;
        }

        public void ResetReviewFlags(int reviewId)
        {
            _reviewsRepository.GetReviewById(reviewId).NumberOfFlags = 0;
        }

        public void HideReview(int reviewId)
        {
            _reviewsRepository.UpdateReviewVisibility(reviewId, true);
        }

        public List<Review> GetFlaggedReviews()
        {
            return _reviewsRepository.GetAllReviews().Where(r => r.NumberOfFlags > 0).ToList();
        }

        public List<Review> GetHiddenReviews()
        {
            return _reviewsRepository.GetAllReviews().Where(r => r.IsHidden == true).ToList();
        }

        public List<Review> GetAllReviews()
        {
            return _reviewsRepository.GetAllReviews();
        }

        public List<Review> GetReviewsSince(DateTime date)
        {
            return _reviewsRepository.GetReviewsSince(date);
        }

        public double GetAverageRatingForVisibleReviews()
        {
            return _reviewsRepository.GetAverageRatingForVisibleReviews();
        }

        public List<Review> GetMostRecentReviews(int count)
        {
            return _reviewsRepository.GetMostRecentReviews(count);
        }

        public int GetReviewCountAfterDate(DateTime date)
        {
            return _reviewsRepository.GetReviewCountAfterDate(date);
        }

        public List<Review> GetReviewsByUser(int userId)
        {
            return _reviewsRepository.GetReviewsByUser(userId);
        }

        public List<Review> GetReviewsForReport()
        {
            return GetMostRecentReviews(GetReviewCountAfterDate(DateTime.Now.AddDays(-1)));
        }
    }
}

