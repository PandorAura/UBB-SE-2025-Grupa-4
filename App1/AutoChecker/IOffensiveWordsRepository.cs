using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.AutoChecker
{
    public interface IOffensiveWordsRepository
    {
        HashSet<string> LoadOffensiveWords();

        void AddWord(string word);
        
        void DeleteWord(string word);
    }
}
