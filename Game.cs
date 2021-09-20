using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SOM_Score_Assistant
{
    public class Game
    {
        private Team awayTeam;
        private Team homeTeam;
        private LineScore lineScore;
        private BoxScore boxScore;

        public int outs = 0;
        public bool topOfInning;
        public int inning;
        public Dictionary<int, PositionPlayer> bases = new Dictionary<int, PositionPlayer>()
        {
            {1, null },
            {2, null },
            {3, null }
        };
        
        public bool init = false;
        public bool final = false;

        public Game(Team away, Team home)
        {
            awayTeam = away;
            homeTeam = home;
            lineScore = new LineScore(away.getTrigram(), home.getTrigram());
            boxScore = new BoxScore(away, home);

            topOfInning = true;
            inning = 1;
        }

        public Game() { }

        /// <summary>
        /// Returns home team if passed True, and away team if passed False.
        /// </summary>
        public Team getTeam(bool home) => home ? homeTeam : awayTeam;
        /// <returns>[Away team, Home team]</returns>
        public Team[] getTeams() => new Team[] { awayTeam, homeTeam };
        public Team getBattingTeam() => topOfInning ? awayTeam : homeTeam;
        public Team getPitchingTeam() => topOfInning ? homeTeam : awayTeam;
        public LineScore getLineScore() => lineScore;
        public BoxScore getBoxScore() => boxScore;

        public TextStats getFieldingBoxScore()
        {
            return topOfInning ? boxScore.homeTextStatsFielding : boxScore.awayTextStatsFielding;
        }

        public TextStats getBattingBoxScore()
        {
            return topOfInning ? boxScore.awayTextStatsBatting : boxScore.homeTextStatsBatting;
        }

        public void endHalfInning()
        {
            if (topOfInning) { topOfInning = false; }
            else
            {
                topOfInning = true;
                inning += 1;              
                if(lineScore.getTotalScore()[0] != lineScore.getTotalScore()[1] && inning > 9)
                {
                    final = true;
                }
                else
                {
                    lineScore.addInning();
                }
            }

            bases = new Dictionary<int, PositionPlayer>()
            {
                {1, null },
                {2, null },
                {3, null }
            };

            outs = 0;


        }

        public void addRuns(int runs)
        {
            lineScore.addScore(runs, topOfInning);
            getPitcher().baseStats["R"] += runs;
        }

        public Pitcher getPitcher() => topOfInning? homeTeam.getPitcher() : awayTeam.getPitcher();

        public PositionPlayer getBatter() => topOfInning ? awayTeam.getBatter() : homeTeam.getBatter();
    }
}
