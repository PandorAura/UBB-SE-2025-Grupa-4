using App1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Services
{
    public interface ICheckersService
    {
        public List<string> RunAutoCheck(List<Review> reviews);
        public HashSet<string> getOffensiveWordsList();

        public void AddOffensiveWord(string newWord);

        public void DeleteOffensiveWord(string word);

    }
}
