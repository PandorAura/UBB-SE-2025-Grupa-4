using System;
using App1.Models;
using App1.Repositories;
using App1.Services; 
using Microsoft.ML;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using App1.AutoChecker;

namespace App1.Services
{
    internal class CheckersService
    {
        private readonly ReviewRepo reviewsRepo;
        private readonly IReviewService reviewsService;
        public readonly AutoCheck autoCheck;
        private static readonly string ModelPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "curseword_model.zip");

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

        public void RunAICheck(int reviewID)
        {
            ////get the specific review from the repository by ID
            //var review = reviewsService.GetReviewsByUser(userID);
            //if (review != null)
            //{
            //    // perform AI-based check
            //    bool isOffensive = CheckReviewWithAI(review.content);

            //    // if the review is offensive, hide it
            //    if (isOffensive)
            //    {
            //        Console.WriteLine($"Review {reviewID} is offensive. Hiding the review.");
            //        reviewsService.HideReview(reviewID); // hide the review
            //    }
            //    else
            //    {
            //        Console.WriteLine($"Review {reviewID} is not offensive.");
            //    }
            //}
            //else
            //{
            //    Console.WriteLine("Review not found.");
            //}
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
