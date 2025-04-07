using System;
using App1.Models;
using App1.Repositories;
using App1.Services;
using Microsoft.ML;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using App1.AutoChecker;
using System.Runtime.CompilerServices;
using App1.Ai_Check;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace App1.Services
{
    internal class CheckersService
    {
        private static readonly string ProjectRoot = GetProjectRoot();
        private static readonly string LogPath = Path.Combine(ProjectRoot, "Logs", "training_log.txt");

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

        private readonly ReviewRepo reviewsRepo;
        private readonly IReviewService reviewsService;
        private readonly AutoCheck autoCheck;
        private static readonly string ModelPath = Path.Combine(GetProjectRoot(), "Models", "curseword_model.zip");

        private static string GetProjectRoot([CallerFilePath] string filePath = "")
        {
            var dir = new FileInfo(filePath).Directory;
            while (dir != null && !dir.GetFiles("*.csproj").Any())
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new Exception("Project root not found!");
        }

        public CheckersService(IReviewService reviewsService)
        {
            LogToFile(ModelPath);
            this.reviewsService = reviewsService;
            this.autoCheck = new AutoCheck();
        }

        public List<string> RunAutoCheck(List<Review> reviews)
        {
            List<string> messages = new List<string>();

            foreach (var review in reviews)
            {
                if (review != null)
                {
                    bool isOffensive = autoCheck.AutoCheckReview(review.Content);

                    if (isOffensive)
                    {
                        messages.Add($"Review {review.ReviewID} is offensive. Hiding the review.");
                        reviewsService.HideReview(review.ReviewID);
                        reviewsService.resetReviewFlags(review.ReviewID);
                    }
                    else
                    {
                        messages.Add($"Review {review.ReviewID} is not offensive.");
                    }
                }
                else
                {
                    messages.Add("Review not found.");
                }
            }
            return messages;
        }

        public void RunAICheck(Review review)
        {
            if (review != null)
            {
                bool isOffensive = CheckReviewWithAI(review.Content);

                if (isOffensive)
                {
                    Console.WriteLine($"Review {review.ReviewID} is offensive. Hiding the review.");
                    reviewsService.HideReview(review.ReviewID);
                    reviewsService.resetReviewFlags(review.ReviewID);
                }
                else
                {
                    Console.WriteLine($"Review {review.ReviewID} is not offensive.");
                }
            }
            else
            {
                Console.WriteLine("Review not found.");
            }
        }

        private bool CheckReviewWithAI(string reviewText)
        {
            
            var result = OffensiveTextDetector.DetectOffensiveContent(reviewText);
            Console.WriteLine("Hugging Face Response: " + result);

            float threshold = 0.1f;
            float score = GetConfidenceScore(result);
            return score >= threshold;
        }

        private float GetConfidenceScore(string result)
        {
            try
            {
                var outer = JsonConvert.DeserializeObject<List<List<Dictionary<string, object>>>>(result);
                var predictions = outer.FirstOrDefault();
                if (predictions != null)
                {
                    foreach (var item in predictions)
                    {
                        if (item.TryGetValue("label", out var labelObj) &&
                            labelObj.ToString().ToLower() == "hate" &&
                            item.TryGetValue("score", out var scoreObj))
                        {
                            return Convert.ToSingle(scoreObj);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Deserialization error: " + ex.Message);
            }

            return 0.0f; // default if parsing fails
        }


    }

    
    public static class OffensiveTextDetector
    {
        private static readonly string HuggingFaceApiUrl = "https://api-inference.huggingface.co/models/facebook/roberta-hate-speech-dynabench-r1-target";
        private static readonly string HuggingFaceApiToken = "INSERT YOUR OWN API KEY HERE from https://huggingface.co"; 

        public static string DetectOffensiveContent(string text)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {HuggingFaceApiToken}");

                var requestData = new { inputs = text };
                var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

                try
                {
                    var response = client.PostAsync(HuggingFaceApiUrl, jsonContent).GetAwaiter().GetResult();

                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    }
                    else
                    {
                        return $"Error: {response.StatusCode}";
                    }
                }
                catch (Exception ex)
                {
                    return $"Exception: {ex.Message}";
                }
            }
        }
    }

}
