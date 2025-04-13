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
            this._reviewsRepository.UpdateNumberOfFlagsForReview(reviewId, 0);
        }

        public void HideReview(int reviewId)
        {
            this._reviewsRepository.UpdateReviewVisibility(reviewId, true);
        }

        public List<Review> GetFlaggedReviews()
        {
            return this._reviewsRepository.GetAllReviews().Where(review => review.NumberOfFlags > 0).ToList();
        }

        public List<Review> GetHiddenReviews()
        {
            return this._reviewsRepository.GetAllReviews().Where(review => review.IsHidden == true).ToList();
        }

        public List<Review> GetAllReviews()
        {
            return this._reviewsRepository.GetAllReviews();
        }

        public List<Review> GetReviewsSince(DateTime date)
        {
            return this._reviewsRepository.GetReviewsSince(date);
        }

        public double GetAverageRatingForVisibleReviews()
        {
            return this._reviewsRepository.GetAverageRatingForVisibleReviews();
        }

        public List<Review> GetMostRecentReviews(int count)
        {
            return this._reviewsRepository.GetMostRecentReviews(count);
        }

        public int GetReviewCountAfterDate(DateTime date)
        {
            return this._reviewsRepository.GetReviewCountAfterDate(date);
        }

        public List<Review> GetReviewsByUser(int userId)
        {
            return this._reviewsRepository.GetReviewsByUser(userId);
        }

        public List<Review> GetReviewsForReport()
        {
            DateTime date = DateTime.Now.AddDays(-1);
            int count = this._reviewsRepository.GetReviewCountAfterDate(date);

            List<Review> reviews = this._reviewsRepository.GetMostRecentReviews(count);
            return reviews ?? [];
        }

        public List<Review> FilterReviewsByContent(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return GetFlaggedReviews();
            }

            content = content.ToLower();
            return GetFlaggedReviews()
                .Where(review => review.Content.ToLower().Contains(content))
                .ToList();
        }

        //public List<Review> FilterReviewsByUser(string userFilter)
        //{
        //    if (string.IsNullOrEmpty(userFilter))
        //    {
        //        return GetFlaggedReviews();
        //    }

        //    userFilter = userFilter.ToLower();
        //    return GetFlaggedReviews()
        //        .Where(review => review.UserName.ToLower().Contains(userFilter))
        //        .ToList();
        //}
    }
}