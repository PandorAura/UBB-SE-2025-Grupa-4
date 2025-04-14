﻿using MailKit.Net.Smtp;
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
    private readonly ITemplateProvider _templateProvider ;
    private readonly IEmailSender _emailSender ;

    public EmailJob(
        IUserService userService,
        IReviewService reviewService,
        IConfiguration configuration)
    {
        _userService = userService;
        _reviewService = reviewService;
        _config = configuration;
        _templateProvider = new FileTemplateProvider();
        _emailSender = new SmtpEmailSender();
    }
    public EmailJob(
        IUserService userService,
        IReviewService reviewService,
        IConfiguration configuration,
        IEmailSender emailSender,
        ITemplateProvider templateProvider)
    {
        _userService = userService;
        _reviewService = reviewService;
        _config = configuration;
        _templateProvider = templateProvider;
        _emailSender = emailSender;
    }
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            string? smtpEmail = _config["SMTP_MODERATOR_EMAIL"];
            string? smtpPassword = _config["SMTP_MODERATOR_PASSWORD"];

            if (string.IsNullOrEmpty(smtpEmail) || string.IsNullOrEmpty(smtpPassword))
                throw new Exception("SMTP credentials not configured in appsettings.json");

            AdminReportData reportData = GatherReportData();

            string emailHtml = GenerateEmailContent(reportData);
            string emailText = GeneratePlainTextContent(reportData);

            foreach (User admin in reportData.AdminUsers)
            {
                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress("System Admin", smtpEmail));
                message.To.Add(new MailboxAddress(admin.FullName, admin.EmailAddress));
                message.Subject = $"Admin Report - {reportData.ReportDate:yyyy-MM-dd}";

                var body = new BodyBuilder
                {
                    HtmlBody = emailHtml,
                    TextBody = emailText
                };

                message.Body = body.ToMessageBody();

                await _emailSender.SendEmailAsync(message, smtpEmail, smtpPassword);
                System.Diagnostics.Debug.WriteLine($"Sent report to {admin.EmailAddress}");
            }

            System.Diagnostics.Debug.WriteLine("Email job completed successfully");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Email Job Failed: {ex}");
        }
    }

    private AdminReportData GatherReportData()
    {
        DateTime reportDate = DateTime.Now;
        DateTime yesterday = reportDate.AddDays(-1);
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
        string emailTemplate = _templateProvider.GetEmailTemplate();
        emailTemplate = emailTemplate.Replace("{{ReportDate}}", data.ReportDate.ToString("yyyy-MM-dd"))
                                     .Replace("{{ActiveUsersCount}}", data.ActiveUsersCount.ToString())
                                     .Replace("{{BannedUsersCount}}", data.BannedUsersCount.ToString())
                                     .Replace("{{NewReviewsCount}}", data.NewReviewsCount.ToString())
                                     .Replace("{{AverageRating}}", data.AverageRating.ToString("0.0"))
                                     .Replace("{{RecentReviewsHtml}}", GenerateRecentReviewsHtml(data.RecentReviews))
                                     .Replace("{{GeneratedAt}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        return emailTemplate;
    }

    private string GenerateRecentReviewsHtml(List<Review> reviews)
    {
        if (!reviews.Any())
            return "<p>No recent reviews</p>";

        StringBuilder html = new StringBuilder("<table border='1' cellpadding='5' style='border-collapse: collapse; width: 100%;'> <tr><th>User</th><th>Rating</th><th>Date</th></tr>");
        foreach (var review in reviews)
        {
            string row = _templateProvider.GetReviewRowTemplate();
            string userName = _userService.GetUserById(review.UserId).FullName;
            row = row.Replace("{{userName}}", userName)
                     .Replace("{{rating}}", review.Rating.ToString())
                     .Replace("{{creationDate}}", review.CreatedDate.ToString("yyyy-MM-dd"));
            html.Append(row);
        }

        html.Append("</table>");
        return html.ToString();
    }

    private string GeneratePlainTextContent(AdminReportData data)
    {
        string textTemplate = _templateProvider.GetPlainTextTemplate();
        textTemplate = textTemplate.Replace("{{ReportDate}}", data.ReportDate.ToString("yyyy-MM-dd"))
                                   .Replace("{{ActiveUsersCount}}", data.ActiveUsersCount.ToString())
                                   .Replace("{{BannedUsersCount}}", data.BannedUsersCount.ToString())
                                   .Replace("{{AverageRating}}", data.AverageRating.ToString("0.0"))
                                   .Replace("{{GeneratedAt}}", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        return textTemplate;
    }
}
public class AdminReportData
{
    public AdminReportData(DateTime reportDate, List<User> adminUsers, int activeUsersCount, int bannedUsersCount, int newReviewsCount, double averageRating, List<Review> recentReviews)
    {
        this.ReportDate = reportDate;
        this.AdminUsers = adminUsers;
        this.ActiveUsersCount = activeUsersCount;
        this.BannedUsersCount = bannedUsersCount;
        this.NewReviewsCount = newReviewsCount;
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