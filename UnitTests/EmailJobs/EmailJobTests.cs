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

        private readonly Mock<IUserRepository> _userRepository;
        private readonly Mock<IReviewsRepository> _reviewRepository;
        
        private readonly EmailJob _emailJob;

        private readonly List<User> _adminUsers = new List<User>
        {
            new User(1, "admin@example.com", "Admin User", 0, false, UserRepo.adminRoles),
            new User(2, "admin2@example.com", "Second Admin", 0, false,UserRepo.adminRoles)
        };

        private readonly List<User> _regularUsers = new List<User>
        {
            new User(3, "user@example.com", "Regular User", 0, false, UserRepo.basicUserRoles),
            new User(4, "user2@example.com", "Another User", 1, false, UserRepo.basicUserRoles)
        };

        private readonly List<User> _bannedUsers = new List<User>
        {
            new User(5, "banned@example.com", "Banned User", 3, true, UserRepo.bannedUserRoles)
        };

        private readonly List<User> _allUsers = new List<User>
        {
            new User(1, "admin@example.com", "Admin User", 0, false, UserRepo.adminRoles),
            new User(2, "admin2@example.com", "Second Admin", 0, false,UserRepo.adminRoles),
            new User(3, "user@example.com", "Regular User", 0, false, UserRepo.basicUserRoles),
            new User(4, "user2@example.com", "Another User", 1, false, UserRepo.basicUserRoles),
            new User(5, "banned@example.com", "Banned User", 3, true, UserRepo.bannedUserRoles)

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
            // review service setup
            _mockReviewService.Setup(s => s.GetReviewsSince(It.IsAny<DateTime>())).Returns(_reviews);
            _mockReviewService.Setup(s => s.GetAverageRatingForVisibleReviews()).Returns(4.5);
            _mockReviewService.Setup(s => s.GetReviewsForReport()).Returns(_reviews);

            // configuration setup
            _mockConfiguration.SetupGet(x => x["SMTP_MODERATOR_EMAIL"]).Returns("moderator@example.com");
            _mockConfiguration.SetupGet(x => x["SMTP_MODERATOR_PASSWORD"]).Returns("password123");

            _emailJob = new EmailJob(_mockUserService.Object, _mockReviewService.Object, _mockConfiguration.Object);
        }
        [Fact]
        public async Task Execute_ShouldSendEmailsToAllAdmins() { 
            
        }
        [Fact]
        public async Task Execute_WithoutCredentials_ShouldThrowException() { }
        [Fact]
        public async Task Execute_ShouldIncludeCorrectReportData() { }
        [Fact]
        public async Task Execute_WithExceptionInSendMail_ShouldHandleException() { }
    }
}