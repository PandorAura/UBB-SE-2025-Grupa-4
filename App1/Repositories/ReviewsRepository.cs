using System;
using System.Collections.Generic;
using System.Linq;
using App1.Models;

namespace App1.Repositories
{
    public class ReviewsRepository : IReviewsRepository
    {
        private readonly List<Review> _reviews;
        private int _nextReviewId;

        public ReviewsRepository()
        {
            _reviews = new List<Review>();
            _nextReviewId = 1;
            InitializeSampleData();
        }

        private void InitializeSampleData()
        {
            // to be replaced with database initialization
            AddSampleReviews();
        }

        private void AddSampleReviews()
        {
            Review[] sampleReviews = new[]
            {
                new Review(
                    reviewId: _nextReviewId++,
                    userId: 1,
                    rating: 5,
                    content: "Terrible mix, a complete mess dick ass taste",
                    createdDate: DateTime.Now.AddHours(-1),
                    numberOfFlags: 1,
                    isHidden: false
                ),
                new Review(
                    reviewId: _nextReviewId++,
                    userId: 3,
                    rating: 4,
                    content: "Good experience",
                    createdDate: DateTime.Now.AddHours(-5),
                    isHidden: false
                ),
                new Review(
                    reviewId: _nextReviewId++,
                    userId: 1,
                    rating: 2,
                    content: "Such a bitter aftertaste",
                    createdDate: DateTime.Now.AddDays(-1),
                    numberOfFlags: 3,
                    isHidden: false
                ),
                new Review(
                    reviewId: _nextReviewId++,
                    userId: 2,
                    rating: 5,
                    content: "Excellent!",
                    createdDate: DateTime.Now.AddDays(-2),
                    numberOfFlags: 1,
                    isHidden: false
                ),
                new Review(
                    reviewId: _nextReviewId++,
                    userId: 3,
                    rating: 5,
                    content: "dunce",
                    createdDate: DateTime.Now.AddDays(-2),
                    numberOfFlags: 1,
                    isHidden: false
                ),
                new Review(
                    reviewId: _nextReviewId++,
                    userId: 2,
                    rating: 5,
                    content: "Amazing",
                    createdDate: DateTime.Now.AddDays(-2),
                    isHidden: false
                ),
                new Review(
                    reviewId: _nextReviewId++,
                    userId: 2,
                    rating: 5,
                    content: "My favorite!",
                    createdDate: DateTime.Now.AddDays(-2),
                    isHidden: false
                )
            };

            _reviews.AddRange(sampleReviews);
        }

        public List<Review> GetAllReviews()
        {
            return _reviews.ToList();
        }

        public List<Review> GetReviewsSince(DateTime date)
        {
            return _reviews
                .Where(review => review.CreatedDate >= date && !review.IsHidden)
                .OrderByDescending(review => review.CreatedDate)
                .ToList();
        }

        public double GetAverageRatingForVisibleReviews()
        {
            if (!_reviews.Any(review => !review.IsHidden))
                return 0.0;

            double average = _reviews
                .Where(review => !review.IsHidden)
                .Average(r => r.Rating);
            return Math.Round(average, 1);
        }

        public List<Review> GetMostRecentReviews(int count)
        {
            return _reviews
                .Where(review => !review.IsHidden)
                .OrderByDescending(review => review.CreatedDate)
                .Take(count)
                .ToList();
        }

        public int GetReviewCountAfterDate(DateTime date)
        {
            return _reviews
                .Count(review => review.CreatedDate >= date && !review.IsHidden);
        }

        public List<Review> GetFlaggedReviews(int minFlags)
        {
            return _reviews
                .Where(review => review.NumberOfFlags >= minFlags && !review.IsHidden)
                .ToList();
        }

        public List<Review> GetReviewsByUser(int userId)
        {
            return _reviews
                .Where(review => review.UserId == userId && !review.IsHidden)
                .OrderByDescending(review => review.CreatedDate)
                .ToList();
        }

        public Review GetReviewById(int reviewId)
        {
            return _reviews.FirstOrDefault(review => review.ReviewId == reviewId);
        }

        public void UpdateReviewVisibility(int reviewId, bool isHidden)
        {
            Review currentReview = _reviews.FirstOrDefault(review => review.ReviewId == reviewId);

            if (currentReview != null)
            {
                currentReview.IsHidden = isHidden;
            }
        }

        public void UpdateNumberOfFlagsForReview(int reviewId, int numberOfFlags)
        {
            Review currentReview = _reviews.FirstOrDefault(review => review.ReviewId == reviewId);
            
            if (currentReview != null)
            {
                currentReview.NumberOfFlags = numberOfFlags;
            }
        }

        public int AddReview(Review review)
        {
            // Normally, this would be handled by the database
            int newId = _nextReviewId++;
            
            Review newReview = new Review(
                reviewId: newId,
                userId: review.UserId,
                rating: review.Rating,
                content: review.Content,
                createdDate: review.CreatedDate,
                numberOfFlags: review.NumberOfFlags,
                isHidden: review.IsHidden
            );
            
            _reviews.Add(newReview);
            return newId;
        }

        public bool RemoveReviewById(int reviewId)
        {
            Review reviewToRemove = _reviews.FirstOrDefault(review => review.ReviewId == reviewId);
            
            if (reviewToRemove != null)
            {
                _reviews.Remove(reviewToRemove);
                return true;
            }
            
            return false;
        }

        public List<Review> GetHiddenReviews()
        {
            return _reviews.Where(review => review.IsHidden).ToList();
        }
    }
}