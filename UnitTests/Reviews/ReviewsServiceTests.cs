using App1.Models;
using App1.Repositories;
using App1.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTests.Reviews
{
    public class ReviewsServiceTests
    {
        private readonly Mock<IReviewsRepository> _mockRepository;
        private readonly ReviewsService _service;

        public ReviewsServiceTests()
        {
            _mockRepository = new Mock<IReviewsRepository>();
            _service = new ReviewsService(_mockRepository.Object);
        }

        [Fact]
        public void ResetReviewFlags_SetsNumberOfFlagsToZero()
        {
            // Arrange
            int reviewId = 1;
            var review = new Review(
                reviewId: reviewId,
                userId: 2,
                rating: 4,
                content: "Great drink!",
                createdDate: DateTime.Now,
                numberOfFlags: 3
            );
            _mockRepository.Setup(review => review.GetReviewById(reviewId)).Returns(review);

            // Act
            _service.ResetReviewFlags(reviewId);

            // Assert
            _mockRepository.Verify(review => review.UpdateNumberOfFlagsForReview(reviewId, 0), Times.Once);
        }

        [Fact]
        public void HideReview_SetsIsHiddenToTrue()
        {
            // Arrange
            int reviewId = 1;

            // Act
            _service.HideReview(reviewId);

            // Assert
            _mockRepository.Verify(review => review.UpdateReviewVisibility(reviewId, true), Times.Once);
        }

        [Fact]
        public void GetFlaggedReviews_ReturnsReviewsWithFlags()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review(1, 2, 4, "Great drink!", DateTime.Now, 1),
                new Review(2, 3, 5, "Amazing drink!", DateTime.Now, 0),
                new Review(3, 4, 3, "Good drink!", DateTime.Now, 2)
            };
            _mockRepository.Setup(review => review.GetAllReviews()).Returns(reviews);

            // Act
            var flaggedReviews = _service.GetFlaggedReviews();

            // Assert
            Assert.Equal(2, flaggedReviews.Count);
            Assert.All(flaggedReviews, review => Assert.True(review.NumberOfFlags > 0));
        }

        [Fact]
        public void GetHiddenReviews_ReturnsHiddenReviews()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review(1, 2, 4, "Great drink!", DateTime.Now, 0, true),
                new Review(2, 3, 5, "Amazing drink!", DateTime.Now, 0, false),
                new Review(3, 4, 3, "Good drink!", DateTime.Now, 0, true)
            };
            _mockRepository.Setup(review => review.GetAllReviews()).Returns(reviews);

            // Act
            var hiddenReviews = _service.GetHiddenReviews();

            // Assert
            Assert.Equal(2, hiddenReviews.Count);
            Assert.All(hiddenReviews, review => Assert.True(review.IsHidden));
        }

        [Fact]
        public void GetAllReviews_ReturnsAllReviews()
        {
            // Arrange
            var reviews = new List<Review>
            {
                new Review(1, 2, 4, "Great drink!", DateTime.Now),
                new Review(2, 3, 5, "Amazing drink!", DateTime.Now),
                new Review(3, 4, 3, "Good drink!", DateTime.Now)
            };
            _mockRepository.Setup(review => review.GetAllReviews()).Returns(reviews);

            // Act
            var allReviews = _service.GetAllReviews();

            // Assert
            Assert.Equal(3, allReviews.Count);
            _mockRepository.Verify(review => review.GetAllReviews(), Times.Once);
        }

        [Fact]
        public void GetReviewsSince_ReturnsReviewsAfterDate()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-2);
            var reviews = new List<Review>
            {
                new Review(1, 2, 4, "Great drink!", DateTime.Now.AddDays(-1)),
                new Review(2, 3, 5, "Amazing drink!", DateTime.Now.AddDays(-3)),
                new Review(3, 4, 3, "Good drink!", DateTime.Now)
            };
            _mockRepository.Setup(review => review.GetReviewsSince(date)).Returns(reviews.Where(review => review.CreatedDate >= date && !review.IsHidden).ToList());

            // Act
            var recentReviews = _service.GetReviewsSince(date);

            // Assert
            Assert.Equal(2, recentReviews.Count);
            Assert.All(recentReviews, review => Assert.True(review.CreatedDate >= date));
            _mockRepository.Verify(review => review.GetReviewsSince(date), Times.Once);
        }

        [Fact]
        public void GetAverageRatingForVisibleReviews_ReturnsCorrectAverage()
        {
            // Arrange
            double expectedAverage = 4.0;
            _mockRepository.Setup(review => review.GetAverageRatingForVisibleReviews()).Returns(expectedAverage);

            // Act
            var average = _service.GetAverageRatingForVisibleReviews();

            // Assert
            Assert.Equal(expectedAverage, average);
            _mockRepository.Verify(review => review.GetAverageRatingForVisibleReviews(), Times.Once);
        }

        [Fact]
        public void GetMostRecentReviews_ReturnsCorrectNumberOfReviews()
        {
            // Arrange
            int count = 2;
            var reviews = new List<Review>
            {
                new Review(1, 2, 4, "Great drink!", DateTime.Now.AddDays(-1)),
                new Review(2, 3, 5, "Amazing drink!", DateTime.Now),
                new Review(3, 4, 3, "Good drink!", DateTime.Now.AddDays(-2))
            };
            _mockRepository.Setup(review => review.GetMostRecentReviews(count)).Returns(reviews.OrderByDescending(review => review.CreatedDate).Take(count).ToList());

            // Act
            var recentReviews = _service.GetMostRecentReviews(count);

            // Assert
            Assert.Equal(2, recentReviews.Count);
            Assert.Equal(2, recentReviews[0].ReviewId); // Most recent
            Assert.Equal(1, recentReviews[1].ReviewId); // Second most recent
            _mockRepository.Verify(review => review.GetMostRecentReviews(count), Times.Once);
        }

        [Fact]
        public void GetReviewCountAfterDate_ReturnsCorrectCount()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-2);
            int expectedCount = 5;
            _mockRepository.Setup(review => review.GetReviewCountAfterDate(date)).Returns(expectedCount);

            // Act
            var count = _service.GetReviewCountAfterDate(date);

            // Assert
            Assert.Equal(expectedCount, count);
            _mockRepository.Verify(review => review.GetReviewCountAfterDate(date), Times.Once);
        }

        [Fact]
        public void GetReviewsByUser_ReturnsCorrectReviews()
        {
            // Arrange
            int userId = 2;
            var reviews = new List<Review>
            {
                new Review(1, 2, 4, "Great drink!", DateTime.Now),
                new Review(2, 3, 5, "Amazing drink!", DateTime.Now),
                new Review(3, 2, 3, "Good drink!", DateTime.Now)
            };
            _mockRepository.Setup(review => review.GetReviewsByUser(userId)).Returns(reviews.Where(review => review.UserId == userId && !review.IsHidden).OrderByDescending(review => review.CreatedDate).ToList());

            // Act
            var userReviews = _service.GetReviewsByUser(userId);

            // Assert
            Assert.Equal(2, userReviews.Count);
            Assert.All(userReviews, review => Assert.Equal(userId, review.UserId));
            _mockRepository.Verify(review => review.GetReviewsByUser(userId), Times.Once);
        }

        [Fact]
        public void GetReviewsForReport_ReturnsRecentReviews()
        {
            // Arrange
            var date = DateTime.Now.AddDays(-1);
            var reviews = new List<Review>
            {
                new Review(1, 2, 4, "Great drink!", DateTime.Now), // Should be MORE recent
                new Review(2, 3, 5, "Amazing drink!", DateTime.Now.AddMinutes(-5)), // Should be LESS recent
                new Review(3, 4, 3, "Good drink!", DateTime.Now.AddDays(-2))
            };

            _mockRepository.Setup(review => 
                review.GetReviewCountAfterDate(It.Is<DateTime>(d => d.Date == date.Date))).Returns(2);
            _mockRepository.Setup(review => 
                review.GetMostRecentReviews(2)).Returns(reviews.OrderByDescending(review => review.CreatedDate).Take(2).ToList());

            // Act
            var reportReviews = _service.GetReviewsForReport();

            // Assert
            Assert.Equal(2, reportReviews.Count);
            Assert.Equal(1, reportReviews[0].ReviewId); // Most recent
            Assert.Equal(2, reportReviews[1].ReviewId); // Second most recent
            _mockRepository.Verify(review => review.GetReviewCountAfterDate(It.Is<DateTime>(d => d.Date == date.Date)), Times.Once);
            _mockRepository.Verify(review => review.GetMostRecentReviews(2), Times.Once);
        }
    }
}
