using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using App1.Models;
using App1.Repositories;
using Xunit;

namespace UnitTests.EmailJobs
{
    public class AdminReportDataTests
    {
        DateTime ReportDate = DateTime.Now;
        List<User> AdminUsers = new List<User>();
        int ActiveUsersCount = 0;
        int BannedUsersCount = 0;
        int NewReviewsCount = 0;
        float AverageRating = 4.5f;
        List<Review> RecentReviews = new List<Review>();
        AdminReportData _reportData;

        [Fact]
        public void AdminReportData_WhenCreated_InitializesCorrectly()
        {
            // Arrange & Act
            _reportData = new AdminReportData(ReportDate, AdminUsers, ActiveUsersCount, BannedUsersCount, NewReviewsCount, AverageRating, RecentReviews);

            // Assert
            Assert.Equal(ReportDate, _reportData.ReportDate);
            Assert.Same(AdminUsers, _reportData.AdminUsers);
            Assert.Equal(ActiveUsersCount, _reportData.ActiveUsersCount);
            Assert.Equal(BannedUsersCount, _reportData.BannedUsersCount);
            Assert.Equal(NewReviewsCount, _reportData.NewReviewsCount);
            Assert.Equal(AverageRating, _reportData.AverageRating);
            Assert.Same(RecentReviews, _reportData.RecentReviews);
        }

        [Fact]
        public void AdminReportData_WithNonEmptyCollections_InitializesCorrectly()
        {
            // Arrange
            var adminUsers = new List<User>
            {
                new User(1, "admin@example.com", "Admin User", 0, false, UserRepo.AdminRoles )
            };

            var recentReviews = new List<Review>
            {
                new Review(1, 1, 4, "Great product", DateTime.Now)
            };

            // Act
            _reportData = new AdminReportData(ReportDate, adminUsers, 10, 5, 3, 4.2, recentReviews);

            // Assert
            Assert.Equal(ReportDate, _reportData.ReportDate);
            Assert.Same(adminUsers, _reportData.AdminUsers);
            Assert.Equal(10, _reportData.ActiveUsersCount);
            Assert.Equal(5, _reportData.BannedUsersCount);
            Assert.Equal(3, _reportData.NewReviewsCount);
            Assert.Equal(4.2, _reportData.AverageRating);
            Assert.Same(recentReviews, _reportData.RecentReviews);
            Assert.Single(_reportData.AdminUsers);
            Assert.Single(_reportData.RecentReviews);
        }

       
        [Fact]
        public void AdminReportData_ModifyingProperties_ChangesValues()
        {
            // Arrange
            _reportData = new AdminReportData(ReportDate, AdminUsers, ActiveUsersCount, BannedUsersCount, NewReviewsCount, AverageRating, RecentReviews);

            // Act
            var newDate = DateTime.Now.AddDays(1);
            var newUsers = new List<User> { new User(2, "new@example.com", "New User", 0, false, new List<Role>()) };
            var newReviews = new List<Review> { new Review(3, 2, 5, "New review", DateTime.Now) };

            _reportData.ReportDate = newDate;
            _reportData.AdminUsers = newUsers;
            _reportData.ActiveUsersCount = 100;
            _reportData.BannedUsersCount = 50;
            _reportData.NewReviewsCount = 25;
            _reportData.AverageRating = 3.7;
            _reportData.RecentReviews = newReviews;

            // Assert
            Assert.Equal(newDate, _reportData.ReportDate);
            Assert.Same(newUsers, _reportData.AdminUsers);
            Assert.Equal(100, _reportData.ActiveUsersCount);
            Assert.Equal(50, _reportData.BannedUsersCount);
            Assert.Equal(25, _reportData.NewReviewsCount);
            Assert.Equal(3.7, _reportData.AverageRating);
            Assert.Same(newReviews, _reportData.RecentReviews);
        }

        [Fact]
        public void AdminReportData_WithExtremeDateValues_InitializesCorrectly()
        {
            // Arrange
            var minDate = DateTime.MinValue;
            var maxDate = DateTime.MaxValue;

            // Act & Assert
            var data1 = new AdminReportData(minDate, AdminUsers, ActiveUsersCount, BannedUsersCount, NewReviewsCount, AverageRating, RecentReviews);
            Assert.Equal(minDate, data1.ReportDate);

            var data2 = new AdminReportData(maxDate, AdminUsers, ActiveUsersCount, BannedUsersCount, NewReviewsCount, AverageRating, RecentReviews);
            Assert.Equal(maxDate, data2.ReportDate);
        }

        [Fact]
        public void AdminReportData_WithExtremeNumericValues_InitializesCorrectly()
        {
            // Arrange
            int maxInt = int.MaxValue;
            int minInt = int.MinValue;
            double maxDouble = double.MaxValue;
            double minDouble = double.MinValue;

            // Act
            var data = new AdminReportData(
                ReportDate,
                AdminUsers,
                maxInt,
                minInt,
                maxInt,
                maxDouble,
                RecentReviews);

            // Assert
            Assert.Equal(maxInt, data.ActiveUsersCount);
            Assert.Equal(minInt, data.BannedUsersCount);
            Assert.Equal(maxInt, data.NewReviewsCount);
            Assert.Equal(maxDouble, data.AverageRating);

            // Act
            data = new AdminReportData(
                ReportDate,
                AdminUsers,
                minInt,
                maxInt,
                minInt,
                minDouble,
                RecentReviews);

            // Assert
            Assert.Equal(minInt, data.ActiveUsersCount);
            Assert.Equal(maxInt, data.BannedUsersCount);
            Assert.Equal(minInt, data.NewReviewsCount);
            Assert.Equal(minDouble, data.AverageRating);
        }
    }
}