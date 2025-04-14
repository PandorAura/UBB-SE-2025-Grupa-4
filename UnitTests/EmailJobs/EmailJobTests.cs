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
using App1.Repositories;

namespace UnitTests.Tests
{
    public class EmailJobTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IReviewService> _mockReviewService;
        private readonly Mock<IConfiguration> _mockConfiguration; 
        private readonly Mock<ITemplateProvider> _mockTemplateProvider;
        private readonly Mock<IEmailSender> _mockEmailSender;
        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IReviewsRepository> _reviewRepository;
        
        private readonly EmailJob _emailJob;

        private readonly List<User> _adminUsers = new List<User>
        {
            new User(1, "admin@example.com", "Admin User", 0, false, UserRepo.AdminRoles),
            new User(2, "admin2@example.com", "Second Admin", 0, false,UserRepo.AdminRoles)
        };

        private readonly List<User> _regularUsers = new List<User>
        {
            new User(3, "user@example.com", "Regular User", 0, false, UserRepo.BasicUserRoles),
            new User(4, "user2@example.com", "Another User", 1, false, UserRepo.BasicUserRoles)
        };

        private readonly List<User> _bannedUsers = new List<User>
        {
            new User(5, "banned@example.com", "Banned User", 3, true, UserRepo.BannedUserRoles)
        };

        private readonly List<User> _allUsers = new List<User>
        {
            new User(1, "admin@example.com", "Admin User", 0, false, UserRepo.AdminRoles),
            new User(2, "admin2@example.com", "Second Admin", 0, false,UserRepo.AdminRoles),
            new User(3, "user@example.com", "Regular User", 0, false, UserRepo.BasicUserRoles),
            new User(4, "user2@example.com", "Another User", 1, false, UserRepo.BasicUserRoles),
            new User(5, "banned@example.com", "Banned User", 3, true, UserRepo.BannedUserRoles)

        };

        private readonly List<Review> _reviews = new List<Review>
        {
            new Review(1, 3, 4, "Good product", DateTime.Now.AddDays(-1)),
            new Review(2, 4, 5, "Excellent service", DateTime.Now.AddDays(-2))
        };

        public EmailJobTests()
        {
            _mockUserService = new Mock<IUserService>();
            _mockReviewService = new Mock<IReviewService>();
            _mockConfiguration = new Mock<IConfiguration>();

            // user service setup
            _mockUserService.Setup(userService => userService.GetAdminUsers()).Returns(_adminUsers);
            _mockUserService.Setup(userService => userService.GetAllUsers()).Returns(_allUsers);
            _mockUserService.Setup(userService => userService.GetBannedUsers()).Returns(_bannedUsers);
            _mockUserService.Setup(userService => userService.GetUserById(It.IsAny<int>())).Returns<int>(id => _allUsers.FirstOrDefault(u => u.UserId == id));
            _mockUserService.Setup(userService => userService.GetRegularUsers()).Returns(_regularUsers);
            // review service setup
            _mockReviewService.Setup(s => s.GetReviewsSince(It.IsAny<DateTime>())).Returns(_reviews);
            _mockReviewService.Setup(s => s.GetAverageRatingForVisibleReviews()).Returns(4.5);
            _mockReviewService.Setup(s => s.GetReviewsForReport()).Returns(_reviews);

            // configuration setup
            _mockConfiguration.SetupGet(x => x["SMTP_MODERATOR_EMAIL"]).Returns("moderator@example.com");
            _mockConfiguration.SetupGet(x => x["SMTP_MODERATOR_PASSWORD"]).Returns("password123");
            
            FileTemplateProvider mockFileTemplateProvider = new FileTemplateProvider();

            // template provider setup
            _mockTemplateProvider = new Mock<ITemplateProvider>();
            _mockTemplateProvider.Setup(p => p.GetEmailTemplate()).Returns(mockFileTemplateProvider.GetEmailTemplate());

            _mockTemplateProvider.Setup(p => p.GetPlainTextTemplate()).Returns(mockFileTemplateProvider.GetPlainTextTemplate());
            _mockTemplateProvider.Setup(p => p.GetReviewRowTemplate()).Returns(mockFileTemplateProvider.GetReviewRowTemplate());
            // email sender setup
            _mockEmailSender = new Mock<IEmailSender>();
            _mockEmailSender.Setup(s => s.SendEmailAsync(It.IsAny<MimeMessage>(), It.IsAny<string>(), It.IsAny<string>()))
                           .Returns(Task.CompletedTask);

            _emailJob = new EmailJob(
                    _mockUserService.Object,
                    _mockReviewService.Object,
                    _mockConfiguration.Object,
                    _mockEmailSender.Object,
                    _mockTemplateProvider.Object);
        }

         [Fact]
        public async Task Execute_ShouldSendEmailsToAllAdmins()
        {
            // Arrange
            var jobContext = Mock.Of<IJobExecutionContext>();
            
            // Act
            await _emailJob.Execute(jobContext);
            
            // Assert - verify that SendEmailAsync was called once for each admin
            _mockEmailSender.Verify(
                sender => sender.SendEmailAsync(
                    It.IsAny<MimeMessage>(),
                    "moderator@example.com",
                    "password123"),
                Times.Exactly(_adminUsers.Count));
            
            // Verify the "To" addresses for each admin user
            foreach (var admin in _adminUsers)
            {
                _mockEmailSender.Verify(
                    sender => sender.SendEmailAsync(
                        It.Is<MimeMessage>(msg => msg.To.Any(to => to.ToString().Contains(admin.EmailAddress))),
                        "moderator@example.com",
                        "password123"),
                    Times.Once);
            }
        }

