using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.FastTree;

namespace App1.Ai_Check
{
    public class ReviewModelTrainer
    {
        private static readonly string ProjectRoot = GetProjectRoot();

        private static readonly string DataPath = Path.Combine(ProjectRoot, "Ai_Check", "review_data.csv");
        private static readonly string ModelPath = Path.Combine(ProjectRoot, "Models", "curseword_model.zip");
        private static readonly string LogPath = Path.Combine(ProjectRoot, "Logs", "training_log.txt");

        private static string GetProjectRoot([CallerFilePath] string filePath = "")
        {
            var dir = new FileInfo(filePath).Directory;
            while (dir != null && !dir.GetFiles("*.csproj").Any())
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new Exception("Project root not found!");
        }

        public bool TrainModel()
        {
            Console.WriteLine($"Project root: {ProjectRoot}");
            Console.WriteLine($"Looking for data at: {DataPath}");

            if (!File.Exists(DataPath))
            {
                LogToFile($"ERROR: Missing file at {DataPath}");
                return false;
            }

            try
            {
                // Ensure directories exist
                Directory.CreateDirectory(Path.GetDirectoryName(ModelPath));
                Directory.CreateDirectory(Path.GetDirectoryName(LogPath));

                // Initialize MLContext
                var mlContext = new MLContext(seed: 0);

                // Load data (ensure separator matches your CSV)
                var data = mlContext.Data.LoadFromTextFile<ReviewData>(
                    path: DataPath,
                    separatorChar: '}',
                    hasHeader: true
                );

                // Define pipeline with FastTreeClassifier
                var pipeline = mlContext.Transforms.Text.FeaturizeText(
                    outputColumnName: "Features",
                    inputColumnName: nameof(ReviewData.Text))
                .Append(mlContext.BinaryClassification.Trainers.FastTree(
                    labelColumnName: nameof(ReviewData.IsOffensive),
                    featureColumnName: "Features",
                    numberOfTrees: 100,        // Number of trees to use
                    numberOfLeaves: 50,        // Number of leaves per tree
                    minimumExampleCountPerLeaf: 10,  // Minimum examples in a leaf
                    learningRate: 0.1         // Learning rate
                ));

                // Split data into training and testing sets
                var trainTestSplit = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

                // Train the model
                var model = pipeline.Fit(trainTestSplit.TrainSet);

                // Evaluate the model on the test set
                var predictions = model.Transform(trainTestSplit.TestSet);

                // Convert predictions to a list for easy comparison
                var predictedResults = mlContext.Data.CreateEnumerable<ReviewPrediction>(predictions, reuseRowObject: false).ToList();
                var actualResults = mlContext.Data.CreateEnumerable<ReviewData>(trainTestSplit.TestSet, reuseRowObject: false).ToList();

                // Compare predictions with actual values and log mistakes
                for (int i = 0; i < predictedResults.Count; i++)
                {
                    var prediction = predictedResults[i];
                    var actual = actualResults[i];

                    // If the prediction is incorrect, log it
                    if (prediction.Prediction != actual.IsOffensive)
                    {
                        LogToFile($"Mistake in Review {i + 1}: Predicted {prediction.Prediction}, Actual {actual.IsOffensive}. Text: {actual.Text}");
                    }
                }

                // Save the trained model
                mlContext.Model.Save(model, data.Schema, ModelPath);

                LogToFile($"Model trained and saved successfully at {ModelPath}");
                return true;
            }
            catch (Exception ex)
            {
                LogToFile($"ERROR: {ex.Message}");
                return false;
            }
        }

        private static void LogToFile(string message)
        {
            try
            {
                File.AppendAllText(LogPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}\n");
            }
            catch
            {
                // Fallback to console if logging fails
                Console.WriteLine($"LOG FAILED: {message}");
            }
        }
    }
}
