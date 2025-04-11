using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;

namespace App1.AutoChecker
{
    /// <summary>
    /// Provides functionality for automatically checking reviews for offensive content
    /// using a predefined list of offensive words stored in a database.
    /// </summary>
    public class AutoCheck : IAutoCheck
    {
        private readonly HashSet<string> _offensiveWords;
        private readonly string _connectionString;
        
        // SQL query constants
        private const string SELECT_OFFENSIVE_WORDS_QUERY = "SELECT Word FROM OffensiveWords";
        private const string INSERT_OFFENSIVE_WORD_QUERY = 
            "IF NOT EXISTS (SELECT 1 FROM OffensiveWords WHERE Word = @Word) " +
            "INSERT INTO OffensiveWords (Word) VALUES (@Word)";
        private const string DELETE_OFFENSIVE_WORD_QUERY = "DELETE FROM OffensiveWords WHERE Word = @Word";
        private const string WORD_PARAMETER_NAME = "@Word";
        
        // Word splitting delimiters
        private static readonly char[] WordDelimiters = new[] 
        { 
            ' ', ',', '.', '!', '?', ';', ':', '\n', '\r', '\t' 
        };

        /// <summary>
        /// Initializes a new instance of the AutoCheck class.
        /// </summary>
        /// <param name="connectionString">The database connection string.</param>
        public AutoCheck(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _offensiveWords = LoadOffensiveWords();
        }

        /// <summary>
        /// Checks if the provided review text contains any offensive words.
        /// </summary>
        /// <param name="reviewText">The text of the review to check.</param>
        /// <returns>True if the review contains offensive content, otherwise false.</returns>
        public bool AutoCheckReview(string reviewText)
        {
            if (string.IsNullOrWhiteSpace(reviewText))
                return false;

            var words = reviewText.Split(
                WordDelimiters,
                StringSplitOptions.RemoveEmptyEntries);

            return words.Any(word => _offensiveWords.Contains(word, StringComparer.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Loads the list of offensive words from the database.
        /// </summary>
        /// <returns>A HashSet containing all offensive words.</returns>
        /// <exception cref="SqlException">Thrown when there is an error accessing the database.</exception>
        public HashSet<string> LoadOffensiveWords()
        {
            var offensiveWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();
                using var command = new SqlCommand(SELECT_OFFENSIVE_WORDS_QUERY, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    offensiveWords.Add(reader.GetString(0));
                }
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"Database error loading offensive words: {sqlException.Message}");
                throw;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error loading offensive words: {exception.Message}");
                throw;
            }

            return offensiveWords;
        }

        /// <summary>
        /// Adds a new offensive word to the database and the in-memory list.
        /// </summary>
        /// <param name="newWord">The offensive word to add.</param>
        /// <exception cref="SqlException">Thrown when there is an error accessing the database.</exception>
        public void AddOffensiveWord(string newWord)
        {
            if (string.IsNullOrWhiteSpace(newWord))
                return;
                
            if (_offensiveWords.Contains(newWord, StringComparer.OrdinalIgnoreCase))
                return;

            using var connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();
                using var command = new SqlCommand(INSERT_OFFENSIVE_WORD_QUERY, connection);

                command.Parameters.AddWithValue(WORD_PARAMETER_NAME, newWord);
                command.ExecuteNonQuery();

                _offensiveWords.Add(newWord);
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"Database error adding offensive word: {sqlException.Message}");
                throw;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error adding offensive word: {exception.Message}");
                throw;
            }
        }

        /// <summary>
        /// Deletes an offensive word from the database and the in-memory list.
        /// </summary>
        /// <param name="word">The offensive word to delete.</param>
        /// <exception cref="SqlException">Thrown when there is an error accessing the database.</exception>
        public void DeleteOffensiveWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return;
                
            if (!_offensiveWords.Contains(word, StringComparer.OrdinalIgnoreCase))
                return;

            using var connection = new SqlConnection(_connectionString);

            try
            {
                connection.Open();
                using var command = new SqlCommand(DELETE_OFFENSIVE_WORD_QUERY, connection);

                command.Parameters.AddWithValue(WORD_PARAMETER_NAME, word);
                command.ExecuteNonQuery();

                _offensiveWords.Remove(word);
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"Database error deleting offensive word: {sqlException.Message}");
                throw;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error deleting offensive word: {exception.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gets a copy of the current list of offensive words.
        /// </summary>
        /// <returns>A HashSet containing all offensive words.</returns>
        public HashSet<string> GetOffensiveWordsList()
        {
            return new HashSet<string>(_offensiveWords, StringComparer.OrdinalIgnoreCase);
        }
    }
}