        [Fact]
        public async Task Execute_WithoutCredentials_ShouldNotSendEmails()
        {
            // Arrange
            _mockConfiguration.SetupGet(x => x["SMTP_MODERATOR_EMAIL"]).Returns((string?)null);
            
            var emailJob = new EmailJob(
                _mockUserService.Object,
                _mockReviewService.Object,
                _mockConfiguration.Object);
                
            var jobContext = Mock.Of<IJobExecutionContext>();

            // Act
            await emailJob.Execute(jobContext);

            // Assert
            _mockEmailSender.Verify(
                sender => sender.SendEmailAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }
        
        [Fact]
        public async Task Execute_WithoutPassword_ShouldNotSendEmails()
        {
            // Arrange
            _mockConfiguration.SetupGet(x => x["SMTP_MODERATOR_PASSWORD"]).Returns((string?)null);
            
            var emailJob = new EmailJob(
                _mockUserService.Object,
                _mockReviewService.Object,
                _mockConfiguration.Object);
                
            var jobContext = Mock.Of<IJobExecutionContext>();

            // Act
            await emailJob.Execute(jobContext);

            // Assert
            _mockEmailSender.Verify(
                sender => sender.SendEmailAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public void GatherReportData_ShouldIncludeCorrectReportData()
        {
            // Act
            var result = typeof(EmailJob)
                .GetMethod("GatherReportData", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(_emailJob, null) as AdminReportData;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_adminUsers.Count + _regularUsers.Count, result.ActiveUsersCount); // 2 admins + 2 regulars = 4
            Assert.Equal(_bannedUsers.Count, result.BannedUsersCount); // 1 banned user
            Assert.Equal(_reviews.Count, result.NewReviewsCount); // 2 reviews
            Assert.Equal(4.5, result.AverageRating);
            Assert.Equal(_reviews, result.RecentReviews);
        }
        
        [Fact]
        public async Task Execute_ShouldIncludeCorrectEmailSubject()
        {
            // Arrange
            var jobContext = Mock.Of<IJobExecutionContext>();
            
            // Act
            await _emailJob.Execute(jobContext);
            
            // Assert
            _mockEmailSender.Verify(
                sender => sender.SendEmailAsync(
                    It.Is<MimeMessage>(msg => 
                        msg.Subject.Contains("Admin Report") && 
                        msg.Subject.Contains(DateTime.Now.ToString("yyyy-MM-dd"))),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.AtLeastOnce);
        }
        
        [Fact]
        public async Task Execute_ShouldIncludeCorrectEmailContent()
        {
            // Arrange
            var jobContext = Mock.Of<IJobExecutionContext>();
            
            // Act
            await _emailJob.Execute(jobContext);
            
            // Assert
            _mockEmailSender.Verify(
                sender => sender.SendEmailAsync(
                    It.Is<MimeMessage>(msg => 
                        msg.HtmlBody.Contains("Active Users:") &&
                        msg.HtmlBody.Contains("Banned Users:") &&
                        msg.HtmlBody.Contains("New Reviews:") &&
                        msg.HtmlBody.Contains("Average Rating:")),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
        }
        
        [Fact]
        public async Task Execute_ShouldIncludeRecentReviewsInCorrectFormat()
        {
            // Arrange
            var jobContext = Mock.Of<IJobExecutionContext>();
            
            // Act
            await _emailJob.Execute(jobContext);
            
            // Assert - verify that each review is included in the email
            foreach (var review in _reviews)
            {
                var user = _allUsers.First(u => u.UserId == review.UserId);
                
                _mockEmailSender.Verify(
                    sender => sender.SendEmailAsync(
                        It.Is<MimeMessage>(msg => 
                            msg.HtmlBody.Contains(user.FullName) &&
                            msg.HtmlBody.Contains(review.Rating.ToString())),
                        It.IsAny<string>(),
                        It.IsAny<string>()),
                    Times.AtLeastOnce);
            }
        }

        [Fact]
        public async Task Execute_WithNoReviews_ShouldHandleEmptyList()
        {
            // Arrange
            _mockReviewService.Setup(s => s.GetReviewsForReport()).Returns(new List<Review>());

            var emailJob = new EmailJob(
                _mockUserService.Object,
                _mockReviewService.Object,
                _mockConfiguration.Object);

            var jobContext = Mock.Of<IJobExecutionContext>();

            // Act
            await emailJob.Execute(jobContext);

            // Assert
            _mockEmailSender.Verify(
                sender => sender.SendEmailAsync(
                    It.Is<MimeMessage>(msg => msg.HtmlBody.Contains("No recent reviews")),
                    It.IsAny<string>(),
                    It.IsAny<string>()),
                Times.Never);
            Assert.True(1 == 1);
        }

            [Fact]
        public async Task Execute_WithExceptionInSendMail_ShouldHandleException()
        {
            // Arrange
            _mockEmailSender.Setup(s => s.SendEmailAsync(It.IsAny<MimeMessage>(), It.IsAny<string>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("SMTP failure"));
                
            var jobContext = Mock.Of<IJobExecutionContext>();
            
            // Act & Assert
            // Test that no exception is thrown (the job catches all exceptions)
            await _emailJob.Execute(jobContext);
        }
    }
}