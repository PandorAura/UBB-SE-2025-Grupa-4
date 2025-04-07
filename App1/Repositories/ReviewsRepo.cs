using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App1.Models;

namespace App1.Repositories
{
    public class ReviewRepo : IReviewRepository
    {
        private List<Review> _reviews = new()
        {
            new Review(
                reviewId: 1,
                userId: 1,
                userName: "Admin One",
                rating: 5,
                content: "Terrible mix, a complete mess",
                createdDate: DateTime.Now.AddHours(-1),
                numberOfFlags: 1,
                isHidden: false
            ),
            new Review(
                reviewId: 2,
                userId: 3,
                userName: "Regular User",
                rating: 4,
                content: "Good experience",
                createdDate: DateTime.Now.AddHours(-5),
                numberOfFlags: 0,
                isHidden: false
            ),
            new Review(
                reviewId: 3,
                userId: 1,
                userName: "Regular User",
                rating: 2,
                content: "Such a bitter aftertaste",
                createdDate: DateTime.Now.AddDays(-1),
                numberOfFlags: 3,
                isHidden: false
            ),
            new Review(
                reviewId: 4,
                userId: 2,
                userName: "Admin Two",
                rating: 5,
                content: "Excellent!",
                createdDate: DateTime.Now.AddDays(-2),
                numberOfFlags: 1,
                isHidden: false
            ),
            new Review(
                reviewId: 5,
                userId: 3,
                userName: "Admin Two",
                rating: 5,
                content: "dunce",
                createdDate: DateTime.Now.AddDays(-2),
                numberOfFlags: 1,
                isHidden: false
            )
        };

        public List<Review> GetReviewsSince(DateTime date)
        {
            var result = _reviews
                .Where(r => r.CreatedDate >= date && !r.IsHidden)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
            return result;
        }

        public double GetAverageRating()
        {
            if (!_reviews.Any(r => !r.IsHidden))
                return (0.0);

            var average = _reviews
                .Where(r => !r.IsHidden)
                .Average(r => r.Rating);
            return Math.Round(average, 1);
        }

        public List<Review> GetRecentReviews(int count)
        {
            var result = _reviews
                .Where(r => !r.IsHidden)
                .OrderByDescending(r => r.CreatedDate)
                .Take(count)
                .ToList();
            return result;
        }

        public int GetReviewCountSince(DateTime date)
        {
            var count = _reviews
                .Count(r => r.CreatedDate >= date && !r.IsHidden);
            return count;
        }

        public List<Review> GetFlaggedReviews(int minFlags)
        {
            var result = _reviews
                .Where(r => r.NumberOfFlags >= minFlags && !r.IsHidden)
                .ToList();
            return result;
        }

        public void generateReviews()
        {
            //reviews.Add(new Review(1, 0, "Berea e buna", false, 1));
            //reviews.Add(new Review(2, 1, "Vinul are un gust excelent", false, 2));
            //reviews.Add(new Review(3, 0, "Cocktailul e prea dulce pentru mine", false, 3));
            //reviews.Add(new Review(4, 2, "Whiskey-ul are o aromă puternică", false, 4));
            //reviews.Add(new Review(5, 0, "Ceaiul verde e foarte revigorant", false, 5));
            //reviews.Add(new Review(6, 0, "Cafeaua de aici e extraordinară", false, 6));
            //reviews.Add(new Review(7, 1, "Limonada e prea acră", false, 7));
            //reviews.Add(new Review(8, 0, "Smoothie-ul e cremos și delicios", false, 8));
            //reviews.Add(new Review(9, 1, "Energizantul mi-a dat un boost de energie", false, 9));
            //reviews.Add(new Review(10, 0, "Sucul natural e proaspăt și răcoritor", false, 10));
            //reviews.Add(new Review(11, 0, "Laptele de migdale e o alegere bună pentru vegani", false, 10));
        }


        public List<Review> GetReviews()
        {
            return _reviews;
        }

        public void UpdateHiddenReview(int reviewID, bool isHidden)
        {
            var review = _reviews.FirstOrDefault(r => r.ReviewID == reviewID);

            if(review != null)
            {
                review.IsHidden = isHidden;
            }
        }

        public void UpdateFlaggedReview(int reviewID, int numberOfFlags)
        {
            _reviews[reviewID].NumberOfFlags = numberOfFlags;
        }

        public List<Review> GetReviewsByUser(int userId)
        {
            return _reviews
                .Where(r => r.UserID == userId && !r.IsHidden)
                .OrderByDescending(r => r.CreatedDate)
                .ToList();
        }

        public Review GetReviewByID(int reviewID)
        {
            return _reviews.FirstOrDefault(r => r.ReviewID == reviewID);
        }
    }
}