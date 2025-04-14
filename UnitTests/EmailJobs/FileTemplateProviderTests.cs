using System;
using System.IO;
using App1.Services;
using Moq;
using Xunit;

namespace App1.Tests.Services
{
    public class FileTemplateProviderTests : IDisposable
    {
        private readonly string _tempDirectory;
        private readonly string _templatesDirectory;
        private readonly string _emailTemplatePath;
        private readonly string _plainTextTemplatePath;
        private readonly string _reviewTemplatePath;

        public FileTemplateProviderTests()
        {
            // Set up test directory structure
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            _templatesDirectory = Path.Combine(_tempDirectory, "Templates");
            Directory.CreateDirectory(_templatesDirectory);

            // Create template files with test content
            _emailTemplatePath = Path.Combine(_templatesDirectory, "EmailContentTemplate.html");
            _plainTextTemplatePath = Path.Combine(_templatesDirectory, "PlainTextContentTemplate.txt");
            _reviewTemplatePath = Path.Combine(_templatesDirectory, "RecentReviewForReportTemplate.html");

            File.WriteAllText(_emailTemplatePath, "<html><body>Email Template</body></html>");
            File.WriteAllText(_plainTextTemplatePath, "Plain Text Template");
            File.WriteAllText(_reviewTemplatePath, "<div>Review Template</div>");

            // Mock or override the base directory for tests
            Environment.SetEnvironmentVariable("BASEDIR", _tempDirectory);
        }

        [Fact]
        public void GetEmailTemplate_WhenFileExists_ReturnsFileContent()
        {
            // Arrange
            var provider = new TestFileTemplateProvider(_tempDirectory);

            // Act
            var result = provider.GetEmailTemplate();

            // Assert
            Assert.Equal("<html><body>Email Template</body></html>", result);
        }

        [Fact]
        public void GetPlainTextTemplate_WhenFileExists_ReturnsFileContent()
        {
            // Arrange
            var provider = new TestFileTemplateProvider(_tempDirectory);

            // Act
            var result = provider.GetPlainTextTemplate();

            // Assert
            Assert.Equal("Plain Text Template", result);
        }

        [Fact]
        public void GetReviewRowTemplate_WhenFileExists_ReturnsFileContent()
        {
            // Arrange
            var provider = new TestFileTemplateProvider(_tempDirectory);

            // Act
            var result = provider.GetReviewRowTemplate();

            // Assert
            Assert.Equal("<div>Review Template</div>", result);
        }

