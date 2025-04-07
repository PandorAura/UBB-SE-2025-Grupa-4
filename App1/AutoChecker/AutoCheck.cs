using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace App1.AutoChecker
{
    public class AutoCheck : IAutoCheck
    {
        private readonly HashSet<string> _offensiveWords;
        private readonly string _connectionString;

        public AutoCheck(string connectionString)
        {
            _connectionString = connectionString;
            _offensiveWords = LoadOffensiveWords();
        }

        public bool AutoCheckReview(string reviewText)
        {
            if (string.IsNullOrWhiteSpace(reviewText))
                return false;

            var words = reviewText.Split(
                new[] { ' ', ',', '.', '!', '?', ';', ':', '\n', '\r', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            return words.Any(word => _offensiveWords.Contains(word, StringComparer.OrdinalIgnoreCase));
        }

        public HashSet<string> LoadOffensiveWords()
        {
            var offensiveWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();
                using var command = new SqlCommand("SELECT Word FROM OffensiveWords", connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    offensiveWords.Add(reader.GetString(0));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading offensive words: {ex.Message}");
                throw; 
            }

            return offensiveWords;
        }

        public void AddOffensiveWord(string newWord)
        {
            if (string.IsNullOrWhiteSpace(newWord) ||
                _offensiveWords.Contains(newWord, StringComparer.OrdinalIgnoreCase))
                return;

            using var connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();
                using var command = new SqlCommand(
                    "IF NOT EXISTS (SELECT 1 FROM OffensiveWords WHERE Word = @Word) " +
                    "INSERT INTO OffensiveWords (Word) VALUES (@Word)",
                    connection);

                command.Parameters.AddWithValue("@Word", newWord);
                command.ExecuteNonQuery();

                _offensiveWords.Add(newWord);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding offensive word: {ex.Message}");
                throw;
            }
        }

        public void DeleteOffensiveWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word) ||
                !_offensiveWords.Contains(word, StringComparer.OrdinalIgnoreCase))
                return;

            using var connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();
                using var command = new SqlCommand(
                    "DELETE FROM OffensiveWords WHERE Word = @Word",
                    connection);

                command.Parameters.AddWithValue("@Word", word);
                command.ExecuteNonQuery();

                _offensiveWords.Remove(word);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting offensive word: {ex.Message}");
                throw;
            }
        }

        public HashSet<string> GetOffensiveWordsList()
        {
            return new HashSet<string>(_offensiveWords, StringComparer.OrdinalIgnoreCase);
        }
    }
}