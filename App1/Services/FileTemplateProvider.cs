using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services
{
    public class FileTemplateProvider : ITemplateProvider
    {
        private static readonly string _emailPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "EmailContentTemplate.html");
        private static readonly string _plainTextPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "PlainTextContentTemplate.txt");
        private static readonly string _reviewPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates", "RecentReviewForReportTemplate.html");
        

        public string GetEmailTemplate() { return File.ReadAllText(_emailPath); }
        public string GetPlainTextTemplate() { return File.ReadAllText(_plainTextPath); }
        public string GetReviewRowTemplate() { return File.ReadAllText(_reviewPath); }
    }
}
