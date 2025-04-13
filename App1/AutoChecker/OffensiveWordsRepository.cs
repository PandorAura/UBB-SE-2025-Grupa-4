using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.AutoChecker
{
    public class OffensiveWordsRepository : IOffensiveWordsRepository
    {
        private const string SELECTOFFENSIVEWORDSQUERY = "SELECT Word FROM OffensiveWords";
        private const string INSERTOFFENSIVEWORDQUERY =
            "IF NOT EXISTS (SELECT 1 FROM OffensiveWords WHERE Word = @Word) " +
            "INSERT INTO OffensiveWords (Word) VALUES (@Word)";

        private const string DELETEOFFENSIVEWORDQUERY = "DELETE FROM OffensiveWords WHERE Word = @Word";
        private const string WORDPARAMETERNAME = "@Word";
        private readonly string connectionString;

        public OffensiveWordsRepository(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
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
