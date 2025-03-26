using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Models
{
    public class WeeklyReport
    {
        public int WeeklyReportId { get; }

        public DateOnly ReportDate { get; set; }

        public int NumberOfActiveUsers { get; set; }

        public int NumbrOfBannedUsers { get; set; }

        public int NumberOfNewReviews {  get; set; }

        public float AverageRating { get; set; }


    }
}
