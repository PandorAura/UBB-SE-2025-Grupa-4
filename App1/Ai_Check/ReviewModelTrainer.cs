using System;
using Microsoft.ML;
using Microsoft.ML.Data;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace App1.Ai_Check
{
    public class ReviewModelTrainer
    {
        private static readonly string DataPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.Parent.FullName,"Ai_Check", "review_data.csv");
        private static readonly string ModelPath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.Parent.FullName, "Logs", "curseword_model.zip");
        private static readonly string LogFilePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent.Parent.FullName, "Logs", "training_log.txt");

        public static void TrainModel()
        {
            var context = new MLContext(seed: 0);

            // load the data from csv file
            var data = context.Data.LoadFromTextFile<ReviewData>(
                path: DataPath,
                separatorChar: '}',
                hasHeader: true,
                trimWhitespace: true);

            // pipeline with text featurization and logistic regression
            var pipeline = context.Transforms.Text.FeaturizeText("Features", nameof(ReviewData.Text))
                .Append(context.BinaryClassification.Trainers.SdcaLogisticRegression(
                    labelColumnName: nameof(ReviewData.IsOffensive),
                    featureColumnName: "Features",
                    maximumNumberOfIterations: 100,
                    l2Regularization: (float?)0.001));

            // cross-validation (in folds)
            var crossValidationResults = context.BinaryClassification.CrossValidate(data, pipeline, numberOfFolds: 5, labelColumnName: nameof(ReviewData.IsOffensive));


            // log the metrics from each fold
            foreach (var fold in crossValidationResults)
            {
                LogToFile($"Fold Accuracy: {fold.Metrics.Accuracy:P2}");
                LogToFile($"Fold Precision: {fold.Metrics.PositivePrecision:P2}");
                LogToFile($"Fold Recall: {fold.Metrics.PositiveRecall:P2}");
                LogToFile($"Fold F1 Score: {fold.Metrics.F1Score:P2}");
            }

            // calculate and log average metrics
            var averageAccuracy = crossValidationResults.Average(result => result.Metrics.Accuracy);
            var averagePrecision = crossValidationResults.Average(result => result.Metrics.PositivePrecision);
            var averageRecall = crossValidationResults.Average(result => result.Metrics.PositiveRecall);
            var averageF1Score = crossValidationResults.Average(result => result.Metrics.F1Score);

            LogToFile($"Average Accuracy: {averageAccuracy:P2}");
            LogToFile($"Average Precision: {averagePrecision:P2}");
            LogToFile($"Average Recall: {averageRecall:P2}");
            LogToFile($"Average F1 Score: {averageF1Score:P2}");

            // save the final model
            var finalModel = pipeline.Fit(data);
            context.Model.Save(finalModel, data.Schema, ModelPath);

            LogToFile("Model training with cross-validation complete.");
        }

        private static void LogToFile(string message)
        {
                try
                {
                    // append the message to the log file
                    using (StreamWriter writer = new StreamWriter(LogFilePath, append: true))
                    {
                        writer.WriteLine($"{DateTime.Now}: {message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"error writing to log file: {ex.Message}");
                }
        }
       
    }
}
