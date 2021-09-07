using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SOM_Score_Assistant
{
    /// <summary>
    /// Contains all elements to be displayed in the box score for a given team.
    /// </summary>
    class Team
    {
        private string name;
        private string trigram;
        private Image logo;

        public Team(string newName, string newTrigram, string logoFilename)
        {
            setNames(newName, newTrigram);
            setLogoFromFilename(logoFilename);
        }

        public Team(string newName, string newTrigram)
        {
            setNames(newName, newTrigram);
        }

        public Team() { }

        public void setNames(string newName, string newTrigram)
        {
            name = newName;
            trigram = newTrigram;
        }

        public string getName()
        {
            return name;
        }

        public string getTrigram()
        {
            return trigram;
        }
        public void setLogoFromFilename(string filename)
        {
            logo = Image.FromFile(filename);
        }

        public Image getLogo()
        {
            return logo;
        }
    }
}
