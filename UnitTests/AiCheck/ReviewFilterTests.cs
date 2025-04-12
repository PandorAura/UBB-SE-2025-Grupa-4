using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xunit;
using App1.AiCheck;

namespace UnitTests.AiCheck
{
    public class ReviewFilterTests
    {
        [Fact]
        public void ReviewData_Constructor_SetsAllProperties()
        {
            // Arrange
            string reviewContent = "This is a great drink!";
            bool isOffensiveContent = false;

            // Act
            ReviewData reviewData = new ReviewData
            {
                ReviewContent = reviewContent,
                IsOffensiveContent = isOffensiveContent
            };

            // Assert
            Assert.Equal(reviewContent, reviewData.ReviewContent);
            Assert.Equal(isOffensiveContent, reviewData.IsOffensiveContent);
        }

        [Fact]
        public void ReviewData_HasLoadColumnAttribute()
        {
            // Arrange & Act
            var reviewContentProperty = typeof(ReviewData).GetProperty("ReviewContent");
            var isOffensiveContentProperty = typeof(ReviewData).GetProperty("IsOffensiveContent");

            // Assert
            Assert.NotNull(reviewContentProperty);
            Assert.NotNull(isOffensiveContentProperty);

            var reviewContentAttribute = reviewContentProperty.GetCustomAttributes(typeof(Microsoft.ML.Data.LoadColumnAttribute), false).FirstOrDefault();
            var isOffensiveContentAttribute = isOffensiveContentProperty.GetCustomAttributes(typeof(Microsoft.ML.Data.LoadColumnAttribute), false).FirstOrDefault();

            Assert.NotNull(reviewContentAttribute);
            Assert.NotNull(isOffensiveContentAttribute);
        }

        [Fact]
        public void ReviewData_HasColumnNameAttribute()
        {
            // Arrange & Act
            var reviewContentProperty = typeof(ReviewData).GetProperty("ReviewContent");
            var isOffensiveContentProperty = typeof(ReviewData).GetProperty("IsOffensiveContent");

            // Assert
            Assert.NotNull(reviewContentProperty);
            Assert.NotNull(isOffensiveContentProperty);

            var reviewContentAttribute = reviewContentProperty.GetCustomAttributes(typeof(Microsoft.ML.Data.ColumnNameAttribute), false).FirstOrDefault();
            var isOffensiveContentAttribute = isOffensiveContentProperty.GetCustomAttributes(typeof(Microsoft.ML.Data.ColumnNameAttribute), false).FirstOrDefault();

            Assert.NotNull(reviewContentAttribute);
            Assert.NotNull(isOffensiveContentAttribute);
        }

        [Fact]
        public void ReviewPrediction_Constructor_SetsAllProperties()
        {
            // Arrange
            bool isPredictedOffensive = true;
            float offensiveProbabilityScore = 0.85f;

            // Act
            ReviewPrediction reviewPrediction = new ReviewPrediction
            {
                IsPredictedOffensive = isPredictedOffensive,
                OffensiveProbabilityScore = offensiveProbabilityScore
            };

            // Assert
            Assert.Equal(isPredictedOffensive, reviewPrediction.IsPredictedOffensive);
            Assert.Equal(offensiveProbabilityScore, reviewPrediction.OffensiveProbabilityScore);
        }

        [Fact]
        public void ReviewPrediction_HasColumnNameAttribute()
        {
            // Arrange & Act
            var isPredictedOffensiveProperty = typeof(ReviewPrediction).GetProperty("IsPredictedOffensive");
            var offensiveProbabilityScoreProperty = typeof(ReviewPrediction).GetProperty("OffensiveProbabilityScore");

            // Assert
            Assert.NotNull(isPredictedOffensiveProperty);
            Assert.NotNull(offensiveProbabilityScoreProperty);

            var isPredictedOffensiveAttribute = isPredictedOffensiveProperty.GetCustomAttributes(typeof(Microsoft.ML.Data.ColumnNameAttribute), false).FirstOrDefault();
            var offensiveProbabilityScoreAttribute = offensiveProbabilityScoreProperty.GetCustomAttributes(typeof(Microsoft.ML.Data.ColumnNameAttribute), false).FirstOrDefault();

            Assert.NotNull(isPredictedOffensiveAttribute);
            Assert.NotNull(offensiveProbabilityScoreAttribute);
        }
    }
} 