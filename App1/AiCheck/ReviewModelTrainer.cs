using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.FastTree;

namespace App1.AiCheck
{
    /// <summary>
    /// Handles the training and evaluation of the review content classification model.
    /// </summary>
    public class ReviewModelTrainer
    {
        // File paths
        private static readonly string ProjectRoot = GetProjectRoot();
        private static readonly string DataPath = Path.Combine(ProjectRoot, "AiCheck", "review_data.csv");
        private static readonly string ModelPath = Path.Combine(ProjectRoot, "Models", "curseword_model.zip");
        private static readonly string LogPath = Path.Combine(ProjectRoot, "Logs", "training_log.txt");

        // Model training parameters
        private const int NumberOfTrees = 100;
        private const int NumberOfLeaves = 50;
        private const int MinimumExampleCountPerLeaf = 10;
        private const float LearningRate = 0.1f;
        private const float TestFraction = 0.2f;
        private const char CsvSeparator = '}';

        /// <summary>
        /// Gets the project root directory by traversing up from the current file.
        /// </summary>
        /// <param name="filePath">The path of the current file (automatically provided by the compiler).</param>
        /// <returns>The full path to the project root directory.</returns>
        /// <exception cref="Exception">Thrown when the project root cannot be found.</exception>
        private static string GetProjectRoot([CallerFilePath] string filePath = "")
        {
            var directory = new FileInfo(filePath).Directory;
            while (directory != null && !directory.GetFiles("*.csproj").Any())
            {
                directory = directory.Parent;
            }
            return directory?.FullName ?? throw new Exception("Project root not found!");
        }

        /// <summary>
        /// Trains a binary classification model to detect offensive content in reviews.
        /// </summary>
        /// <returns>True if training was successful, false otherwise.</returns>
        public static bool TrainModel()
        {
            LogToFile($"Starting model training process. Project root: {ProjectRoot}");
            LogToFile($"Looking for training data at: {DataPath}");

            if (!File.Exists(DataPath))
            {
                LogToFile($"ERROR: Missing training data file at {DataPath}");
                return false;
            }

            try
            {
                // Ensure directories exist
                EnsureDirectoriesExist();

                // Initialize MLContext with a fixed seed for reproducibility
                var machineLearningContext = new MLContext(seed: 0);

                // Load and prepare the training data
                var trainingData = LoadTrainingData(machineLearningContext);

                // Create and configure the model pipeline
                var modelPipeline = CreateModelPipeline(machineLearningContext);

                // Split data into training and testing sets
                var trainTestSplit = machineLearningContext.Data.TrainTestSplit(trainingData, testFraction: TestFraction);

                // Train the model
                var trainedModel = modelPipeline.Fit(trainTestSplit.TrainSet);

                // Evaluate the model on the test set
                EvaluateModel(machineLearningContext, trainedModel, trainTestSplit.TestSet);

                // Save the trained model
                machineLearningContext.Model.Save(trainedModel, trainingData.Schema, ModelPath);

                LogToFile($"Model trained and saved successfully at {ModelPath}");
                return true;
            }
            catch (FileNotFoundException fileNotFoundException)
            {
                LogToFile($"File not found error: {fileNotFoundException.Message}");
                return false;
            }
            catch (InvalidOperationException invalidOperationException)
            {
                LogToFile($"Invalid operation error: {invalidOperationException.Message}");
                return false;
            }
            catch (Exception exception)
            {
                LogToFile($"Unexpected error during model training: {exception.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ensures that all required directories exist.
        /// </summary>
        private static void EnsureDirectoriesExist()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ModelPath));
            Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
        }

        /// <summary>
        /// Loads the training data from the CSV file.
        /// </summary>
        /// <param name="machineLearningContext">The ML.NET context.</param>
        /// <returns>The loaded training data.</returns>
        private static IDataView LoadTrainingData(MLContext machineLearningContext)
        {
            return machineLearningContext.Data.LoadFromTextFile<ReviewData>(
                path: DataPath,
                separatorChar: CsvSeparator,
                hasHeader: true
            );
        }

        /// <summary>
        /// Creates the model pipeline for training.
        /// </summary>
        /// <param name="machineLearningContext">The ML.NET context.</param>
        /// <returns>The configured model pipeline.</returns>
        private static IEstimator<ITransformer> CreateModelPipeline(MLContext machineLearningContext)
        {
            return machineLearningContext.Transforms.Text.FeaturizeText(
                outputColumnName: "Features",
                inputColumnName: nameof(ReviewData.ReviewContent))
            .Append(machineLearningContext.BinaryClassification.Trainers.FastTree(
                labelColumnName: nameof(ReviewData.IsOffensiveContent),
                featureColumnName: "Features",
                numberOfTrees: NumberOfTrees,
                numberOfLeaves: NumberOfLeaves,
                minimumExampleCountPerLeaf: MinimumExampleCountPerLeaf,
                learningRate: LearningRate
            ));
        }

        /// <summary>
        /// Evaluates the trained model on the test dataset.
        /// </summary>
        /// <param name="machineLearningContext">The ML.NET context.</param>
        /// <param name="trainedModel">The trained model.</param>
        /// <param name="testData">The test dataset.</param>
        private static void EvaluateModel(MLContext machineLearningContext, ITransformer trainedModel, IDataView testData)
        {
            // Transform the test data using the trained model
            var predictions = trainedModel.Transform(testData);

            // Convert predictions to a list for easy comparison
            var predictedResults = machineLearningContext.Data.CreateEnumerable<ReviewPrediction>(predictions, reuseRowObject: false).ToList();
            var actualResults = machineLearningContext.Data.CreateEnumerable<ReviewData>(testData, reuseRowObject: false).ToList();

            // Compare predictions with actual values and log mistakes
            int correctPredictions = 0;
            int totalPredictions = predictedResults.Count;

            for (int index = 0; index < predictedResults.Count; index++)
            {
                var prediction = predictedResults[index];
                var actual = actualResults[index];

                // If the prediction is incorrect, log it
                if (prediction.IsPredictedOffensive != actual.IsOffensiveContent)
                {
                    LogToFile($"Mistake in Review {index + 1}: Predicted {prediction.IsPredictedOffensive}, Actual {actual.IsOffensiveContent}. Text: {actual.ReviewContent}");
                }
                else
                {
                    correctPredictions++;
                }
            }

            // Log overall accuracy
            float accuracy = (float)correctPredictions / totalPredictions * 100;
            LogToFile($"Model evaluation complete. Accuracy: {accuracy:F2}% ({correctPredictions}/{totalPredictions} correct predictions)");
        }

        /// <summary>
        /// Logs a message to the log file with a timestamp.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private static void LogToFile(string message)
        {
            try
            {
                string timestampedMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                File.AppendAllText(LogPath, timestampedMessage + Environment.NewLine);
                Console.WriteLine(timestampedMessage);
            }
            catch (Exception exception)
            {
                // Fallback to console if logging fails
                Console.WriteLine($"LOG FAILED: {message}. Error: {exception.Message}");
            }
        }
    }
}
