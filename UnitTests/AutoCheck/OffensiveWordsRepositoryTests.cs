namespace UnitTests.Autocheck
{
    using System;
    using System.Linq;
    using App1.AutoChecker;
    using App1.Infrastructure;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Xunit;
    using System.IO;

    // integration tests
    public class OffensiveWordsRepositoryTests : IDisposable
    {
        private readonly string connectionString;
        private readonly OffensiveWordsRepository repository;
        private readonly IDbConnectionFactory connectionFactory;

        public OffensiveWordsRepositoryTests()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .Build();

            this.connectionString = config.GetConnectionString("TestConnection");
            this.connectionFactory = new SqlConnectionFactory(connectionString);
            this.repository = new OffensiveWordsRepository(connectionFactory);
            EnsureTableExists();
            CleanupTable();
        }

        public void Dispose()
        {
            CleanupTable();
        }

        [Fact]
        public void LoadOffensiveWords_WhenEmpty_ReturnsEmptySet()
        {
            var result = this.repository.LoadOffensiveWords();
            Assert.Empty(result);
        }

        [Fact]
        public void AddWord_ThenLoadOffensiveWords_ContainsWord()
        {
            this.repository.AddWord("troll");

            var result = this.repository.LoadOffensiveWords();

            Assert.Contains("troll", result, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void DeleteWord_RemovesWord()
        {
            this.repository.AddWord("annoying");
            this.repository.DeleteWord("annoying");

            var result = this.repository.LoadOffensiveWords();

            Assert.DoesNotContain("annoying", result, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void Constructor_NullConnectionFactory_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new OffensiveWordsRepository(null));
        }

        [Fact]
        public void AddWord_NullWord_DoesNotThrow()
        {
            var exception = Record.Exception(() => this.repository.AddWord(null));
            Assert.Null(exception);
        }

        [Fact]
        public void AddWord_EmptyWord_DoesNotThrow()
        {
            var exception = Record.Exception(() => this.repository.AddWord(string.Empty));
            Assert.Null(exception);
        }

        [Fact]
        public void AddWord_WhitespaceWord_DoesNotThrow()
        {
            var exception = Record.Exception(() => this.repository.AddWord("   "));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteWord_NullWord_DoesNotThrow()
        {
            var exception = Record.Exception(() => this.repository.DeleteWord(null));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteWord_EmptyWord_DoesNotThrow()
        {
            var exception = Record.Exception(() => this.repository.DeleteWord(string.Empty));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteWord_WhitespaceWord_DoesNotThrow()
        {
            var exception = Record.Exception(() => this.repository.DeleteWord("   "));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteWord_NonExistentWord_DoesNotThrow()
        {
            var exception = Record.Exception(() => this.repository.DeleteWord("nonexistent"));
            Assert.Null(exception);
        }

        private void CleanupTable()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM OffensiveWords", conn);
            cmd.ExecuteNonQuery();
        }

        private void EnsureTableExists()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand(
                @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OffensiveWords')
                BEGIN
                    CREATE TABLE OffensiveWords (
                        Word NVARCHAR(100) PRIMARY KEY
                    )
                END", conn);
            cmd.ExecuteNonQuery();
        }
    }
}
