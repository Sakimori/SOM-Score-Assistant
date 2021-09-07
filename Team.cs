using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace SOM_Score_Assistant
{
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
