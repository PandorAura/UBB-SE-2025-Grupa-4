using MailKit.Net.Smtp;
using MimeKit;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using App1.Services;
using App1.Models;
using Microsoft.Extensions.Configuration;
using static Org.BouncyCastle.Math.EC.ECCurve;

public class EmailJob : IJob
{
    private readonly IUserService _userService;
    private readonly IConfiguration _config;

    public EmailJob(IUserService userService, IConfiguration configuration)
    {
        if (userService == null || configuration == null)
        {
            System.Diagnostics.Debug.WriteLine("DI FAILED - Null dependencies injected");
            throw new ArgumentNullException();
        }
        System.Diagnostics.Debug.WriteLine("EmailJob created with valid dependencies");
        _userService = userService;
        _config = configuration;
        System.Diagnostics.Debug.WriteLine("EmailJob constructor called");
    }

    public async Task Execute(IJobExecutionContext context)
    {
        System.Diagnostics.Debug.WriteLine("EmailJob execution started");

        try
        {
            // 1. Get credentials from configuration
            var moderatorEmail = _config["SMTP_MODERATOR_EMAIL"];
            var moderatorPassword = _config["SMTP_MODERATOR_PASSWORD"];

            if (string.IsNullOrEmpty(moderatorEmail) || string.IsNullOrEmpty(moderatorPassword))
                throw new Exception("SMTP credentials not configured");

            // 2. Get all admin users through the injected service
            var adminUsers = (_userService.GetActiveUsers(2)).ToList(); // 2 = admin role
            if (!adminUsers.Any()) return;

            // 3. Create and send emails
            using var client = new SmtpClient();
            await client.ConnectAsync("smtp.gmail.com", 587, false);
            await client.AuthenticateAsync(moderatorEmail, moderatorPassword);

            foreach (var user in adminUsers)
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("System Notifications", moderatorEmail));
                message.To.Add(new MailboxAddress(user.Name, user.Email));
                message.Subject = $"Admin Report - {DateTime.Now:yyyy-MM-dd}";

                message.Body = new TextPart("html")
                {
                    Text = $@"<html>
                        <body>
                            <h1>Hello {user.Name},</h1>
                            <p>This is your automated admin report.</p>
                            <p>Generated at: {DateTime.Now}</p>
                        </body>
                    </html>"
                };

                await client.SendAsync(message);
                System.Diagnostics.Debug.WriteLine($"Sent admin email to {user.Email}");
            }

            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Email Job Failed: {ex}");
        }
    }
}