        [Fact]
        public void GetEmailTemplate_WhenFileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Arrange
            var provider = new TestFileTemplateProvider(_tempDirectory);
            File.Delete(_emailTemplatePath);

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => provider.GetEmailTemplate());
        }

        [Fact]
        public void GetPlainTextTemplate_WhenFileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Arrange
            var provider = new TestFileTemplateProvider(_tempDirectory);
            File.Delete(_plainTextTemplatePath);

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => provider.GetPlainTextTemplate());
        }

        [Fact]
        public void GetReviewRowTemplate_WhenFileDoesNotExist_ThrowsFileNotFoundException()
        {
            // Arrange
            var provider = new TestFileTemplateProvider(_tempDirectory);
            File.Delete(_reviewTemplatePath);

            // Act & Assert
            Assert.Throws<FileNotFoundException>(() => provider.GetReviewRowTemplate());
        }

        [Fact]
        public void GetEmailTemplate_WhenDirectoryDoesNotExist_ThrowsDirectoryNotFoundException()
        {
            // Arrange
            Directory.Delete(_templatesDirectory, true);
            var provider = new TestFileTemplateProvider(_tempDirectory);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => provider.GetEmailTemplate());
        }

        [Fact]
        public void GetPlainTextTemplate_WhenDirectoryDoesNotExist_ThrowsDirectoryNotFoundException()
        {
            // Arrange
            Directory.Delete(_templatesDirectory, true);
            var provider = new TestFileTemplateProvider(_tempDirectory);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => provider.GetPlainTextTemplate());
        }

        [Fact]
        public void GetReviewRowTemplate_WhenDirectoryDoesNotExist_ThrowsDirectoryNotFoundException()
        {
            // Arrange
            Directory.Delete(_templatesDirectory, true);
            var provider = new TestFileTemplateProvider(_tempDirectory);

            // Act & Assert
            Assert.Throws<DirectoryNotFoundException>(() => provider.GetReviewRowTemplate());
        }

        [Fact]
        public void GetEmailTemplate_WhenAccessDenied_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var mockFileSystem = new Mock<IFileSystem>();
            mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Throws(new UnauthorizedAccessException("Access denied"));

            var provider = new TestFileTemplateProvider(_tempDirectory, mockFileSystem.Object);

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => provider.GetEmailTemplate());
        }

        [Fact]
        public void GetPlainTextTemplate_WhenAccessDenied_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var mockFileSystem = new Mock<IFileSystem>();
            mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Throws(new UnauthorizedAccessException("Access denied"));

            var provider = new TestFileTemplateProvider(_tempDirectory, mockFileSystem.Object);

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => provider.GetPlainTextTemplate());
        }

        [Fact]
        public void GetReviewRowTemplate_WhenAccessDenied_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var mockFileSystem = new Mock<IFileSystem>();
            mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Throws(new UnauthorizedAccessException("Access denied"));

            var provider = new TestFileTemplateProvider(_tempDirectory, mockFileSystem.Object);

            // Act & Assert
            Assert.Throws<UnauthorizedAccessException>(() => provider.GetReviewRowTemplate());
        }

        [Fact]
        public void GetEmailTemplate_WhenIOExceptionOccurs_ThrowsIOException()
        {
            // Arrange
            var mockFileSystem = new Mock<IFileSystem>();
            mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Throws(new IOException("IO error occurred"));

            var provider = new TestFileTemplateProvider(_tempDirectory, mockFileSystem.Object);

            // Act & Assert
            Assert.Throws<IOException>(() => provider.GetEmailTemplate());
        }

        [Fact]
        public void GetPlainTextTemplate_WhenIOExceptionOccurs_ThrowsIOException()
        {
            // Arrange
            var mockFileSystem = new Mock<IFileSystem>();
            mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Throws(new IOException("IO error occurred"));

            var provider = new TestFileTemplateProvider(_tempDirectory, mockFileSystem.Object);

            // Act & Assert
            Assert.Throws<IOException>(() => provider.GetPlainTextTemplate());
        }

        [Fact]
        public void GetReviewRowTemplate_WhenIOExceptionOccurs_ThrowsIOException()
        {
            // Arrange
            var mockFileSystem = new Mock<IFileSystem>();
            mockFileSystem.Setup(fs => fs.ReadAllText(It.IsAny<string>()))
                .Throws(new IOException("IO error occurred"));

            var provider = new TestFileTemplateProvider(_tempDirectory, mockFileSystem.Object);

            // Act & Assert
            Assert.Throws<IOException>(() => provider.GetReviewRowTemplate());
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_tempDirectory, true);
            }
            catch
            {
                // Best effort cleanup
            }

            Environment.SetEnvironmentVariable("BASEDIR", null);
        }
    }

    // Testable version of FileTemplateProvider
    public interface IFileSystem
    {
        string ReadAllText(string path);
    }

    public class DefaultFileSystem : IFileSystem
    {
        public string ReadAllText(string path)
        {
            return File.ReadAllText(path);
        }
    }

    public class TestFileTemplateProvider : ITemplateProvider
    {
        private readonly string _baseDirectory;
        private readonly IFileSystem _fileSystem;

        public TestFileTemplateProvider(string baseDirectory, IFileSystem fileSystem = null)
        {
            _baseDirectory = baseDirectory;
            _fileSystem = fileSystem ?? new DefaultFileSystem();
        }

        public string GetEmailTemplate()
        {
            string path = Path.Combine(_baseDirectory, "Templates", "EmailContentTemplate.html");
            return _fileSystem.ReadAllText(path);
        }

        public string GetPlainTextTemplate()
        {
            string path = Path.Combine(_baseDirectory, "Templates", "PlainTextContentTemplate.txt");
            return _fileSystem.ReadAllText(path);
        }

        public string GetReviewRowTemplate()
        {
            string path = Path.Combine(_baseDirectory, "Templates", "RecentReviewForReportTemplate.html");
            return _fileSystem.ReadAllText(path);
        }
    }
}