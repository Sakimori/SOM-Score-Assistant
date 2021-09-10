using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace SOM_Score_Assistant
{
    public class BoxScore
    {
        private Team awayTeam;
        private Team homeTeam;
        public BoxScore(Team away, Team home)
        {
            awayTeam = away;
            homeTeam = home;
        }

        public BoxScore() { }

        public TextStats awayTextStatsBatting = new TextStats();
        public TextStats awayTextStatsFielding = new TextStats();
        public TextStats homeTextStatsBatting = new TextStats();
        public TextStats homeTextStatsFielding = new TextStats();

        public string[] getTeamNames() => new string[] { awayTeam.getName(), homeTeam.getName() };
        public BatterRow[] getBatterRows(bool away)
        {
            Team team = away ? awayTeam : homeTeam;
            BatterRow[] rows = new BatterRow[10];
            int index = 0;
            foreach(PositionPlayer player in team.getLineup())
            {
                if(player != null)
                {
                    rows[index] = makeBatterRow(player, index);
                    index++;
                }
            }
            return rows;
        }

        public PitcherRow[] getPitcherRows(bool away)
        {
            Team team = away ? awayTeam : homeTeam;
            PitcherRow[] rows = new PitcherRow[13];
            int index = 0;
            foreach(Pitcher player in team.getAllPitchers())
            {
                if(player != null)
                {
                    rows[index] = makePitcherRow(player);
                    index++;
                }
            }
            return rows;
        }

        public BatterRow makeBatterRow(PositionPlayer player, int lineupPosition)
        {
            TextBlock[] blocks = new TextBlock[9];
            blocks[0] = new TextBlock()
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                FontSize = 14,
                Text = "",
            };
            if(lineupPosition >= 0) { blocks[0].Text = (lineupPosition+1).ToString(); }

            blocks[1] = new TextBlock()
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                FontSize = 18,
                Text = player.ToString()
            };

            string position;

            switch (player.positionIndex)
            {
                case 0:
                    position = "C";
                    break;
                case 1:
                    position = "1B";
                    break;
                case 2:
                    position = "2B";
                    break;
                case 3:
                    position = "3B";
                    break;
                case 4:
                    position = "SS";
                    break;
                case 5:
                    position = "LF";
                    break;
                case 6:
                    position = "CF";
                    break;
                case 7:
                    position = "RF";
                    break;
                case 8:
                    position = "DH";
                    break;
                default:
                    position = "PH";
                    break;
            }

            blocks[2] = basicBlock(position);

            string[] statNames = new string[] { "AB", "R", "H", "RBI", "BB", "K" };
            for(int index = 3; index <= 8; index++)
            {
                blocks[index] = basicBlock(player.baseStats[statNames[index - 3]].ToString());
            }

            return new BatterRow(blocks, player);
        }

        public PitcherRow makePitcherRow(Pitcher player)
        {
            TextBlock[] blocks = new TextBlock[7];

            blocks[0] = new TextBlock()
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                FontSize = 18,
                Text = player.ToString()
            };

            //calculate IP from OP
            double OP = Convert.ToDouble(player.baseStats["OP"]);
            blocks[1] = basicBlock(Math.Floor(OP / 3).ToString() + "." + (OP % 3).ToString());

            string[] statNames = new string[] { "H", "R", "BB", "K", "HR" };
            for (int index = 2; index <= 6; index++)
            {
                blocks[index] = basicBlock(player.baseStats[statNames[index - 2]].ToString());
            }

            return new PitcherRow(blocks, player);
        }

        private TextBlock basicBlock(string text)
        {
            return new TextBlock
            {
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                FontSize = 18,
                Text = text
            };
        }

        public class BatterRow
        {
            public TextBlock[] textBlocks;
            PositionPlayer player;

            public BatterRow(TextBlock[] blocks, PositionPlayer inPlayer)
            {
                textBlocks = blocks;
                player = inPlayer;
            }
        }

        public class PitcherRow
        {
            public TextBlock[] textBlocks;
            Pitcher player;

            public PitcherRow(TextBlock[] blocks, Pitcher inPlayer)
            {
                textBlocks = blocks;
                player = inPlayer;
            }
        }
    }
}
