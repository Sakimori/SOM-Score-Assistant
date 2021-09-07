using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOM_Score_Assistant
{
    class Game
    {
        private Team awayTeam;
        private Team homeTeam;
        private LineScore lineScore;
        public bool topOfInning;
        public int inning;

        public Game(Team away, Team home)
        {
            awayTeam = away;
            homeTeam = home;
            lineScore = new LineScore(away.getTrigram(), home.getTrigram());

            topOfInning = true;
            inning = 1;
        }

        /// <summary>
        /// Returns home team if passed True, and away team if passed False.
        /// </summary>
        public Team getTeam(bool home) => home ? homeTeam : awayTeam;
        public LineScore getLineScore() => lineScore;

        public void endHalfInning()
        {
            if (topOfInning) { topOfInning = false; }
            else
            {
                topOfInning = true;
                inning += 1;
                lineScore.addInning();
            }
        }

        public void addRuns(int runs)
        {
            lineScore.addScore(runs, topOfInning);
        }
    }
}
