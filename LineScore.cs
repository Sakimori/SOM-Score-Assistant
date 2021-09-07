using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM_Score_Assistant
{
    class LineScore
    {
        private string awayTrigram;
        private string homeTrigram;
        private string[] trigrams = new string[2];
        private Dictionary<string, int> awayStats = new Dictionary<string, int>() { { "R", 0 }, { "H", 0 }, { "E", 0 } };
        private Dictionary<string, int> homeStats = new Dictionary<string, int>() { { "R", 0 }, { "H", 0 }, { "E", 0 } };
        /// <summary>
        /// List of [int, int] where the first int is away score in that inning.
        /// </summary>
        private List<int[]> innings = new List<int[]>();

        public LineScore(string awayLetters, string homeLetters)
        {
            awayTrigram = awayLetters;
            homeTrigram = homeLetters;
            trigrams = new string[] { awayLetters, homeLetters};
            addInning();
        }

        public void addInning()
        {
            innings.Add(new int[] { 0, 0 });
        }

        public void addScore(int runs, bool top)
        {
            int index = top ? 0 : 1;
            innings[innings.Count - 1][index] += runs;
        }

        public int getScore(int inning, bool top)
        {
            return innings[inning-1][top ? 0 : 1];
        }

        public int inningCount() => innings.Count;
    }
}
