using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(MimeMessage message, string smtpEmail, string smtpPassword);
    }
}
