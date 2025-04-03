using System.Collections.Generic;
using System.Linq;
using App1.Models;

namespace App1.Repositories
{
    internal class ReviewsRepo
    {
        private List<Review> reviews;

        public ReviewsRepo()
        {
            reviews = new List<Review>();
        }

        public void generateReviews()
        {
            reviews.Add(new Review(1, 0, "Berea e buna", false, 1));
            reviews.Add(new Review(2, 1, "Vinul are un gust excelent", false, 2));
            reviews.Add(new Review(3, 0, "Cocktailul e prea dulce pentru mine", false, 3));
            reviews.Add(new Review(4, 2, "Whiskey-ul are o aromă puternică", false, 4));
            reviews.Add(new Review(5, 0, "Ceaiul verde e foarte revigorant", false, 5));
            reviews.Add(new Review(6, 0, "Cafeaua de aici e extraordinară", false, 6));
            reviews.Add(new Review(7, 1, "Limonada e prea acră", false, 7));
            reviews.Add(new Review(8, 0, "Smoothie-ul e cremos și delicios", false, 8));
            reviews.Add(new Review(9, 1, "Energizantul mi-a dat un boost de energie", false, 9));
            reviews.Add(new Review(10, 0, "Sucul natural e proaspăt și răcoritor", false, 10));
            reviews.Add(new Review(11, 0, "Laptele de migdale e o alegere bună pentru vegani", false, 10));
        }


        public List<Review> GetReviews()
        {
            return reviews;
        }

        public List<Review> GetReviewsByUser(int userID)
        {
            return reviews.Where(r => r.userID == userID).ToList();
        }

        public void UpdateNumberOfFlags(int reviewID, int numberOfFlags)
        {
            var review = reviews.FirstOrDefault(r => r.reviewID == reviewID);
            if (review != null)
            {
                review.numberOfFlags = numberOfFlags;
            }
        }

        public void UpdateHiddenReview(int reviewID, bool isHidden)
        {
            var review = reviews.FirstOrDefault(r => r.reviewID == reviewID);
            if (review != null)
            {
                review.isHidden = isHidden;
            }
        }

        public Review GetReviewByID(int reviewID)
        {
            return reviews.FirstOrDefault(r => r.reviewID == reviewID);
        }
    }
}
