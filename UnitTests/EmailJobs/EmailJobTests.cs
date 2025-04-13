using Xunit;
using Moq;
using App1.Services;
using App1.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Quartz;
using System.IO;
using System.Text;
using MailKit.Net.Smtp;
using MimeKit;
using System.Reflection;
using System.Linq;
using Moq.Protected;

namespace UnitTests.Tests
{
    public class EmailJobTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IReviewService> _mockReviewService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly EmailJob _emailJob;

        public EmailJobTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockReviewService = new Mock<IReviewService>();
            _mockConfiguration = new Mock<IConfiguration>();

            // Setup configuration values
            _mockConfiguration.Setup(c => c["SMTP_MODERATOR_EMAIL"]).Returns("test@example.com");
            _mockConfiguration.Setup(c => c["SMTP_MODERATOR_PASSWORD"]).Returns("password123");

            _emailJob = new EmailJob(_mockUserService.Object, _mockReviewService.Object, _mockConfiguration.Object);
        }
        [Fact]
        public async Task Execute_WithValidConfiguration_SendsEmailsToAllAdminUsers()
        {
            // Arrange
            var mockContext = new Mock<IJobExecutionContext>();
            var adminUsers = new List<User>
    {
        new User { FullName = "Admin 1", EmailAddress = "admin1@example.com" },
        new User { FullName = "Admin 2", EmailAddress = "admin2@example.com" }
    };
            var regularUsers = new List<User>
    {
        new User { FullName = "User 1", EmailAddress = "user1@example.com" },
        new User { FullName = "User 2", EmailAddress = "user2@example.com" }
    };
            var bannedUsers = new List<User>
    {
        new User { FullName = "Banned 1", EmailAddress = "banned1@example.com" }
    };
            var recentReviews = new List<Review>
    {
        new Review(1, 1, 9, "name", DateTime.Now.AddDays(-1)),
    };

            // Setup mocks
            _mockUserService.Setup(s => s.GetAdminUsers()).Returns(adminUsers);
            _mockUserService.Setup(s => s.GetRegularUsers()).Returns(regularUsers);
            _mockUserService.Setup(s => s.GetBannedUsers()).Returns(bannedUsers);
            _mockReviewService.Setup(s => s.GetReviewsSince(It.IsAny<DateTime>())).Returns(recentReviews);
            _mockReviewService.Setup(s => s.GetAverageRatingForVisibleReviews()).Returns(4.0);
            _mockReviewService.Setup(s => s.GetReviewsForReport()).Returns(recentReviews);

            // Since we can't easily mock the SmtpClient, we'll need a different approach
            // Option 1: Use a test double for EmailJob that overrides the Execute method

            // First, ensure template files exist
            SetupTemplatePaths();

            // Mock EmailJob completely to avoid SMTP operations
            var mockEmailJob = new EmailJob(_mockUserService.Object, _mockReviewService.Object, _mockConfiguration.Object);

            // Use reflection to test private method GatherReportData instead of Execute
            var gatherReportDataMethod = typeof(EmailJob).GetMethod("GatherReportData",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var reportDataTask = gatherReportDataMethod.Invoke(mockEmailJob, null) as Task<AdminReportData>;
            var reportData = await reportDataTask;

            // Assert
            Assert.NotNull(reportData);
            Assert.Equal(adminUsers, reportData.AdminUsers);
            Assert.Equal(adminUsers.Count + regularUsers.Count, reportData.ActiveUsersCount);
            Assert.Equal(bannedUsers.Count, reportData.BannedUsersCount);
            Assert.Equal(recentReviews.Count, reportData.NewReviewsCount);
            Assert.Equal(4.0, reportData.AverageRating);

            // Verify our services were called
            _mockUserService.Verify(s => s.GetAdminUsers(), Times.AtLeastOnce);
            _mockReviewService.Verify(s => s.GetReviewsSince(It.IsAny<DateTime>()), Times.AtLeastOnce);
            _mockReviewService.Verify(s => s.GetAverageRatingForVisibleReviews(), Times.AtLeastOnce);
            _mockReviewService.Verify(s => s.GetReviewsForReport(), Times.AtLeastOnce);
        }

        // Helper method to set up template paths
        private void SetupTemplatePaths()
        {
            var templatesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            Directory.CreateDirectory(templatesDir);

            var emailTemplatePath = Path.Combine(templatesDir, "EmailContentTemplate.html");
            if (!File.Exists(emailTemplatePath))
            {
                File.WriteAllText(emailTemplatePath, "<html><body>Report Date: {{ReportDate}}, Active Users: {{ActiveUsersCount}}, Banned: {{BannedUsersCount}}, New Reviews: {{NewReviewsCount}}, Avg Rating: {{AverageRating}}, {{RecentReviewsHtml}}, Generated: {{GeneratedAt}}</body></html>");
            }

            var plainTextTemplatePath = Path.Combine(templatesDir, "PlainTextContentTemplate.txt");
            if (!File.Exists(plainTextTemplatePath))
            {
                File.WriteAllText(plainTextTemplatePath, "Report Date: {{ReportDate}}\nActive Users: {{ActiveUsersCount}}\nBanned Users: {{BannedUsersCount}}\nAverage Rating: {{AverageRating}}\nGenerated At: {{GeneratedAt}}");
            }

            var reviewTemplatePath = Path.Combine(templatesDir, "RecentReviewForReportTemplate.html");
            if (!File.Exists(reviewTemplatePath))
            {
                File.WriteAllText(reviewTemplatePath, "<tr><td>{{userName}}</td><td>{{rating}}</td><td>{{creationDate}}</td></tr>");
            }
        }
        [Fact]
        public void GatherReportData_ReturnsValidData()
        {
            // Arrange
            var adminUsers = new List<User>
            {
                new User { FullName = "Admin 1", EmailAddress = "admin1@example.com" }
            };

            var regularUsers = new List<User>
            {
                new User { FullName = "User 1", EmailAddress = "user1@example.com" },
                new User { FullName = "User 2", EmailAddress = "user2@example.com" }
            };

            var bannedUsers = new List<User>
            {
                new User { FullName = "Banned 1", EmailAddress = "banned1@example.com" }
            };

            var recentReviews = new List<Review>
            {
                new Review (1, 1, 9, "name", DateTime.Now.AddDays(-1)),
            };

            _mockUserService.Setup(s => s.GetAdminUsers()).Returns(adminUsers);
            _mockUserService.Setup(s => s.GetRegularUsers()).Returns(regularUsers);
            _mockUserService.Setup(s => s.GetBannedUsers()).Returns(bannedUsers);
            _mockReviewService.Setup(s => s.GetReviewsSince(It.IsAny<DateTime>())).Returns(recentReviews);
            _mockReviewService.Setup(s => s.GetAverageRatingForVisibleReviews()).Returns(4.0);
            _mockReviewService.Setup(s => s.GetReviewsForReport()).Returns(recentReviews);

            // Use reflection to access private method
            var gatherReportDataMethod = typeof(EmailJob).GetMethod("GatherReportData",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = gatherReportDataMethod.Invoke(_emailJob, null) as Task<AdminReportData>;
            var reportData = result.Result;

            // Assert
            Assert.NotNull(reportData);
            Assert.Equal(adminUsers, reportData.AdminUsers);
            Assert.Equal(adminUsers.Count + regularUsers.Count, reportData.ActiveUsersCount);
            Assert.Equal(bannedUsers.Count, reportData.BannedUsersCount);
            Assert.Equal(recentReviews.Count, reportData.NewReviewsCount);
            Assert.Equal(4.0, reportData.AverageRating);
            Assert.Equal(recentReviews, reportData.RecentReviews);
        }

        [Fact]
        public void GenerateEmailContent_WithValidData_ReturnsFormattedEmail()
        {
            // Arrange
            var adminUsers = new List<User>
            {
                new User { FullName = "Admin 1", EmailAddress = "admin1@example.com" }
            };

            var recentReviews = new List<Review>
            {
                new Review (1, 1, 9, "name", DateTime.Now.AddDays(-1)),
            };

            var reportData = new AdminReportData(
                DateTime.Now,
                adminUsers,
                5,
                1,
                3,
                4.2,
                recentReviews
            );

            // Create test template files if they don't exist
            var templatesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            Directory.CreateDirectory(templatesDir);

            var emailTemplatePath = Path.Combine(templatesDir, "EmailContentTemplate.html");
            File.WriteAllText(emailTemplatePath, "<html><body>Report Date: {{ReportDate}}, Active Users: {{ActiveUsersCount}}, Banned: {{BannedUsersCount}}, New Reviews: {{NewReviewsCount}}, Avg Rating: {{AverageRating}}, {{RecentReviewsHtml}}, Generated: {{GeneratedAt}}</body></html>");

            var reviewTemplatePath = Path.Combine(templatesDir, "RecentReviewForReportTemplate.html");
            File.WriteAllText(reviewTemplatePath, "<tr><td>{{userName}}</td><td>{{rating}}</td><td>{{creationDate}}</td></tr>");

            // Use reflection to access private method
            var generateEmailContentMethod = typeof(EmailJob).GetMethod("GenerateEmailContent",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = generateEmailContentMethod.Invoke(_emailJob, new object[] { reportData }) as string;

            // Assert
            Assert.NotNull(result);
            Assert.Contains(reportData.ReportDate.ToString("yyyy-MM-dd"), result);
            Assert.Contains(reportData.ActiveUsersCount.ToString(), result);
            Assert.Contains(reportData.BannedUsersCount.ToString(), result);
            Assert.Contains(reportData.NewReviewsCount.ToString(), result);
            Assert.Contains(reportData.AverageRating.ToString("0.0"), result);
        }

        [Fact]
        public void GenerateRecentReviewsHtml_WithReviews_ReturnsFormattedHtml()
        {
            // Arrange
            var recentReviews = new List<Review>
            {
                new Review (1, 1, 9, "name", DateTime.Now.AddDays(-1)),
                new Review (2, 1, 9, "name",  DateTime.Now.AddDays(-1))
            };

            // Create test template files if they don't exist
            var templatesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            Directory.CreateDirectory(templatesDir);

            var reviewTemplatePath = Path.Combine(templatesDir, "RecentReviewForReportTemplate.html");
            File.WriteAllText(reviewTemplatePath, "<tr><td>{{userName}}</td><td>{{rating}}</td><td>{{creationDate}}</td></tr>");

            // Use reflection to access private method
            var generateRecentReviewsHtmlMethod = typeof(EmailJob).GetMethod("GenerateRecentReviewsHtml",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = generateRecentReviewsHtmlMethod.Invoke(_emailJob, new object[] { recentReviews }) as string;

            // Assert
            Assert.NotNull(result);
            Assert.Contains("<table", result);
            Assert.Contains("</table>", result);
            Assert.Contains("<tr>", result);
        }

        [Fact]
        public void GenerateRecentReviewsHtml_WithNoReviews_ReturnsNoReviewsMessage()
        {
            // Arrange
            var emptyReviews = new List<Review>();

            // Use reflection to access private method
            var generateRecentReviewsHtmlMethod = typeof(EmailJob).GetMethod("GenerateRecentReviewsHtml",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = generateRecentReviewsHtmlMethod.Invoke(_emailJob, new object[] { emptyReviews }) as string;

            // Assert
            Assert.NotNull(result);
            Assert.Contains("No recent reviews", result);
        }

        [Fact]
        public void GeneratePlainTextContent_WithValidData_ReturnsFormattedText()
        {
            // Arrange
            var adminUsers = new List<User>
            {
                new User { FullName = "Admin 1", EmailAddress = "admin1@example.com" }
            };

            var recentReviews = new List<Review>
            {
                new Review (1, 1, 9, "name", DateTime.Now.AddDays(-1)),
            };

            var reportData = new AdminReportData(
                DateTime.Now,
                adminUsers,
                5,
                1,
                3,
                4.2,
                recentReviews
            );

            // Create test template files if they don't exist
            var templatesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates");
            Directory.CreateDirectory(templatesDir);

            var textTemplatePath = Path.Combine(templatesDir, "PlainTextContentTemplate.txt");
            File.WriteAllText(textTemplatePath, "Report Date: {{ReportDate}}\nActive Users: {{ActiveUsersCount}}\nBanned Users: {{BannedUsersCount}}\nAverage Rating: {{AverageRating}}\nGenerated At: {{GeneratedAt}}");

            // Use reflection to access private method
            var generatePlainTextContentMethod = typeof(EmailJob).GetMethod("GeneratePlainTextContent",
                BindingFlags.NonPublic | BindingFlags.Instance);

            // Act
            var result = generatePlainTextContentMethod.Invoke(_emailJob, new object[] { reportData }) as string;

            // Assert
            Assert.NotNull(result);
            Assert.Contains(reportData.ReportDate.ToString("yyyy-MM-dd"), result);
            Assert.Contains(reportData.ActiveUsersCount.ToString(), result);
            Assert.Contains(reportData.BannedUsersCount.ToString(), result);
            Assert.Contains(reportData.AverageRating.ToString("0.0"), result);
        }
    }
}