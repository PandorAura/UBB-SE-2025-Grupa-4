using MailKit.Net.Smtp;
using MimeKit;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

public class EmailJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            // 1. Get credentials from environment variables
            var moderatorEmail = Environment.GetEnvironmentVariable("SMTP_MODERATOR_EMAIL", EnvironmentVariableTarget.User);
            var moderatorPassword = Environment.GetEnvironmentVariable("SMTP_MODERATOR_PASSWORD", EnvironmentVariableTarget.User);

            if (string.IsNullOrEmpty(moderatorEmail) || string.IsNullOrEmpty(moderatorPassword))
                throw new Exception("SMTP credentials not configured in environment variables");

            // 2. Get all admin users (example - replace with your actual user repository)
            var adminUsers = await UserRepository.GetAdminsAsync(); // Your method to get admins (roleId == 2)
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
            // Consider adding retry logic here
        }
    }
}