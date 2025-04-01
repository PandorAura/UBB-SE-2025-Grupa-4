using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    public class WeeklyReport
    {
        public int weeklyReportId { get; }

        public DateOnly reportDate { get; set; }

        public int numberOfActiveUsers { get; set; }

        public int numbrOfBannedUsers { get; set; }

        public int numberOfNewReviews {  get; set; }

        public float averageRating { get; set; }


    }
}
