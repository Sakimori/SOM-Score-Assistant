using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace SOM_Score_Assistant
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Game activeGame;
        private readonly Dictionary<string, string> reminderTexts = new Dictionary<string, string>
        {
            {"chaosCheck", "Have you rolled for chaos?" },
            {"defenseCheck", "What is the state of the defense? Are they holding the runner(s)?" }
        };
        private string readyString = "Ready for another at-bat!";
        bool reminderDisable = false;
        private string[] buttonNames = new string[] { "StealButton", "WalkButton",
        "OutButton", "HitButton", "OtherButton", "SecondBaseButton", "FirstBaseButton", "ThirdBaseButton", "NoneBaseButton", "BasesBackButton", "SetupButton",
        "Button0", "Button1", "Button2", "Button3", "Button4", "Button5", "Button6", "Button7", "ButtonP",
        "FlyoutButton", "GroundoutButton", "StrikeoutButton"};
        MenuManager menu;

        private List<int> baserunnersToCheck = new List<int>();
        private (int, PositionPlayer) queuedBaserunner = (-1, null);

        
        public MainPage()
        {
            this.InitializeComponent();
            InfoBox.Text = "Waiting for game setup...";
            
            Dictionary<string, Button> menuButtons = new Dictionary<string, Button>();
            foreach (string name in buttonNames)
            {
                menuButtons.Add(name, (Button)this.FindName(name));
            }
            menu = new MenuManager(menuButtons);
        }

        private async void gameSetup()
        {
            string[] teamNames = await addTeams();

            activeGame = new Game(new Team(teamNames[0], teamNames[1]), new Team(teamNames[2], teamNames[3]));
            awayTrigram.Text = activeGame.getTeam(false).getTrigram();
            homeTrigram.Text = activeGame.getTeam(true).getTrigram();

            Pitcher startingPitcher = await getPitcherFromInput(String.Format("Starting Pitcher for the {0}:", activeGame.getPitchingTeam().getName()), true);
            PositionPlayer leadoffBatter = await getPositionPlayerFromInput(String.Format("Leadoff batter for the {0}:", activeGame.getBattingTeam().getName()), true);
            activeGame.getPitchingTeam().setPitcher(startingPitcher);
            activeGame.getBattingTeam().setLineupPosition(leadoffBatter, 0);
            updateGame();

            InfoBox.Text = readyString;
            menu.mainMenuEnable();
        }

        private void updateLineScore()
        {
            int[] scores = new int[] { 0, 0 };
            for (int inning = 1; inning <= activeGame.getLineScore().inningCount(); inning++)
            {
                foreach (bool top in new bool[] { true, false })
                {
                    string shortName = String.Format("{0}{1}Cell", inning, top ? "t" : "b");
                    TextBlock currentBlock = (TextBlock)this.FindName(shortName);
                    if (currentBlock == null)
                    {
                        currentBlock = new TextBlock();
                        currentBlock.VerticalAlignment = VerticalAlignment.Center;
                        currentBlock.HorizontalAlignment = HorizontalAlignment.Center;
                        currentBlock.Text = "0";
                        currentBlock.FontSize = 18;
                        currentBlock.Margin = new Thickness(0,0,0,0);
                        currentBlock.Name = String.Format("{0}{1}Cell", inning, top ? "t" : "b");
                        LineScore.Children.Add(currentBlock);
                        Grid.SetRow(currentBlock, top ? 1 : 2);
                        Grid.SetColumn(currentBlock, inning);
                    }
                    int score = activeGame.getLineScore().getScore(inning, top);
                    scores[top ? 0 : 1] += score;
                    currentBlock.Text = Convert.ToString(score);
                    currentBlock.Visibility = Visibility.Visible;
                    if (activeGame.topOfInning && inning == activeGame.getLineScore().inningCount() && !top) { currentBlock.Visibility = Visibility.Collapsed; }
                }
            }
            AwayRuns.Text = Convert.ToString(scores[0]);
            HomeRuns.Text = Convert.ToString(scores[1]);
        }

        private void updateBases()
        {
            foreach(int baseNum in activeGame.bases.Keys)
            {
                if(activeGame.bases[baseNum] != null)
                {
                    TextBlock baseName = (TextBlock)this.FindName("Baserunner" + Convert.ToString(baseNum));
                    baseName.Text = activeGame.bases[baseNum].ToString();
                }
            }
        }

        private void updatePitcher() => ((TextBlock)this.FindName("PitcherName")).Text = activeGame.getPitcher().ToString();

        private void updateBatter()
        {
            TextBlock updateBlock;
            TextBlock otherBlock;
            PositionPlayer batter = activeGame.getBatter();
            if(batter.getHandedness() == Handedness.Left || (batter.getHandedness() == Handedness.Switch && activeGame.getPitcher().getHandedness() == Handedness.Right))
            {
                updateBlock = BatterLeftHanded;
                otherBlock = BatterRightHanded;
            }
            else 
            { 
                updateBlock = BatterRightHanded;
                otherBlock = BatterLeftHanded;
            }
            updateBlock.Text = batter.ToString();
            otherBlock.Text = "";
        }
        
        private bool areBaserunners()
        {
            foreach(int checkBase in activeGame.bases.Keys)
            {
                if(activeGame.bases[checkBase] != null) { return true; }
            }
            return false;
        }

        private bool thirdOccupied()
        {
            return activeGame.bases[3] != null;
        }

        private async void addBatter()
        {

        }
        private void confirmButtonClick(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            int score = rand.Next(6);
            activeGame.addRuns(score);
            activeGame.endHalfInning();

            reminderDisable = (bool)ReminderCheckBox.IsChecked;
        }
        private void OutButton_Click(object sender, RoutedEventArgs e)
        {
            menu.state = QueryStates.OutType;
            menu.outTypeButtonsEnable();
        }

        private void HitButton_Click(object sender, RoutedEventArgs e)
        {
            menu.state = QueryStates.Batter;
            menu.baseButtonsEnable(true);
        }

        private void stealButtonClick(object sender, RoutedEventArgs e)
        {
            
        }

        private async Task<PositionPlayer> getPositionPlayerFromInput(string title, bool required)
        {
            bool set = false;
            while (!set)
            {
                if (required)
                {
                    var reqPopup = new AddPlayerDialog(title, true);
                    
                    var result = await reqPopup.ShowAsync();
                    if (result == ContentDialogResult.Primary && reqPopup.enteredText.Length > 0 && reqPopup.handedness != Handedness.None && reqPopup.positionIndex != -1)
                    {
                        return new PositionPlayer((string)reqPopup.enteredText, reqPopup.handedness, reqPopup.positionIndex);
                    }                 
                }
                else
                {
                    var addPopup = new AddPlayerDialog(title, false);
                    
                    var result = await addPopup.ShowAsync();
                    if (result == ContentDialogResult.Primary && addPopup.enteredText.Length > 0 && addPopup.handedness != Handedness.None && addPopup.positionIndex != -1)
                    {
                        //do stuff here with player
                        return new PositionPlayer((string)addPopup.enteredText, addPopup.handedness, addPopup.positionIndex);
                    }
                    else { return null; }
                }
            }
            return null;
        }

        private async Task<Pitcher> getPitcherFromInput(string title, bool required)
        {
            bool set = false;
            while (!set)
            {
                if (required)
                {
                    var reqPopup = new AddPlayerDialog(title, true);
                    ((ComboBox)reqPopup.FindName("PositionDropdown")).Visibility = Visibility.Collapsed;
                    ((RadioButton)reqPopup.FindName("SwitchSelect")).Visibility = Visibility.Collapsed;
                    ((RadioButton)reqPopup.FindName("SwitchSelect")).IsEnabled = false;
                    var result = await reqPopup.ShowAsync();
                    if (result == ContentDialogResult.Primary && reqPopup.enteredText.Length > 0 && reqPopup.handedness != Handedness.None)
                    {
                        return new Pitcher((string)reqPopup.enteredText, reqPopup.handedness);
                    }
                }
                else
                {
                    var addPopup = new AddPlayerDialog(title, false);
                    ((ComboBox)addPopup.FindName("PositionDropdown")).Visibility = Visibility.Collapsed;
                    ((RadioButton)addPopup.FindName("SwitchSelect")).Visibility = Visibility.Collapsed;
                    ((RadioButton)addPopup.FindName("SwitchSelect")).IsEnabled = false;
                    var result = await addPopup.ShowAsync();
                    if (result == ContentDialogResult.Primary && addPopup.enteredText.Length > 0 && addPopup.handedness != Handedness.None)
                    {
                        //do stuff here with player
                        return new Pitcher((string)addPopup.enteredText, addPopup.handedness);
                    }
                    else { return null; }
                }
            }
            return null;
        }

        private async Task<string[]> addTeams()
        {
            bool set = false;
            string[] output = new string[4];
            while(!set)
            {
                var teamPopup = new AddTeamDialog("Team Input:");
                var result = await teamPopup.ShowAsync();
                if(teamPopup.enteredText[0].Length > 0 && teamPopup.enteredText[1].Length > 0 && teamPopup.enteredText[2].Length > 0 && teamPopup.enteredText[3].Length > 0)
                {
                    output = teamPopup.enteredText;
                    set = true;
                }
            }
            return output;
        }

        private void OtherButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BaseButton_Click(object sender, RoutedEventArgs e)
        {          
            string buttonName = ((Button)sender).Name;
            int baseNum;

            switch (buttonName)
            {
                case "FirstBaseButton":
                    baseNum = 1;
                    break;
                case "SecondBaseButton":
                    baseNum = 2;
                    break;
                case "ThirdBaseButton":
                    baseNum = 3;
                    break;
                default:
                    baseNum = 4;
                    break;
            }
            
            if(menu.state == QueryStates.Batter)
            {
                int runs = 0;
                activeGame.getBatter().baseStats["AB"] += 1;
                activeGame.getBatter().baseStats["H"] += 1;
                activeGame.getBattingBoxScore().addValue("TB", activeGame.getBatter(), baseNum);
                if (baseNum == 4)
                {
                    activeGame.getBattingBoxScore().addValue("HR", activeGame.getBatter(), 1);
                    runs = 1;
                    foreach(int baseI in activeGame.bases.Keys)
                    {
                        if(activeGame.bases[baseI] != null)
                        {
                            runs += 1;
                            activeGame.bases[baseI].baseStats["R"] += 1;
                            activeGame.bases[baseI] = null;
                        }
                    }
                }
                else if (baseNum == 3)
                {
                    activeGame.getBattingBoxScore().addValue("3B", activeGame.getBatter(), 1);
                    for(int baseI = 1; baseI <= 3; baseI++)
                    {
                        if (activeGame.bases[baseI] != null)
                        {
                            runs += 1;
                            activeGame.bases[baseI].baseStats["R"] += 1;
                            activeGame.bases[baseI] = null;
                        }
                    }
                    activeGame.bases[3] = activeGame.getBatter();
                }
                else if (baseNum == 2)
                {
                    activeGame.getBattingBoxScore().addValue("2B", activeGame.getBatter(), 1);
                    for(int baseI = 2; baseI <= 3; baseI++)
                    {
                        if (activeGame.bases[baseI] != null)
                        {
                            runs += 1;
                            activeGame.bases[baseI].baseStats["R"] += 1;
                            activeGame.bases[baseI] = null;
                        }
                    }
                    if(activeGame.bases[1] != null)
                    {
                        baserunnersToCheck.Add(1);
                        queuedBaserunner = (2, activeGame.getBatter());
                    }
                    else { activeGame.bases[2] = activeGame.getBatter(); }
                }
                else if (baseNum == 1)
                {
                    if(activeGame.bases[3] != null)
                    {
                        runs += 1;
                        activeGame.bases[3].baseStats["R"] += 1;
                        activeGame.bases[3] = null;
                    }
                    for(int baseI = 2; baseI >= 1; baseI--)
                    {
                        if(activeGame.bases[baseI] != null) { baserunnersToCheck.Add(baseI); }
                    }
                    if(baserunnersToCheck.Count > 0)
                    {
                        queuedBaserunner = (1, activeGame.getBatter());
                    }
                    else { activeGame.bases[1] = activeGame.getBatter(); }
                }
                activeGame.getBatter().baseStats["RBI"] += runs;
                activeGame.getBattingBoxScore().addValue("RBI", activeGame.getBatter(), runs);
                activeGame.addRuns(runs);
            }

            else if(menu.state == QueryStates.Baserunner)
            {             
                int originBase = baserunnersToCheck.First<int>();
                PositionPlayer runner = activeGame.bases[originBase];
                if (baseNum == 4)
                {
                    if (queuedBaserunner.Item2 != null)
                    {
                        queuedBaserunner.Item2.baseStats["RBI"] += 1;
                        activeGame.getBattingBoxScore().addValue("RBI", queuedBaserunner.Item2, 1);
                        runner.baseStats["R"] += 1;
                        activeGame.addRuns(1);
                        activeGame.bases[originBase] = null;
                        baserunnersToCheck.RemoveAt(0);
                    }
                }
                else if (baseNum > originBase && activeGame.bases[baseNum] == null && baseNum > queuedBaserunner.Item1)
                {
                    activeGame.bases[baseNum] = runner;
                    activeGame.bases[originBase] = null;
                    baserunnersToCheck.RemoveAt(0);
                }           
            }

            updateGame();
            return;
        }

        private async void updateGame()
        {
            if(baserunnersToCheck.Count > 0)
            {
                menu.state = QueryStates.Baserunner;
                menu.baseButtonsNoCancel(false);
                InfoBox.Text = String.Format("What base did {0} reach?", activeGame.bases[baserunnersToCheck.First<int>()].ToString());
            }
            else if(queuedBaserunner.Item1 > 0)
            {
                activeGame.bases[queuedBaserunner.Item1] = queuedBaserunner.Item2;
                queuedBaserunner = (-1, null);
                updateGame();
            }
            else
            {
                if(activeGame.outs >= 3)
                {
                    activeGame.endHalfInning();
                }

                if(activeGame.outs == 2) 
                { 
                    Out2.Fill = new SolidColorBrush(Windows.UI.Colors.Black); 
                    Out1.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
                }
                else if(activeGame.outs == 1)
                {
                    Out2.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                    Out1.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
                }
                else
                {
                    Out2.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                    Out1.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                }

                if(activeGame.getPitcher() == null)
                {
                    bool set = false;                 
                    while (!set)
                    {
                        Pitcher newPlayer = await getPitcherFromInput(String.Format("Please input the pitcher for the {0}.", activeGame.getPitchingTeam().getName()), true);
                        activeGame.getPitchingTeam().setPitcher(newPlayer);
                        set = true;
                    }
                }

                if (!activeGame.getBattingTeam().hasNextLineup())
                {
                    bool set = false;
                    PositionPlayer newPlayer = await getPositionPlayerFromInput(String.Format("Please input the next batter for the {0}.", activeGame.getBattingTeam().getName()), true);
                    while (!set)
                    {
                        
                        if (!activeGame.getBattingTeam().hasDefender(newPlayer.positionIndex))
                        {
                            activeGame.getBattingTeam().setLineupPosition(newPlayer, activeGame.getBattingTeam().getNextEmptyLineup());
                            set = true;
                        }
                        else { newPlayer = await getPositionPlayerFromInput(String.Format("The {0} already have that defensive position.", activeGame.getBattingTeam().getName()), true); }                      
                    }                 
                }

                PitcherName.Text = activeGame.getPitchingTeam().getPitcher().ToString();
                TextBlock[] baseBoxes = new TextBlock[] { Baserunner1, Baserunner2, Baserunner3 };
                for(int baseI = 1; baseI <= 3; baseI++)
                {
                    if (activeGame.bases[baseI] == null)
                    {
                        baseBoxes[baseI - 1].Text = "";
                    }
                    else
                    {
                        baseBoxes[baseI - 1].Text = activeGame.bases[baseI].ToString();
                    }
                }

                InfoBox.Text = "";
                if (!reminderDisable)
                {
                    if (areBaserunners())
                    {
                        InfoBox.Text += reminderTexts["chaosCheck"];
                        InfoBox.Text += " " + reminderTexts["defenseCheck"] + " ";
                    }
                }
                InfoBox.Text += readyString;
                menu.state = QueryStates.MainMenu;
                menu.mainMenuEnable();
                activeGame.getBattingTeam().advanceLineup();
                updateLineScore();
                updateBases();
                updateBatter();
                updatePitcher();
            }
        }

        private void BasesBackButton_Click(object sender, RoutedEventArgs e)
        {
            if (!menu.locked)
            {
                menu.state = QueryStates.MainMenu;
                menu.mainMenuEnable();
            }
        }

        private void SetupButton_Click(object sender, RoutedEventArgs e)
        {
            gameSetup();
        }

        private void FielderButton_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            InfoBox.Text = buttonName;
        }

        private void OutTypeButton_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            if(buttonName == "StrikeoutButton")
            {
                activeGame.getPitcher().baseStats["K"] += 1;
                activeGame.getBatter().baseStats["K"] += 1;
                activeGame.getBatter().baseStats["AB"] += 1;
                activeGame.outs += 1;
                updateGame();
            }
            else if(buttonName == "GroundoutButton")
            {
                
            }
        }

        private void WalkButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
