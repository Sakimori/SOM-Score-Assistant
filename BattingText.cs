using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM_Score_Assistant
{
    /// <summary>
    /// Contains counts for XBH, total bases, RBI, RISP, and GIDP in an easily-displayed text format.
    /// </summary>
    class BattingText : TextStats
    {
        private Dictionary<string, string> statDictionary = new Dictionary<string, string>();

        public BattingText() { }
    }
}
