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
    public abstract class TextStats
    {
        private Dictionary<string, string> statDictionary = new Dictionary<string, string>();

        public void setValue(string header, string value)
        {
            statDictionary[header] = value;
        }

        public string getValue(string header)
        {
            if (statDictionary.ContainsKey(header)) { return statDictionary[header]; }
            else { return ""; }
        }

        public override string ToString()
        {
            string readouts = "";

            foreach(string header in statDictionary.Keys)
            {
                readouts += String.Format("{0}: {1}", header, statDictionary[header]);
            }

            return readouts;
        }
    }
}
