using App1.AutoChecker;
using System;
using Microsoft.Data.SqlClient;
using System.Linq;
using Xunit;

namespace UnitTests.Autocheck
{
    // integration tests
    public class OffensiveWordsRepositoryTests : IDisposable
    {
        private readonly string connectionString = "Server=ALEXIA_ZEN\\SQLEXPRESS;Database=TestDb;Integrated Security=True;TrustServerCertificate=True;";
        private readonly OffensiveWordsRepository repository;

        public OffensiveWordsRepositoryTests()
        {
            repository = new OffensiveWordsRepository(connectionString);
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
            var result = repository.LoadOffensiveWords();
            Assert.Empty(result);
        }

        [Fact]
        public void AddWord_ThenLoadOffensiveWords_ContainsWord()
        {
            repository.AddWord("troll");

            var result = repository.LoadOffensiveWords();

            Assert.Contains("troll", result, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void DeleteWord_RemovesWord()
        {
            repository.AddWord("annoying");
            repository.DeleteWord("annoying");

            var result = repository.LoadOffensiveWords();

            Assert.DoesNotContain("annoying", result, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void Constructor_NullConnectionString_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new OffensiveWordsRepository(null));
        }

        [Fact]
        public void AddWord_NullWord_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => repository.AddWord(null));
            Assert.Null(exception);
        }

        [Fact]
        public void AddWord_EmptyWord_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => repository.AddWord(string.Empty));
            Assert.Null(exception);
        }

        [Fact]
        public void AddWord_WhitespaceWord_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => repository.AddWord("   "));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteWord_NullWord_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => repository.DeleteWord(null));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteWord_EmptyWord_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => repository.DeleteWord(string.Empty));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteWord_WhitespaceWord_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => repository.DeleteWord("   "));
            Assert.Null(exception);
        }

        [Fact]
        public void DeleteWord_NonExistentWord_DoesNotThrow()
        {
            // Act & Assert
            var exception = Record.Exception(() => repository.DeleteWord("nonexistent"));
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
