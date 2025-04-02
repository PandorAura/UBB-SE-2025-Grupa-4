using MailKit.Net.Smtp;
using MimeKit;
using Quartz;
using System;
using System.Threading.Tasks;

public class EmailJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var emailData = context.MergedJobDataMap;

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Your App", "aurapandor@gmail.com"));
            message.To.Add(new MailboxAddress("", emailData.GetString("RecipientEmail")));
            message.Subject = emailData.GetString("Subject");
            message.Body = new TextPart("plain") { Text = emailData.GetString("Body") };

            using (var client = new SmtpClient())
            {
                // Gmail SMTP configuration
                await client.ConnectAsync("smtp.gmail.com", 587, false);
                await client.AuthenticateAsync("aurapandor@gmail.com", "ingh lwzm aayx esnt"); // Use App Password
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            System.Diagnostics.Debug.WriteLine($"Email sent successfully at {DateTime.Now}");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Email failed: {ex.Message}");
        }
    }
}