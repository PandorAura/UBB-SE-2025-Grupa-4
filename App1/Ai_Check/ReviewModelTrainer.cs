using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Microsoft.ML;

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
                throw new FileNotFoundException($"Missing: {DataPath}");
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

                // Define pipeline
                var pipeline = mlContext.Transforms.Text.FeaturizeText(
                        outputColumnName: "Features",
                        inputColumnName: nameof(ReviewData.Text))
                    .Append(mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                        labelColumnName: "Label",  // Matches [ColumnName("Label")] in ReviewData
                        featureColumnName: "Features"
                    ));

                // Train and save model
                var model = pipeline.Fit(data);
                mlContext.Model.Save(model, data.Schema, ModelPath);

                LogToFile("Model trained and saved successfully.");
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