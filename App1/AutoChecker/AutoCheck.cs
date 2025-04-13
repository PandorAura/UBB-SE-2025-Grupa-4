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
        private readonly IOffensiveWordsRepository _repository;
        private readonly HashSet<string> _offensiveWords;

        private static readonly char[] WordDelimiters = new[]
        {
        ' ', ',', '.', '!', '?', ';', ':', '\n', '\r', '\t'
    };

        public AutoCheck(IOffensiveWordsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _offensiveWords = _repository.LoadOffensiveWords();
        }

        public bool AutoCheckReview(string reviewText)
        {
            if (string.IsNullOrWhiteSpace(reviewText))
                return false;

            string[] words = reviewText.Split(WordDelimiters, StringSplitOptions.RemoveEmptyEntries);
            return words.Any(word => _offensiveWords.Contains(word, StringComparer.OrdinalIgnoreCase));
        }

        public void AddOffensiveWord(string newWord)
        {
            if (string.IsNullOrWhiteSpace(newWord)) return;
            if (_offensiveWords.Contains(newWord, StringComparer.OrdinalIgnoreCase)) return;

            _repository.AddWord(newWord);
            _offensiveWords.Add(newWord);
        }

        public void DeleteOffensiveWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word)) return;
            if (!_offensiveWords.Contains(word, StringComparer.OrdinalIgnoreCase)) return;

            _repository.DeleteWord(word);
            _offensiveWords.Remove(word);
        }

        public HashSet<string> GetOffensiveWordsList()
        {
            return new HashSet<string>(_offensiveWords, StringComparer.OrdinalIgnoreCase);
        }
    }
}