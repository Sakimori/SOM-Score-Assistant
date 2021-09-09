using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM_Score_Assistant
{
    class BoxScore
    {
        private Team awayTeam;
        private Team homeTeam;
        public BoxScore(Team away, Team home)
        {
            awayTeam = away;
            homeTeam = home;
        }

        public TextStats awayTextStatsBatting = new TextStats();
        public TextStats awayTextStatsFielding = new TextStats();
        public TextStats homeTextStatsBatting = new TextStats();
        public TextStats homeTextStatsFielding = new TextStats();
    }
}
