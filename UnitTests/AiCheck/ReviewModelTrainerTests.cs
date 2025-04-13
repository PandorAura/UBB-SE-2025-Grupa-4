using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.AiCheck
{
    public class ReviewModelTrainerTests
    {
        private static readonly string TestDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "review_data.csv");
        private static readonly string TestModelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "test_model.zip");
        private static readonly string TestLogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", "test_training_log.txt");

        [Fact]
        public async Task TrainModel_WithValidData_ShouldSucceed()
        {
            // Arrange
            EnsureTestDirectoriesExist();
            CreateTestDataFile();

            // Act
            bool result = App1.AiCheck.ReviewModelTrainer.TrainModel(TestDataPath, TestModelPath, TestLogPath);

            // Assert
            if (!result)
            {
                // If the test fails, read and display the log file
                if (File.Exists(TestLogPath))
                {
                    string logContent = File.ReadAllText(TestLogPath);
                    Console.WriteLine("Training failed. Log contents:");
                    Console.WriteLine(logContent);
                }
                else
                {
                    Console.WriteLine("Training failed but no log file was created.");
                }
            }

            Assert.True(result);
            Assert.True(File.Exists(TestModelPath));
            Assert.True(File.Exists(TestLogPath));

            // Cleanup
            CleanupTestFiles();
        }

        [Fact]
        public async Task TrainModel_WithInvalidData_ShouldFail()
        {
            // Arrange
            EnsureTestDirectoriesExist();
            CreateInvalidTestDataFile();

            // Act
            bool result = App1.AiCheck.ReviewModelTrainer.TrainModel(TestDataPath, TestModelPath, TestLogPath);

            // Assert
            if (result)
            {
                // If the test fails, read and display the log file
                if (File.Exists(TestLogPath))
                {
                    string logContent = File.ReadAllText(TestLogPath);
                    Console.WriteLine("Test failed - training succeeded when it should have failed. Log contents:");
                    Console.WriteLine(logContent);
                }
            }

            Assert.False(result);
            Assert.False(File.Exists(TestModelPath));

            // Cleanup
            CleanupTestFiles();
        }

        private static void EnsureTestDirectoriesExist()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(TestDataPath));
            Directory.CreateDirectory(Path.GetDirectoryName(TestModelPath));
            Directory.CreateDirectory(Path.GetDirectoryName(TestLogPath));
        }

        private static void CreateTestDataFile()
        {
            var csvContent = new StringBuilder();
            csvContent.AppendLine("ReviewContent}IsOffensiveContent");
            csvContent.AppendLine("This is a great drink!}0");
            csvContent.AppendLine("I love this cocktail, it's amazing!}0");
            csvContent.AppendLine("This is the worst drink ever!}0");
            csvContent.AppendLine("You're an idiot if you like this drink!}1");
            csvContent.AppendLine("This drink is f***ing terrible!}1");
            csvContent.AppendLine("I can't believe how bad this is, you moron!}1");

            File.WriteAllText(TestDataPath, csvContent.ToString());
        }

        private static void CreateInvalidTestDataFile()
        {
            var csvContent = new StringBuilder();
            csvContent.AppendLine("Invalid,Data,Format");
            csvContent.AppendLine("This,is,not,correct");

            File.WriteAllText(TestDataPath, csvContent.ToString());
        }

        private static void CleanupTestFiles()
        {
            if (File.Exists(TestDataPath))
            {
                File.Delete(TestDataPath);
            }

            if (File.Exists(TestModelPath))
            {
                File.Delete(TestModelPath);
            }

            if (File.Exists(TestLogPath))
            {
                File.Delete(TestLogPath);
            }
        }
    }
}