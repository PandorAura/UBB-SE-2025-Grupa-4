using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace App1.AutoChecker
{
    public class AutoCheck
    {
        private static readonly string ProjectRoot = GetProjectRoot();
        public static readonly string offensiveWordsFilePath = Path.Combine(ProjectRoot, "AutoChecker", "offensive_words.txt");
        private static HashSet<string> offensiveWords = LoadOffensiveWords();

        public AutoCheck()
        {
            offensiveWords = LoadOffensiveWords();
        }

        public bool AutoCheckReview(string reviewText)
        {
            if (string.IsNullOrWhiteSpace(reviewText))
                return false;
            Console.WriteLine("ok");
            var words = reviewText.Split(new char[] { ' ', ',', '.', '!', '?', ';', ':', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Any(word => offensiveWords.Contains(word));
        }

        private static HashSet<string> LoadOffensiveWords()
        {
            if (File.Exists(offensiveWordsFilePath))
            {
                return new HashSet<string>(File.ReadAllLines(offensiveWordsFilePath), StringComparer.OrdinalIgnoreCase);
            }
            return new HashSet<string>();

            // Use this when the Offensive Words are stored in a database table
            /*
            HashSet<string> offensiveWords = new HashSet<string>();
            connection.Open();
            using (SqlCommand command = new SqlCommand("SELECT Word FROM OffensiveWords", connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string word = reader.GetString(0);
                        offensiveWords.Add(word);
                    }
                }
            }
            */
        }

        private static string GetProjectRoot([CallerFilePath] string filePath = "")
        {
            var dir = new FileInfo(filePath).Directory;
            while (dir != null && !dir.GetFiles("*.csproj").Any())
            {
                dir = dir.Parent;
            }
            return dir?.FullName ?? throw new Exception("Project root not found!");
        }

        public void AddOffensiveWord(string newWord)
        {
            if (!offensiveWords.Contains(newWord))
            {
                offensiveWords.Add(newWord);
                File.WriteAllLines(offensiveWordsFilePath, offensiveWords);

                /*
                connection.Open();
                using (SqlCommand clearCommand = new SqlCommand("DELETE FROM OffensiveWords", connection))
                {
                    clearCommand.ExecuteNonQuery();
                }
                foreach (var word in offensiveWords)
                {
                    using (SqlCommand insertCommand = new SqlCommand("INSERT INTO OffensiveWords(Word) VALUES(@Word)", connection))
                    {
                        insertCommand.Parameters.AddWithValue("@Word", word);
                        insertCommand.ExecuteNonQuery();
                    }
                }
                */
            }
        }

        public void DeleteOffensiveWord(string word)
        { 
            offensiveWords.Remove(word);
            File.WriteAllLines(offensiveWordsFilePath, offensiveWords);

            /*
            connection.Open();
            using (SqlCommand clearCommand = new SqlCommand("DELETE FROM OffensiveWords", connection))
            {
                clearCommand.ExecuteNonQuery();
            }
            foreach (var word in offensiveWords)
            {
                using (SqlCommand insertCommand = new SqlCommand("INSERT INTO OffensiveWords(Word) VALUES(@Word)", connection))
                {
                    insertCommand.Parameters.AddWithValue("@Word", word);
                    insertCommand.ExecuteNonQuery();
                }
            }
            */
        }

        public HashSet<string> getOffensiveWordsList()
        {
            return offensiveWords;
        }
    }
}
