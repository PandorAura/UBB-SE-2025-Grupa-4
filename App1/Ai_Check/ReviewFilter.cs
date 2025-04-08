using Microsoft.ML.Data;

namespace App1.Ai_Check
{
    public class ReviewData
    {
        [LoadColumn(0)]
        public string Text { get; set; }  // Review text

        [LoadColumn(1)]
        [ColumnName("IsOffensive")]  // ML.NET expects "Label" for binary classification
        public bool IsOffensive { get; set; }  // True = offensive, False = not offensive
    }

    public class ReviewPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }  // Direct bool output (more intuitive)

        [ColumnName("Score")]
        public float Probability { get; set; }  // Raw score for threshold tuning
    }
}