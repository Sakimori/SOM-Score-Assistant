using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM_Score_Assistant
{
    public abstract class FullStats
    {

    }

    public class PitcherFullStats : FullStats
    {
        public int[] statsArray { get; set; } = new int[12];

        public int offset = 2;
        private static Dictionary<string, int> translate = new Dictionary<string, int>()
        {
            {"OP", 0 },
            {"H", 1 },
            {"R", 2 },
            {"ER", 3 },
            {"K", 4 },
            {"BB", 5 },
            {"HBP", 6 },
            {"WP", 7 },
            {"BK", 8 },
            {"HR", 9 },
            {"DP", 10 },
            {"GS", 11 }
        };

        public int getStat(int index)
        {
            return statsArray[index];
        }

        public int getStat(string name)
        {
            return statsArray[translate[name]];
        }

        public void addStat(string name, int add)
        {
            statsArray[translate[name]] += add;
        }

        public PitcherFullStats()
        {

        }
    }

    public class BatterFullStats : FullStats
    {
        public int[] statsArray { get; set; } = new int[27];

        public int offset = 3;
        private static Dictionary<string, int> translate = new Dictionary<string, int>()
        {
            {"PA", 0 },
            {"AB", 1 },
            {"R", 2 },
            {"H", 3 },
            {"RBI", 4 },
            {"2B", 5 },
            {"3B", 6 },
            {"HR", 7 },
            {"BB", 8 },
            {"K", 9 },
            {"HBP", 10 },
            {"SAC", 11 },
            {"IBB", 12 },
            {"GIDP", 13 },
            {"SB", 14 },
            {"CS", 15 },
            {"BPHR", 16 },
            {"BPFO", 17 },
            {"BP1B", 18 },
            {"BPLO", 19 },
            {"xCH", 20 },
            {"xHIT", 21 },
            {"E", 22 },
            {"PB", 23 },
            {"SBc", 24 },
            {"CSc", 25 },
            {"TP", 26 }
        };

        public BatterFullStats()
        {

        }

        public int getStat(int index)
        {
            return statsArray[index];
        }

        public int getStat(string name)
        {
            return statsArray[translate[name]];
        }

        public void addStat(string name, int add)
        {
            statsArray[translate[name]] += add;
        }
    }
}
