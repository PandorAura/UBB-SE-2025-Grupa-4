using MailKit.Net.Smtp;
using MimeKit;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using App1.Services;
using App1.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Windows.ApplicationModel.Contacts;

public class EmailJob : IJob
{
    private readonly IUserService _userService;
    private readonly IReviewService _reviewService;
    private readonly IConfiguration _config;

    private static string EmailTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "EmailContentTemplate.html");
    private static string PlainTextTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "PlainTextContentTemplate.txt");
    private static string ReviewTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "RecentReviewForReportTemplate.html");

    public EmailJob(IUserService userService, IReviewService reviewService, IConfiguration configuration)
    {
        _userService = userService;
        _reviewService = reviewService;
        _config = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            string? smtpEmail = _config["SMTP_MODERATOR_EMAIL"];
            string? smtpPassword = _config["SMTP_MODERATOR_PASSWORD"];

            if (string.IsNullOrEmpty(smtpEmail) || string.IsNullOrEmpty(smtpPassword))
                throw new Exception("SMTP credentials not configured in appsettings.json");

            var reportData = await GatherReportData();

            string emailContent = GenerateEmailContent(reportData);

            SmtpClient client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync(smtpEmail, smtpPassword);

            foreach (User admin in reportData.AdminUsers)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("System Admin", smtpEmail));
                message.To.Add(new MailboxAddress(admin.FullName, admin.EmailAddress));
                message.Subject = $"Admin Report - {reportData.ReportDate:yyyy-MM-dd}";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = emailContent,
                    TextBody = GeneratePlainTextContent(reportData)
                };

                message.Body = bodyBuilder.ToMessageBody();
                await client.SendAsync(message);
                System.Diagnostics.Debug.WriteLine($"Sent report to {admin.EmailAddress}");
            }

            await client.DisconnectAsync(true);
            System.Diagnostics.Debug.WriteLine("Email job completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Email Job Failed: {ex}");
        }
    }

    private async Task<AdminReportData> GatherReportData()
    {
        DateTime reportDate = DateTime.Now;
        DateTime yesterday = reportDate.AddDays(DAYS_FROM_TODAY_TO_YESTERDAY);
        List<User> adminUsers = _userService.GetAdminUsers();
        int activeUsersCount = _userService.GetAdminUsers().Count() + _userService.GetRegularUsers().Count();
        int bannedUsersCount = _userService.GetBannedUsers().Count();
        int numberOfNewReviews = _reviewService.GetReviewsSince(yesterday).Count;
        double averageRating = _reviewService.GetAverageRatingForVisibleReviews();
        List<Review> recentReviews = _reviewService.GetReviewsForReport();

        return new AdminReportData(reportDate, adminUsers, activeUsersCount, bannedUsersCount, numberOfNewReviews, averageRating, recentReviews);
    }
    private string GenerateEmailContent(AdminReportData data)
    {
        string emailTemplate = File.ReadAllText(EmailTemplatePath);
        emailTemplate = emailTemplate.Replace("{{ReportDate}}", data.ReportDate.ToString("yyyy-MM-dd"));
        emailTemplate = emailTemplate.Replace("{{ActiveUsersCount}}", data.ActiveUsersCount.ToString());
        emailTemplate = emailTemplate.Replace("{{BannedUsersCount}}", data.BannedUsersCount.ToString());
        emailTemplate = emailTemplate.Replace("{{NewReviewsCount}}", data.NewReviewsCount.ToString());
        emailTemplate = emailTemplate.Replace("{{AverageRating}}", data.AverageRating.ToString("0.0"));
        emailTemplate = emailTemplate.Replace("{{RecentReviewsHtml}}", GenerateRecentReviewsHtml(data.RecentReviews));
        emailTemplate = emailTemplate.Replace("{{GeneratedAt}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        return emailTemplate;
    }

    private string GenerateRecentReviewsHtml(List<Review> reviews)
    {
        if (!reviews.Any())
            return "<p>No recent reviews</p>";
            
        StringBuilder recentReviewsTable = new StringBuilder("<table border='1' cellpadding='5' style='border-collapse: collapse; width: 100%;'> <tr><th>User</th><th>Rating</th><th>Date</th></tr>");
        foreach (Review review in reviews)
        {
            string row = File.ReadAllText(ReviewTemplatePath);
            string userName = _userService.GetUserById(review.UserId).FullName;
            row = row.Replace("{{userName}}", userName);
            row = row.Replace("{{rating}}", review.Rating.ToString());
            row = row.Replace("{{creationDate}}", review.CreatedDate.ToString("yyyy-MM-dd"));
            recentReviewsTable.Append(row);
        }
        recentReviewsTable.Append("</table>");
        return recentReviewsTable.ToString();
    }

    private string GeneratePlainTextContent(AdminReportData data)
    {
        string textTemplate = File.ReadAllText(PlainTextTemplatePath);

        textTemplate = textTemplate.Replace("{{ReportDate}}", data.ReportDate.ToString("yyyy-MM-dd"));
        textTemplate = textTemplate.Replace("{{ActiveUsersCount}}", data.ActiveUsersCount.ToString());
        textTemplate = textTemplate.Replace("{{BannedUsersCount}}", data.BannedUsersCount.ToString());
        textTemplate = textTemplate.Replace("{{AverageRating}}", data.AverageRating.ToString("0.0"));
        textTemplate = textTemplate.Replace("{{GeneratedAt}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        return textTemplate;
    }

    private const int DAYS_FROM_TODAY_TO_YESTERDAY = -1;
}

public class AdminReportData
{
    public AdminReportData(DateTime reportDate, List<User> adminUsers, int activeUsersCount, int bannedUsersCount, int newReviewsCount, double averageRating, List<Review> recentReviews)
    {
        this.ReportDate = reportDate;
        this.AdminUsers = adminUsers;
        this.ActiveUsersCount = activeUsersCount;
        this.BannedUsersCount = bannedUsersCount;
        this.NewReviewsCount  = newReviewsCount;
        this.AverageRating = averageRating;
        this.RecentReviews = recentReviews;

    }
    public DateTime ReportDate { get; set; }
    public List<User> AdminUsers { get; set; }
    public int ActiveUsersCount { get; set; }
    public int BannedUsersCount { get; set; }
    public int NewReviewsCount { get; set; }
    public double AverageRating { get; set; }
    public List<Review> RecentReviews { get; set; }
}