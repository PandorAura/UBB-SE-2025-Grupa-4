using Microsoft.ML.Data;

namespace App1.Ai_Check
{
    public class ReviewData
    {
        [LoadColumn(0)]
        public string Text { get; set; } // Review text

        [LoadColumn(1)]
        [ColumnName("IsOffensive")] // Label for ML.NET training
        public bool IsOffensive { get; set; } // 1 = offensive, 0 = not offensive
    }

    public class ReviewPrediction
    {
        [ColumnName("PredictedLabel")]
        public float Prediction { get; set; } // Model’s prediction: 1 (offensive) or 0 (not offensive)
    }
}
