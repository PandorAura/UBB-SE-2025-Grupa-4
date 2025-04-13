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
        private readonly string connectionString = "Server=ALEXIA_ZEN\\SQLEXPRESS;Database=TestDb;Trusted_Connection=True;";
        private readonly OffensiveWordsRepository repository;

        public OffensiveWordsRepositoryTests()
        {
            repository = new OffensiveWordsRepository(connectionString);
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
        public void AddWord_Twice_DoesNotDuplicate()
        {
            repository.AddWord("jerk");
            repository.AddWord("jerk");

            var result = repository.LoadOffensiveWords();

            var count = result.Count(w => w.Equals("jerk", StringComparison.OrdinalIgnoreCase));
            Assert.Equal(1, count);
        }

        [Fact]
        public void DeleteWord_RemovesWord()
        {
            repository.AddWord("annoying");
            repository.DeleteWord("annoying");

            var result = repository.LoadOffensiveWords();

            Assert.DoesNotContain("annoying", result, StringComparer.OrdinalIgnoreCase);
        }

        private void CleanupTable()
        {
            using var conn = new SqlConnection(connectionString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM OffensiveWords", conn);
            cmd.ExecuteNonQuery();
        }
    }
}
