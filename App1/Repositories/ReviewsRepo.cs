using System;
using System.Collections.Generic;
using System.Linq;
using App1.Models;

namespace App1.Repositories
{
    public class ReviewRepo : IReviewRepository
    {
        private List<Review> _reviews = new()
        {
            new Review(
                reviewId: 1,
                userId: 1,
                userName: "Admin One",
                rating: 5,
                content: "Great service!",
                createdDate: DateTime.Now.AddHours(-1),
                numberOfFlags: 1,
                isHidden: false
            ),
            new Review(
                reviewId: 2,
                userId: 3,
                userName: "Regular User",
                rating: 4,
                content: "Good experience",
                createdDate: DateTime.Now.AddHours(-5),
                isHidden: false
            ),
            new Review(
                reviewId: 3,
                userId: 1,
                userName: "Regular User",
                rating: 2,
                content: "Could be better",
                createdDate: DateTime.Now.AddDays(-1),
                numberOfFlags: 3,
                isHidden: false
            ),
            new Review(
                reviewId: 4,
                userId: 2,
                userName: "Admin Two",
                rating: 5,
                content: "Excellent!",
                createdDate: DateTime.Now.AddDays(-2),
                numberOfFlags: 1,
                isHidden: false
            ),
            new Review(
                reviewId: 5,
                userId: 3,
                userName: "Admin Three",
                rating: 5,
                content: "dunce",
                createdDate: DateTime.Now.AddDays(-2),
                numberOfFlags: 1,
                isHidden: false
            ),
            new Review(
                reviewId: 6,
                userId: 2,
                userName: "Admin Two",
                rating: 5,
                content: "Amazing",
                createdDate: DateTime.Now.AddDays(-2),
                numberOfFlags: 0,
                isHidden: false
            ),
            new Review(
                reviewId: 7,
                userId: 2,
                userName: "Admin Two",
                rating: 5,
                content: "My favorite!",
                createdDate: DateTime.Now.AddDays(-2),
                numberOfFlags: 0,
                isHidden: false
            )

        };

        public List<Review> GetReviewsSince(DateTime date)
        {
            var result = _reviews
                .Where(r => r.CreatedDate >= date && !r.IsHidden)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
            return result;
        }

        public double GetAverageRating()
        {
            if (!_reviews.Any(r => !r.IsHidden))
                return (0.0);

            var average = _reviews
                .Where(r => !r.IsHidden)
                .Average(r => r.Rating);
            return Math.Round(average, 1);
        }

        public List<Review> GetRecentReviews(int count)
        {
            var result = _reviews
                .Where(r => !r.IsHidden)
                .OrderByDescending(r => r.CreatedDate)
                .Take(count)
                .ToList();
            return result;
        }

        public int GetReviewCountSince(DateTime date)
        {
            var count = _reviews
                .Count(r => r.CreatedDate >= date && !r.IsHidden);
            return count;
        }

        public List<Review> GetFlaggedReviews(int minFlags)
        {
            var result = _reviews
                .Where(r => r.NumberOfFlags >= minFlags && !r.IsHidden)
                .ToList();
            return result;
        }

        public List<Review> GetReviews()
        {
            return _reviews;
        }

        public void UpdateHiddenReview(int reviewID, bool isHidden)
        {
            var review = _reviews.FirstOrDefault(r => r.ReviewID == reviewID);

            if(review != null)
            {
                review.IsHidden = isHidden;
            }
        }

        public void UpdateFlaggedReview(int reviewID, int numberOfFlags)
        {
            _reviews[reviewID].NumberOfFlags = numberOfFlags;
        }

        public List<Review> GetReviewsByUser(int userId)
        {
            return _reviews
                .Where(r => r.UserID == userId && !r.IsHidden)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public Review GetReviewByID(int reviewID)
        {
            return _reviews.FirstOrDefault(r => r.ReviewID == reviewID);
        }
    }
}