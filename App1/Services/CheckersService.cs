using System;
using App1.Models;
using App1.Repositories;
using App1.Services; // Assuming you have ReviewsService available
using Microsoft.ML;
using System.IO;

namespace App1.Services
{
    internal class CheckersService
    {
        private readonly ReviewsRepo reviewsRepo;
        private readonly ReviewsService reviewsService;
        private static readonly string ModelPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "curseword_model.zip");

        public CheckersService()
        {
            reviewsRepo = new ReviewsRepo();
            reviewsService = new ReviewsService(); // Initialize the ReviewsService
        }

        public void RunAutoCheck(int reviewID)
        {
            // Implement the logic for automatic checks on the review with the given reviewID
            var review = reviewsRepo.GetReviewByID(reviewID);
            if (review != null)
            {
                // Perform some automatic check (this can be your custom logic)
                
            }
            else
            {
                Console.WriteLine("Review not found.");
            }
        }

        public void RunAICheck(int reviewID)
        {
            // Get the specific review from the repository by ID
            var review = reviewsRepo.GetReviewByID(reviewID);
            if (review != null)
            {
                // Perform AI-based check
                bool isOffensive = CheckReviewWithAI(review.content); // Use ReviewText getter

                // If the review is offensive, hide it
                if (isOffensive)
                {
                    Console.WriteLine($"Review {reviewID} is offensive. Hiding the review.");
                    reviewsService.HideReview(reviewID); // Hide the review
                }
                else
                {
                    Console.WriteLine($"Review {reviewID} is not offensive.");
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

            // Load the trained model
            ITransformer model = context.Model.Load(ModelPath, out var modelInputSchema);

            // Create the prediction engine
            var predEngine = context.Model.CreatePredictionEngine<ReviewData, ReviewPrediction>(model);

            // Create a new review object with the provided text
            var review = new ReviewData { Text = reviewText };

            // Make the prediction
            var prediction = predEngine.Predict(review);

            // Return whether the review is offensive
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
