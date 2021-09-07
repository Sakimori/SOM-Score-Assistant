using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM_Score_Assistant
{
    /// <summary>
    /// Abstract class for blocks of text, such as game and fielding information.
    /// </summary>
    abstract class TextStats
    {
        private Dictionary<string, string> printouts;

        public override string ToString()
        {
            string readouts = "";

            foreach(string header in printouts.Keys)
            {
                readouts += String.Format("{0}: {1}", header, printouts[header]);
            }

            return readouts;
        }
    }
}
