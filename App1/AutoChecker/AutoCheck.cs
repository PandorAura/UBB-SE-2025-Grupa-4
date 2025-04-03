using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.AutoChecker
{
    public class AutoCheck
    {
        private static readonly string OffensiveWordsFilePath = Path.GetFullPath("offensive_words");
        private static HashSet<string> OffensiveWords = LoadOffensiveWords();

        public AutoCheck()
        {
            OffensiveWords = LoadOffensiveWords();
        }

        public bool AutoCheckReview(string reviewText)
        {
            if (string.IsNullOrWhiteSpace(reviewText))
                return false;
            Console.WriteLine("ok");
            var words = reviewText.Split(new char[] { ' ', ',', '.', '!', '?', ';', ':', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            return words.Any(word => OffensiveWords.Contains(word));
        }

        private static HashSet<string> LoadOffensiveWords()
        {
            if (File.Exists(OffensiveWordsFilePath))
            {
                return new HashSet<string>(File.ReadAllLines(OffensiveWordsFilePath), StringComparer.OrdinalIgnoreCase);
            }
            return new HashSet<string>();
        }
    }
}
