using App1.Models;
using App1.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTests.Reviews
{
    public class ReviewsRepositoryTests
    {
        [Fact]
        public void GetAllReviews_ReturnsAllReviews()
        {
            // Arrange
            var repository = new ReviewsRepository();

            // Act
            var reviews = repository.GetAllReviews();

            // Assert
            Assert.NotNull(reviews);
            Assert.True(reviews.Count > 0);
        }

        [Fact]
        public void GetReviewsSince_ReturnsReviewsAfterDate()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var date = DateTime.Now.AddDays(-2);

            // Act
            var reviews = repository.GetReviewsSince(date);

            // Assert
            Assert.NotNull(reviews);
            Assert.All(reviews, review => Assert.True(review.CreatedDate >= date));
            Assert.All(reviews, review => Assert.False(review.IsHidden));
        }

        [Fact]
        public void GetAverageRatingForVisibleReviews_ReturnsCorrectAverage()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var allReviews = repository.GetAllReviews();
            var visibleReviews = allReviews.Where(review => !review.IsHidden).ToList();
            var expectedAverage = visibleReviews.Any() ? Math.Round(visibleReviews.Average(review => review.Rating), 1) : 0.0;

            // Act
            var average = repository.GetAverageRatingForVisibleReviews();

            // Assert
            Assert.Equal(expectedAverage, average);
        }

        [Fact]
        public void GetMostRecentReviews_ReturnsCorrectNumberOfReviews()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var count = 3;

            // Act
            var reviews = repository.GetMostRecentReviews(count);

            // Assert
            Assert.NotNull(reviews);
            Assert.True(reviews.Count <= count);
            Assert.All(reviews, review => Assert.False(review.IsHidden));
            
            // Verify they are ordered by date (most recent first)
            for (int i = 0; i < reviews.Count - 1; i++)
            {
                Assert.True(reviews[i].CreatedDate >= reviews[i + 1].CreatedDate);
            }
        }

        [Fact]
        public void GetReviewCountAfterDate_ReturnsCorrectCount()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var date = DateTime.Now.AddDays(-2);
            var allReviews = repository.GetAllReviews();
            var expectedCount = allReviews.Count(review => review.CreatedDate >= date && !review.IsHidden);

            // Act
            var count = repository.GetReviewCountAfterDate(date);

            // Assert
            Assert.Equal(expectedCount, count);
        }

        [Fact]
        public void GetFlaggedReviews_ReturnsReviewsWithMinFlags()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var minFlags = 1;
            var allReviews = repository.GetAllReviews();
            var expectedReviews = allReviews.Where(review => review.NumberOfFlags >= minFlags && !review.IsHidden).ToList();

            // Act
            var reviews = repository.GetFlaggedReviews(minFlags);

            // Assert
            Assert.NotNull(reviews);
            Assert.Equal(expectedReviews.Count, reviews.Count);
            Assert.All(reviews, review => Assert.True(review.NumberOfFlags >= minFlags));
            Assert.All(reviews, review => Assert.False(review.IsHidden));
        }

        [Fact]
        public void GetReviewsByUser_ReturnsCorrectReviews()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var userId = 1;
            var allReviews = repository.GetAllReviews();
            var expectedReviews = allReviews.Where(review => review.UserId == userId && !review.IsHidden).ToList();

            // Act
            var reviews = repository.GetReviewsByUser(userId);

            // Assert
            Assert.NotNull(reviews);
            Assert.Equal(expectedReviews.Count, reviews.Count);
            Assert.All(reviews, review => Assert.Equal(userId, review.UserId));
            Assert.All(reviews, review => Assert.False(review.IsHidden));
            
            // Verify they are ordered by date (most recent first)
            for (int i = 0; i < reviews.Count - 1; i++)
            {
                Assert.True(reviews[i].CreatedDate >= reviews[i + 1].CreatedDate);
            }
        }

        [Fact]
        public void GetReviewById_ReturnsCorrectReview()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var allReviews = repository.GetAllReviews();
            var reviewId = allReviews.First().ReviewId;

            // Act
            var review = repository.GetReviewById(reviewId);

            // Assert
            Assert.NotNull(review);
            Assert.Equal(reviewId, review.ReviewId);
        }

        [Fact]
        public void GetReviewById_ReturnsNullForNonExistentId()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var nonExistentId = 9999;

            // Act
            var review = repository.GetReviewById(nonExistentId);

            // Assert
            Assert.Null(review);
        }

        [Fact]
        public void UpdateReviewVisibility_UpdatesIsHidden()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var allReviews = repository.GetAllReviews();
            var reviewId = allReviews.First().ReviewId;
            var isHidden = true;

            // Act
            repository.UpdateReviewVisibility(reviewId, isHidden);

            // Assert
            var updatedReview = repository.GetReviewById(reviewId);
            Assert.NotNull(updatedReview);
            Assert.Equal(isHidden, updatedReview.IsHidden);
        }

        [Fact]
        public void UpdateNumberOfFlagsForReview_UpdatesNumberOfFlags()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var allReviews = repository.GetAllReviews();
            var reviewId = allReviews.First().ReviewId;
            var numberOfFlags = 5;

            // Act
            repository.UpdateNumberOfFlagsForReview(reviewId, numberOfFlags);

            // Assert
            var updatedReview = repository.GetReviewById(reviewId);
            Assert.NotNull(updatedReview);
            Assert.Equal(numberOfFlags, updatedReview.NumberOfFlags);
        }

        [Fact]
        public void AddReview_ReturnsNewId()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var allReviews = repository.GetAllReviews();
            var maxId = allReviews.Max(review => review.ReviewId);
            var newReview = new Review(
                reviewId: 0, // This will be replaced by the repository
                userId: 5,
                rating: 4,
                content: "New review",
                createdDate: DateTime.Now
            );

            // Act
            var newId = repository.AddReview(newReview);

            // Assert
            Assert.True(newId > maxId);
            var addedReview = repository.GetReviewById(newId);
            Assert.NotNull(addedReview);
            Assert.Equal(newReview.UserId, addedReview.UserId);
            Assert.Equal(newReview.Rating, addedReview.Rating);
            Assert.Equal(newReview.Content, addedReview.Content);
        }

        [Fact]
        public void RemoveReviewById_RemovesReview()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var allReviews = repository.GetAllReviews();
            var reviewId = allReviews.First().ReviewId;

            // Act
            var result = repository.RemoveReviewById(reviewId);

            // Assert
            Assert.True(result);
            var removedReview = repository.GetReviewById(reviewId);
            Assert.Null(removedReview);
        }

        [Fact]
        public void RemoveReviewById_ReturnsFalseForNonExistentId()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var nonExistentId = 9999;

            // Act
            var result = repository.RemoveReviewById(nonExistentId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetHiddenReviews_ReturnsHiddenReviews()
        {
            // Arrange
            var repository = new ReviewsRepository();
            var allReviews = repository.GetAllReviews();
            var reviewId = allReviews.First().ReviewId;
            repository.UpdateReviewVisibility(reviewId, true);

            // Act
            var hiddenReviews = repository.GetHiddenReviews();

            // Assert
            Assert.NotNull(hiddenReviews);
            Assert.Contains(hiddenReviews, review => review.ReviewId == reviewId);
            Assert.All(hiddenReviews, review => Assert.True(review.IsHidden));
        }
    }
}
