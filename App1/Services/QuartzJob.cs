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

public class EmailJob : IJob
{
    private readonly IUserService _userService;
    private readonly IReviewService _reviewService;
    private readonly IConfiguration _config;

    public EmailJob(IUserService userService, IReviewService reviewService, IConfiguration configuration)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
        _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        System.Diagnostics.Debug.WriteLine("EmailJob initialized with all dependencies");
    }

    public async Task Execute(IJobExecutionContext context)
    {
        System.Diagnostics.Debug.WriteLine($"EmailJob started at {DateTime.Now}");

        try
        {
            var smtpEmail = _config["SMTP_MODERATOR_EMAIL"];
            var smtpPassword = _config["SMTP_MODERATOR_PASSWORD"];

            if (string.IsNullOrEmpty(smtpEmail) || string.IsNullOrEmpty(smtpPassword))
                throw new Exception("SMTP credentials not configured in appsettings.json");

            var reportData = await GatherReportData();

            var emailContent = GenerateEmailContent(reportData);

            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync(smtpEmail, smtpPassword);

            foreach (var admin in reportData.AdminUsers)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("System Admin", smtpEmail));
                message.To.Add(new MailboxAddress(admin.Name, admin.Email));
                message.Subject = $"Admin Report - {reportData.ReportDate:yyyy-MM-dd}";

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = emailContent,
                    TextBody = GeneratePlainTextContent(reportData)
                };

                message.Body = bodyBuilder.ToMessageBody();
                await client.SendAsync(message);
                System.Diagnostics.Debug.WriteLine($"Sent report to {admin.Email}");
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
        var reportDate = DateTime.Now;
        var yesterday = reportDate.AddDays(-1);

        return new AdminReportData
        {
            ReportDate = reportDate,
            AdminUsers = ( _userService.GetActiveUsers(2)).ToList(), // Admins
            ActiveUsersCount = ( _userService.GetActiveUsers(1)).Count + (_userService.GetActiveUsers(2)).Count, // Regular users
            BannedUsersCount = ( _userService.GetUsersByPermission(-1)).Count,
            NewReviewsCount = ( _reviewService.GetReviewsSince(yesterday)).Count,
            AverageRating =  _reviewService.GetAverageRating(),
            RecentReviews = ( _reviewService.GetRecentReviews(5)).ToList()
        };
    }

    private string GenerateEmailContent(AdminReportData data)
    {
        return $@"
        <html>
            <body style='font-family: Arial, sans-serif;'>
                <h1>Admin Report - {data.ReportDate:yyyy-MM-dd}</h1>
                
                <h2>User Statistics</h2>
                <ul>
                    <li>Active Users: {data.ActiveUsersCount}</li>
                    <li>Banned Users: {data.BannedUsersCount}</li>
                </ul>
                
                <h2>Review Statistics</h2>
                <ul>
                    <li>New Reviews (last 24h): {data.NewReviewsCount}</li>
                    <li>Average Rating: {data.AverageRating:0.0}/5</li>
                </ul>
                
                <h2>Recent Reviews</h2>
                {GenerateRecentReviewsHtml(data.RecentReviews)}
                
                <footer style='margin-top: 20px; color: #666;'>
                    Generated at {DateTime.Now:yyyy-MM-dd HH:mm}
                </footer>
            </body>
        </html>";
    }

    private string GenerateRecentReviewsHtml(List<Review> reviews)
    {
        if (!reviews.Any()) return "<p>No recent reviews</p>";

        return "<table border='1' cellpadding='5' style='border-collapse: collapse; width: 100%;'>" +
               "<tr><th>User</th><th>Rating</th><th>Comment</th><th>Date</th></tr>" +
               string.Join("", reviews.Select(r =>
                   $"<tr>" +
                   $"<td>{r.UserName}</td>" +
                   $"<td>{r.Rating}/5</td>" +
                   $"<td>{r.CreatedDate:yyyy-MM-dd}</td>" +
                   $"</tr>")) +
               "</table>";
    }

    private string GeneratePlainTextContent(AdminReportData data)
    {
        return $@"ADMIN REPORT - {data.ReportDate:yyyy-MM-dd}

User Statistics:
- Active Users: {data.ActiveUsersCount}
- Banned Users: {data.BannedUsersCount}

Review Statistics:
- New Reviews: {data.NewReviewsCount}
- Average Rating: {data.AverageRating:0.0}

Generated at {DateTime.Now:yyyy-MM-dd HH:mm}";
    }

    private string Truncate(string value, int maxLength) =>
        string.IsNullOrEmpty(value) ? "" :
        value.Length <= maxLength ? value :
        value.Substring(0, maxLength) + "...";
}

public class AdminReportData
{
    public DateTime ReportDate { get; set; }
    public List<User> AdminUsers { get; set; }
    public int ActiveUsersCount { get; set; }
    public int BannedUsersCount { get; set; }
    public int NewReviewsCount { get; set; }
    public double AverageRating { get; set; }
    public List<Review> RecentReviews { get; set; }
}