using System;
using System.IO;
using App1.Services;
using Moq;
using Xunit;

namespace UnitTests.EmailJobs.AuxiliaryTestClasses
{

    public class TestFileTemplateProvider : ITemplateProvider
    {
        private readonly string baseDirectory;
        private readonly IFileSystem _fileSystem;

        public TestFileTemplateProvider(string baseDirectory, IFileSystem fileSystem = null)
        {
            this.baseDirectory = baseDirectory;
            _fileSystem = fileSystem ?? new DefaultFileSystem();
        }

        public string GetEmailTemplate()
        {
            string path = Path.Combine(baseDirectory, "Templates", "EmailContentTemplate.html");
            return _fileSystem.ReadAllText(path);
        }

        public string GetPlainTextTemplate()
        {
            string path = Path.Combine(baseDirectory, "Templates", "PlainTextContentTemplate.txt");
            return _fileSystem.ReadAllText(path);
        }

        public string GetReviewRowTemplate()
        {
            string path = Path.Combine(baseDirectory, "Templates", "RecentReviewForReportTemplate.html");
            return _fileSystem.ReadAllText(path);
        }
    }
}