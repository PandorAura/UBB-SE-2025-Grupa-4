using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.AutoChecker
{
    public class OffensiveWordsRepository : IOffensiveWordsRepository
    {
        private readonly string _connectionString;

        private const string SELECT_OFFENSIVE_WORDS_QUERY = "SELECT Word FROM OffensiveWords";
        private const string INSERT_OFFENSIVE_WORD_QUERY =
            "IF NOT EXISTS (SELECT 1 FROM OffensiveWords WHERE Word = @Word) " +
            "INSERT INTO OffensiveWords (Word) VALUES (@Word)";
        private const string DELETE_OFFENSIVE_WORD_QUERY = "DELETE FROM OffensiveWords WHERE Word = @Word";
        private const string WORD_PARAMETER_NAME = "@Word";

        public OffensiveWordsRepository(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public HashSet<string> LoadOffensiveWords()
        {
            HashSet<string> offensiveWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand(SELECT_OFFENSIVE_WORDS_QUERY, connection);
            using SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                offensiveWords.Add(reader.GetString(0));
            }

            return offensiveWords;
        }

        public void AddWord(string word)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand(INSERT_OFFENSIVE_WORD_QUERY, connection);
            command.Parameters.AddWithValue(WORD_PARAMETER_NAME, word);
            command.ExecuteNonQuery();
        }

        public void DeleteWord(string word)
        {
            using SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();

            using SqlCommand command = new SqlCommand(DELETE_OFFENSIVE_WORD_QUERY, connection);
            command.Parameters.AddWithValue(WORD_PARAMETER_NAME, word);
            command.ExecuteNonQuery();
        }
    }
}
