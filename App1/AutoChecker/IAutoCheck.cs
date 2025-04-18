﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace App1.AutoChecker
{
    public interface IAutoCheck
    {
        public bool AutoCheckReview(string reviewText);
        public HashSet<string> LoadOffensiveWords();
        public void AddOffensiveWord(string newWord);
        public void DeleteOffensiveWord(string word);
        public HashSet<string> GetOffensiveWordsList();
    }
}
