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
    public class Team
    {
        private string name;
        private string trigram;
        private Image logo;

        public List<Tuple<PositionPlayer,int>> allPositionPlayers = new List<Tuple<PositionPlayer,int>>(); // (player, lineup position)
        private List<Pitcher> allPitchers = new List<Pitcher>();

        private PositionPlayer[] lineup = new PositionPlayer[9];
        private Pitcher activePitcher;

        private int lineupPosition = -1;

        public Team(string newName, string newTrigram, string logoFilename)
        {
            setNames(newName, newTrigram);
            setLogoFromFilename(logoFilename);
        }

        public Team(string[] names)
        {
            setNames(names[0], names[1]);
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

        public int getNextEmptyLineup()
        {
            for(int i = 0; i < lineup.Length; i++)
            {
                if(lineup[i] == null)
                {
                    return i;
                }
            }
            return -1;
        }

        public List<Pitcher> getAllPitchers() => allPitchers;

        public bool hasDefender(int positionIndex)
        {
            foreach(PositionPlayer player in lineup)
            {
                if(player != null && player.positionIndex == positionIndex)
                {
                    return true;
                }
            }
            return false;
        }

        public void setLineupPosition(PositionPlayer player, int position)
        {
            lineup[position] = player;
            allPositionPlayers.Add(new Tuple<PositionPlayer, int>(player, position));
        }

        public void swapPlayer(string oldName, PositionPlayer newPlayer)
        {
            for (int lineupPos = 0; lineupPos < 9; lineupPos++)
            {
                if(getNextEmptyLineup() > lineupPos && lineup[lineupPos].getName().Equals(oldName))
                {
                    setLineupPosition(newPlayer, lineupPos);
                }
            }
        }

        public void setPitcher(Pitcher player)
        {
            activePitcher = player;
            allPitchers.Add(player);
        }

        public bool hasNextLineup()
        {
            return lineup[(lineupPosition + 1) % 9] != null;
        }

        public void advanceLineup()
        {
            lineupPosition += 1;
            lineupPosition = lineupPosition % 9;
        }

        public string getName() => name;

        public string getTrigram() => trigram;

        public PositionPlayer[] getLineup() => lineup;

        public int getBatterIndex() => lineupPosition;

        public void setLogoFromFilename(string filename)
        {
            logo = Image.FromFile(filename);
        }

        public Image getLogo() => logo;

        public Pitcher getPitcher() => activePitcher;

        public PositionPlayer getBatter() => lineup[lineupPosition];

        public PositionPlayer getPlayer(string positionName)
        {
            int positionIndex;

            switch (positionName)
            {
                case "C":
                    positionIndex = 0;
                    break;
                case "1B":
                    positionIndex = 1;
                    break;
                case "2B":
                    positionIndex = 2;
                    break;
                case "3B":
                    positionIndex = 3;
                    break;
                case "SS":
                    positionIndex = 4;
                    break;
                case "LF":
                    positionIndex = 5;
                    break;
                case "CF":
                    positionIndex = 6;
                    break;
                case "RF":
                    positionIndex = 7;
                    break;
                case "DH":
                    positionIndex = 8;
                    break;
                default:
                    positionIndex = 9;
                    break;
            }

            foreach (PositionPlayer player in lineup)
            {
                if (player != null && player.positionIndex == positionIndex)
                {
                    return player;
                }
            }
            return null;
        }
    }

    public abstract class Player
    {
        private string name = "";
        private Handedness handedness;
        public int positionIndex;
        public abstract Dictionary<string, int> baseStats { get; set; }

        public Player() { }

        public Player(string initName, Handedness hand)
        {
            name = initName;
            handedness = hand;
        }

        public Player(string initName, Handedness hand, int position) 
        {
            name = initName;
            handedness = hand;
            positionIndex = position;
        }

        public Handedness getHandedness() => handedness;
        public string getName() => name;

        public override string ToString()
        {
            return name;
        }
    }

    public class PositionPlayer : Player
    {
        public PositionPlayer() { }
        public PositionPlayer(string initName, Handedness hand) : base(initName, hand) { }
        public PositionPlayer(string initName, Handedness hand, int position) : base(initName, hand, position) { }

        override public Dictionary<string, int> baseStats { get; set; } = new Dictionary<string, int>()
        {
            {"AB", 0 },
            {"R", 0 },
            {"H", 0 },
            {"RBI", 0 },
            {"BB", 0 },
            {"K", 0 },
        };

        public BatterFullStats fullStats = new BatterFullStats();
    }

    public class Pitcher : Player
    {
        public Pitcher() { }
        public Pitcher(string initName, Handedness hand) : base(initName, hand) { }
        public override Dictionary<string, int> baseStats { get; set; } = new Dictionary<string, int>()
        {
            {"OP", 0 },
            {"H", 0 },
            {"R", 0 },
            {"BB", 0 },
            {"K", 0 },
            {"HR", 0 }
        };

        public PitcherFullStats fullStats = new PitcherFullStats();

        private string decision;

        public override string ToString()
        {
            string handString = getHandedness() == Handedness.Left ? " (L)" : " (R)";
            return getName() + handString;
        }
    }
    public enum Handedness
    {
        Left,
        Right,
        Switch,
        None
    }
}
