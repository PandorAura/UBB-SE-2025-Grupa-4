using App1.AutoChecker;
using System;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace UnitTests.Autocheck
{
    // integration tests
    public class OffensiveWordsRepositoryTests : IDisposable
    {
        private readonly string _connectionString = "Server=ALEXIA_ZEN\\SQLEXPRESS;Database=TestDb;Trusted_Connection=True;";
        private readonly OffensiveWordsRepository _repository;

        public OffensiveWordsRepositoryTests()
        {
            _repository = new OffensiveWordsRepository(_connectionString);
            CleanupTable();
        }

        private void CleanupTable()
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            using var cmd = new SqlCommand("DELETE FROM OffensiveWords", conn);
            cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            CleanupTable();
        }

        [Fact]
        public void LoadOffensiveWords_WhenEmpty_ReturnsEmptySet()
        {
            var result = _repository.LoadOffensiveWords();
            Assert.Empty(result);
        }

        [Fact]
        public void AddWord_ThenLoadOffensiveWords_ContainsWord()
        {
            _repository.AddWord("troll");

            var result = _repository.LoadOffensiveWords();

            Assert.Contains("troll", result, StringComparer.OrdinalIgnoreCase);
        }

        [Fact]
        public void AddWord_Twice_DoesNotDuplicate()
        {
            _repository.AddWord("jerk");
            _repository.AddWord("jerk");

            var result = _repository.LoadOffensiveWords();

            var count = result.Count(w => w.Equals("jerk", StringComparison.OrdinalIgnoreCase));
            Assert.Equal(1, count);
        }

        [Fact]
        public void DeleteWord_RemovesWord()
        {
            _repository.AddWord("annoying");
            _repository.DeleteWord("annoying");

            var result = _repository.LoadOffensiveWords();

            Assert.DoesNotContain("annoying", result, StringComparer.OrdinalIgnoreCase);
        }
    }
}
