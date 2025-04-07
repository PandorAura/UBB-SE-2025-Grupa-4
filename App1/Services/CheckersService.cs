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

namespace App1.Services
{
    internal class CheckersService
    {
        private readonly ReviewRepo reviewsRepo;
        private readonly IReviewService reviewsService;
        private readonly AutoCheck autoCheck;
        private static readonly string ModelPath = Path.Combine(GetProjectRoot(), "Logs", "curseword_model.zip");

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
            ReviewModelTrainer rmt = new ReviewModelTrainer();
            rmt.TrainModel();
            //get the specific review from the repository by ID
            if (review != null)
            {
                // perform AI-based check
                bool isOffensive = CheckReviewWithAI(review.Content);

                // if the review is offensive, hide it
                if (isOffensive)
                {
                    Console.WriteLine($"Review {review.ReviewID} is offensive. Hiding the review.");
                    reviewsService.HideReview(review.ReviewID); // hide the review
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
            var context = new MLContext();

            // load the trained model
            ITransformer model = context.Model.Load(ModelPath, out var modelInputSchema);

            // create the prediction engine
            var predEngine = context.Model.CreatePredictionEngine<ReviewData, ReviewPrediction>(model);

            // create a new review object with the provided text
            var review = new ReviewData { Text = reviewText };

            // make the prediction
            var prediction = predEngine.Predict(review);

            // return whether the review is offensive
            return prediction.IsOffensive;
        }
    }

    public class ReviewData
    {
        public string Text { get; set; }
    }

    public class ReviewPrediction
    {
        public bool IsOffensive { get; set; }
    }

}
