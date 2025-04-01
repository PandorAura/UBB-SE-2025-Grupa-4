using System;
using App1.Models;
using App1.Repositories;
using App1.Services; 
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
            reviewsService = new ReviewsService();
        }

        public void RunAutoCheck(int reviewID)
        {
            var review = reviewsRepo.GetReviewByID(reviewID);
            if (review != null)
            {
                // partea de auto
                
            }
            else
            {
                Console.WriteLine("Review not found.");
            }
        }

        public void RunAICheck(int reviewID)
        {
            // get the specific review from the repository by ID
            var review = reviewsRepo.GetReviewByID(reviewID);
            if (review != null)
            {
                // perform AI-based check
                bool isOffensive = CheckReviewWithAI(review.content); 

                // if the review is offensive, hide it
                if (isOffensive)
                {
                    Console.WriteLine($"Review {reviewID} is offensive. Hiding the review.");
                    reviewsService.HideReview(reviewID); // hide the review
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
