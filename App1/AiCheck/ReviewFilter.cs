using Microsoft.ML.Data;

namespace App1.AiCheck
{
    /// <summary>
    /// Represents a review data point for training the offensive content detection model.
    /// </summary>
    public class ReviewData
    {
        /// <summary>
        /// The text content of the review.
        /// </summary>
        [LoadColumn(0)]
        [ColumnName("ReviewContent")]
        public string ReviewContent { get; set; }

        /// <summary>
        /// Indicates whether the review contains offensive content.
        /// </summary>
        [LoadColumn(1)]
        [ColumnName("IsOffensiveContent")]
        public bool IsOffensiveContent { get; set; }
    }

    /// <summary>
    /// Represents the prediction output from the offensive content detection model.
    /// </summary>
    public class ReviewPrediction
    {
        /// <summary>
        /// The predicted classification (true = offensive, false = not offensive).
        /// </summary>
        [ColumnName("PredictedLabel")]
        public bool IsPredictedOffensive { get; set; }

        /// <summary>
        /// The probability score indicating the confidence of the offensive content prediction.
        /// </summary>
        [ColumnName("Score")]
        public float OffensiveProbabilityScore { get; set; }
    }
}