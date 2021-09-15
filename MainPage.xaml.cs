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

        List<Game> undoList = new List<Game>();
        private readonly Dictionary<string, string> reminderTexts = new Dictionary<string, string>
        {
            {"chaosCheck", "Have you rolled for chaos?" },
            {"defenseCheck", "What is the state of the defense? Are they holding the runner(s)?" }
        };
        private string readyString = "Ready for another at-bat!";
        bool reminderDisable = false;
        private string[] buttonNames = new string[] { "StealButton", "WalkButton",
        "OutButton", "HitButton", "OtherButton", "SecondBaseButton", "FirstBaseButton", "ThirdBaseButton", "NoneBaseButton", "BasesBackButton", "SetupButton", "SetupButtonTotal",
        "Button0", "Button1", "Button2", "Button3", "Button4", "Button5", "Button6", "Button7", "ButtonP",
        "FlyoutButton", "GroundoutButton", "StrikeoutButton", "FCButton",
        "PinchHitButton", "PitcherSubButton", "PositionChangeButton",
        "ErrorButton", "WildPitchButton"};
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
            BoxBattingAway.Content = teamNames[0] + " Batting";
            BoxBattingHome.Content = teamNames[2] + " Batting";
            BoxPitchingAway.Content = teamNames[0] + " Pitching";
            BoxPitchingHome.Content = teamNames[2] + " Pitching";

            Pitcher startingPitcher = await getPitcherFromInput(String.Format("Starting Pitcher for the {0}:", activeGame.getPitchingTeam().getName()), true);
            PositionPlayer leadoffBatter = await getPositionPlayerFromInput(String.Format("Leadoff batter for the {0}:", activeGame.getBattingTeam().getName()), true);
            activeGame.getPitchingTeam().setPitcher(startingPitcher);
            activeGame.getBattingTeam().setLineupPosition(leadoffBatter, 0);
            updateGame();

            InfoBox.Text = readyString;
            menu.mainMenuEnable();
        }

        private async void totalSetup()
        {
            string[] teamNames = await addTeams();

            activeGame = new Game(new Team(teamNames[0], teamNames[1]), new Team(teamNames[2], teamNames[3]));
            awayTrigram.Text = activeGame.getTeam(false).getTrigram();
            homeTrigram.Text = activeGame.getTeam(true).getTrigram();
            BoxBattingAway.Content = teamNames[0] + " Batting";
            BoxBattingHome.Content = teamNames[2] + " Batting";
            BoxPitchingAway.Content = teamNames[0] + " Pitching";
            BoxPitchingHome.Content = teamNames[2] + " Pitching";

            Pitcher startingPitcherHome = await getPitcherFromInput(String.Format("Starting Pitcher for the {0}:", activeGame.getPitchingTeam().getName()), true);
            Pitcher startingPitcherAway = await getPitcherFromInput(String.Format("Starting Pitcher for the {0}:", activeGame.getBattingTeam().getName()), true);
            foreach (Team team in activeGame.getTeams())
            {
                team.setLineupPosition(await getPositionPlayerFromInput(String.Format("Leadoff batter for the {0}:", team.getName()), true), 0);
                for (int lineupNum = 1; lineupNum <= 8; lineupNum++)
                {
                    team.setLineupPosition(await getPositionPlayerFromInput(String.Format("Batter #{0} for the {1}:", (lineupNum+1).ToString(), team.getName()), true), lineupNum);
                }
            }
            
            activeGame.getPitchingTeam().setPitcher(startingPitcherHome);
            activeGame.getBattingTeam().setPitcher(startingPitcherAway);
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
                    TextBlock currentBlock = null;
                    if(inning < 9 || (inning == 9 && activeGame.getLineScore().inningCount() <= 9))
                    {
                        string shortName = String.Format("{0}{1}Cell", inning, top ? "t" : "b");
                        currentBlock = (TextBlock)this.FindName(shortName);
                        if (currentBlock == null)
                        {
                            currentBlock = new TextBlock();
                            currentBlock.VerticalAlignment = VerticalAlignment.Center;
                            currentBlock.HorizontalAlignment = HorizontalAlignment.Center;
                            currentBlock.Text = "0";
                            currentBlock.FontSize = 18;
                            currentBlock.Margin = new Thickness(0, 0, 0, 0);
                            currentBlock.Name = String.Format("{0}{1}Cell", inning, top ? "t" : "b");
                            LineScore.Children.Add(currentBlock);
                            Grid.SetRow(currentBlock, top ? 1 : 2);
                            Grid.SetColumn(currentBlock, inning);
                        }                      
                    }
                    else
                    {
                        string shortName = String.Format("9{1}Cell", top ? "t" : "b");
                        lastInningLabel.Text = inning.ToString();
                        currentBlock = (TextBlock)this.FindName(shortName);
                    }
                    int score = activeGame.getLineScore().getScore(inning, top);
                    scores[top ? 0 : 1] += score;
                    currentBlock.Text = Convert.ToString(score);
                    currentBlock.Visibility = Visibility.Visible;
                    if (activeGame.topOfInning && inning == activeGame.getLineScore().inningCount() && !top) { currentBlock.Visibility = Visibility.Collapsed; }

                }
            }

            //bold the current inning
            int inningNum = activeGame.getLineScore().inningCount() > 9 ? 9 : activeGame.getLineScore().inningCount();
            Grid table = LineScore;
            List<TextBlock> removeList = new List<TextBlock>();
            foreach (object child in table.Children)
            {
                if (child.GetType().Equals(typeof(TextBlock)))
                {
                    TextBlock block = (TextBlock)child;
                    if (block.Name.Equals(String.Format("{0}{1}Cell", inningNum.ToString(), activeGame.topOfInning ? "t" : "b")))
                    {
                        block.FontWeight = Windows.UI.Text.FontWeights.ExtraBold;
                    }
                    else { block.FontWeight = Windows.UI.Text.FontWeights.Normal; }
                }
            }

            AwayRuns.Text = Convert.ToString(scores[0]);
            HomeRuns.Text = Convert.ToString(scores[1]);
            int[] hits = activeGame.getLineScore().getHits();
            AwayHits.Text = Convert.ToString(hits[0]);
            HomeHits.Text = Convert.ToString(hits[1]);
            int[] errors = activeGame.getLineScore().getErrors();
            AwayErrors.Text = Convert.ToString(errors[0]);
            HomeErrors.Text = Convert.ToString(errors[1]);
        }

        private void updateBoxScore()
        {
            //update lineup
            Team battingTeam = (bool)BoxBattingAway.IsChecked ? activeGame.getTeam(false) : activeGame.getTeam(true);
            Team pitchingTeam = (bool)BoxPitchingAway.IsChecked ? activeGame.getTeam(false) : activeGame.getTeam(true);
            if ((bool)BoxBattingAway.IsChecked)
            {
                BoxBattingAwayIndicator.Fill = new SolidColorBrush(Windows.UI.Colors.LightGray);
                BoxBattingHomeIndicator.Fill = new SolidColorBrush(Windows.UI.Colors.White);
            }
            else
            {
                BoxBattingAwayIndicator.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                BoxBattingHomeIndicator.Fill = new SolidColorBrush(Windows.UI.Colors.LightGray);
            }


            BoxScore.BatterRow[] rows = activeGame.getBoxScore().getBatterRows((bool)BoxBattingAway.IsChecked);

            int emptyLineup = battingTeam.getNextEmptyLineup();
            int lineupBoundary = 0;

            if(emptyLineup >= 0)
            {
                lineupBoundary = emptyLineup;
            }
            else { lineupBoundary = 9; }
            for (int i = 0; i < lineupBoundary; i++)
            {               
                if (i == battingTeam.getBatterIndex())
                {
                    setActiveTableRow(LineupTable, i);
                }
                else
                {
                    setBaseTableRowFormatting(LineupTable, i);
                }
                populateBatterRow(LineupTable, rows[i], i);
            }
            for(int i = lineupBoundary; i < LineupTable.RowDefinitions.Count; i++)
            {
                setBaseTableRowFormatting(LineupTable, i);
                removeTableFormatting(LineupTable, i);
            }
            
            //update batting text

            //update pitching
            if ((bool)BoxPitchingAway.IsChecked)
            {
                BoxPitchingAwayIndicator.Fill = new SolidColorBrush(Windows.UI.Colors.LightGray);
                BoxPitchingHomeIndicator.Fill = new SolidColorBrush(Windows.UI.Colors.White);
            }
            else
            {
                BoxPitchingAwayIndicator.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                BoxPitchingHomeIndicator.Fill = new SolidColorBrush(Windows.UI.Colors.LightGray);
            }

            BoxScore.PitcherRow[] pitcherRows = activeGame.getBoxScore().getPitcherRows((bool)BoxPitchingAway.IsChecked);

            if(pitcherRows != null)
            {
                for (int i = 0; i < pitchingTeam.getAllPitchers().Count; i++)
                {
                    setBaseTableRowFormatting(PitchingTable, i);
                    populatePitcherRow(PitchingTable, pitcherRows[i], i);
                }
            }         

            for(int i = pitchingTeam.getAllPitchers().Count; i < PitchingTable.RowDefinitions.Count; i++)
            {
                setBaseTableRowFormatting(PitchingTable, i);
                removeTableFormatting(PitchingTable, i);
            }
            //update fielding text
        }

        private void populateBatterRow(Grid table, BoxScore.BatterRow row, int rowIndex)
        {
            for(int index = 0; index < row.textBlocks.Length; index++)
            {
                deleteItemInTable(table, rowIndex, index);
                table.Children.Add(row.textBlocks[index]);
                Grid.SetRow(row.textBlocks[index], rowIndex);
                Grid.SetColumn(row.textBlocks[index], index);
            }
        }

        private void populatePitcherRow(Grid table, BoxScore.PitcherRow row, int rowIndex)
        {
            for (int index = 0; index < row.textBlocks.Length; index++)
            {
                deleteItemInTable(table, rowIndex, index);
                table.Children.Add(row.textBlocks[index]);
                Grid.SetRow(row.textBlocks[index], rowIndex);
                Grid.SetColumn(row.textBlocks[index], index);
            }
        }

        private void deleteItemInTable(Grid table, int rowIndex, int columnIndex)
        {
            TextBlock toRemove = null;

            foreach (object child in table.Children)
            {
                if (child.GetType().Equals(typeof(TextBlock)))
                {
                    TextBlock block = (TextBlock)child;
                    if (Grid.GetRow(block) == rowIndex && Grid.GetColumn(block) == columnIndex)
                    {
                        toRemove = block;
                    }
                }
            }

            if(toRemove != null) { table.Children.Remove(toRemove); }
        }

        private void setBaseTableRowFormatting(Grid table, int rowIndex)
        {
            bool set = false;
            List<TextBlock> removeList = new List<TextBlock>();
            foreach(object child in table.Children)
            {
                if(child.GetType().Equals(typeof(Border))) 
                {
                    Border border = (Border)child;
                    if(Grid.GetRow(border) == rowIndex)
                    {
                        border.Background = new SolidColorBrush((rowIndex % 2 == 1) ? Windows.UI.Colors.White : Windows.UI.Colors.LightGray);
                        border.BorderThickness = new Thickness(0, 0, 0, 1);
                        border.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                        set = true;
                    }
                }
                else if (child.GetType().Equals(typeof(TextBlock)) && Grid.GetRow((TextBlock)child) == rowIndex)
                {
                    removeList.Add((TextBlock)child);
                }
            }

            foreach(TextBlock block in removeList)
            {
                table.Children.Remove(block);
            }

            if (!set)
            {
                Border rowBorder = new Border();
                rowBorder.Background = new SolidColorBrush((rowIndex % 2 == 1) ? Windows.UI.Colors.White : Windows.UI.Colors.LightGray);
                rowBorder.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                rowBorder.BorderThickness = new Thickness(0, 0, 0, 1);
                table.Children.Add(rowBorder);
                Grid.SetRow(rowBorder, rowIndex);
                Grid.SetColumn(rowBorder, 0);
                Grid.SetColumnSpan(rowBorder, table.ColumnDefinitions.Count);
            }     
            
        }

        private void removeTableFormatting(Grid table, int rowIndex)
        {
            Border remBorder = null;
            foreach (object child in table.Children)
            {
                if (child.GetType().Equals(typeof(Border)))
                {
                    Border border = (Border)child;
                    if (Grid.GetRow(border) == rowIndex)
                    {
                        remBorder = border;
                    }
                }
            }
            if(remBorder != null) { table.Children.Remove(remBorder); }
        }

        private void setActiveTableRow(Grid table, int rowIndex)
        {
            bool set = false;
            foreach (object child in table.Children)
            {
                if (child.GetType().Equals(typeof(Border)))
                {
                    Border border = (Border)child;
                    if (Grid.GetRow(border) == rowIndex)
                    {
                        border.Background = new SolidColorBrush(Windows.UI.Colors.LightCoral);
                        set = true;
                    }
                }
            }
            if (!set)
            {
                setBaseTableRowFormatting(table, rowIndex);
                setActiveTableRow(table, rowIndex);
            }
        }

        private void updateBases()
        {
            foreach(int baseNum in activeGame.bases.Keys)
            {
                TextBlock baseName = (TextBlock)this.FindName("Baserunner" + Convert.ToString(baseNum));
                if (activeGame.bases[baseNum] != null)
                {                  
                    baseName.Text = activeGame.bases[baseNum].ToString();
                }
                else
                {
                    baseName.Text = "";
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

        private void addBatter()
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

        private async void stealButtonClick(object sender, RoutedEventArgs e)
        {
            for(int i = 3; i >= 1; i--) 
            {
                if(activeGame.bases[i] != null && (i == 3 || activeGame.bases[i+1] == null))
                {
                    await stealAttempt(activeGame.bases[i], i);
                }
            }
            updateUI();
        }

        private async Task<bool> stealAttempt(PositionPlayer runner, int startBase)
        {
            ContentDialogResult result = await baserunnerAdvanceQuery(new string[] { String.Format("What happened to {0}?", runner.ToString()), "Stolen Base", "Caught Stealing", "Nothing" });
            if(result == ContentDialogResult.Primary)
            {
                if (startBase < 3)
                {
                    activeGame.bases[startBase + 1] = runner;                    
                }
                else
                {
                    activeGame.addRuns(1);
                    runner.baseStats["R"] += 1;
                }
                activeGame.bases[startBase] = null;
                activeGame.getBoxScore().awayTextStatsBatting.addValue("SB", runner, 1);
                return true;
            }
            else if(result == ContentDialogResult.Secondary)
            {
                activeGame.getPitcher().baseStats["OP"] += 1;
                activeGame.bases[startBase] = null;
                activeGame.getBoxScore().awayTextStatsBatting.addValue("CS", runner, 1);
                activeGame.outs += 1;
                return true;
            }
            return false;
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
            menu.otherButtonsEnable();
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
            
            if(menu.state == QueryStates.Batter || menu.state == QueryStates.Error)
            {
                int runs = 0;
                if(menu.state == QueryStates.Batter)
                {
                    activeGame.getBatter().baseStats["H"] += 1;
                    activeGame.getPitcher().baseStats["H"] += 1;
                    activeGame.getLineScore().addHit(activeGame.topOfInning);
                    activeGame.getBattingBoxScore().addValue("TB", activeGame.getBatter(), baseNum);
                }
                activeGame.getBatter().baseStats["AB"] += 1;              
                if (baseNum == 4)
                {
                    activeGame.getBattingBoxScore().addValue("HR", activeGame.getBatter(), 1);
                    activeGame.getPitcher().baseStats["HR"] += 1;
                    runs = 1;
                    for(int baseI = 1; baseI <= 3; baseI++)
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

        private void addUndoList()
        {

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
                //addUndoList();

                if(activeGame.outs >= 3)
                {
                    activeGame.endHalfInning();
                    BoxBattingAway.IsChecked = activeGame.topOfInning;
                    BoxBattingHome.IsChecked = !activeGame.topOfInning;
                    BoxPitchingAway.IsChecked = !activeGame.topOfInning;
                    BoxPitchingHome.IsChecked = activeGame.topOfInning;
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
                        activeGame.getBattingTeam().setLineupPosition(newPlayer, activeGame.getBattingTeam().getNextEmptyLineup());
                        set = true;                   
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

                activeGame.getBattingTeam().advanceLineup();

                updateUI();
            }

            if (activeGame.final)
            {
                menu.disableAll();
                InfoBox.Text = "Final!";
            }
        }

        private void updateUI()
        {
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
            updateLineScore();
            updateBoxScore();
            updateBases();
            updateBatter();
            updatePitcher();
            updateOuts();
        }

        private void updateOuts()
        {
            if (activeGame.outs == 2)
            {
                Out2.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
                Out1.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
            }
            else if (activeGame.outs == 1)
            {
                Out2.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                Out1.Fill = new SolidColorBrush(Windows.UI.Colors.Black);
            }
            else
            {
                Out2.Fill = new SolidColorBrush(Windows.UI.Colors.White);
                Out1.Fill = new SolidColorBrush(Windows.UI.Colors.White);
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
            if (((Button)sender).Name.Equals("SetupButtonTotal"))
            {
                totalSetup();
            }
            else { gameSetup(); }
        }

        private void FielderButton_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            InfoBox.Text = buttonName;
        }

        private async void OutTypeButton_Click(object sender, RoutedEventArgs e)
        {
            string buttonName = ((Button)sender).Name;
            activeGame.getPitcher().baseStats["OP"] += 1;
            activeGame.outs += 1;

            if(buttonName == "StrikeoutButton")
            {
                activeGame.getPitcher().baseStats["K"] += 1;
                activeGame.getBatter().baseStats["K"] += 1;
                activeGame.getBatter().baseStats["AB"] += 1;
            }
            else if(buttonName == "GroundoutButton")
            {
                bool sac = false;
                if (!areBaserunners() || activeGame.outs >= 3)
                {
                    //nothing special happens here
                }
                else
                {
                    sac = await outWithBaserunners();   
                }

                if (!sac)
                {
                    activeGame.getBatter().baseStats["AB"] += 1;
                }
                else
                {
                    activeGame.getBatter().baseStats["RBI"] += 1;
                    activeGame.addRuns(1);
                }
            }
            else if(buttonName == "FlyoutButton")
            {
                bool sac = false;
                if (!areBaserunners() || activeGame.outs >= 3)
                {
                    //nothing special happens here
                }
                else
                {
                    sac = await outWithBaserunners();
                }

                if (!sac)
                {
                    activeGame.getBatter().baseStats["AB"] += 1;
                }
                else
                {
                    activeGame.getBatter().baseStats["RBI"] += 1;
                    activeGame.addRuns(1);
                }
            }
            else if(buttonName == "FCButton")
            {
                activeGame.getPitcher().baseStats["OP"] -= 1;
                activeGame.outs -= 1;
                bool score = await outWithBaserunners();
                activeGame.bases[1] = activeGame.getBatter();
            }

            updateGame();
        }

        public async Task<bool> outWithBaserunners()
        {
            bool sac = false;
            if (thirdOccupied())
            {
                ContentDialogResult result = await baserunnerAdvanceQuery(new string[] { String.Format("What happened to {0}, the runner on third?", activeGame.bases[3].ToString()), "Scored", "Thrown Out", "Held" });
                if (result == ContentDialogResult.Primary)
                {
                    sac = true;
                    activeGame.bases[3].baseStats["R"] += 1;
                    activeGame.bases[3] = null;
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    activeGame.bases[3] = null;
                    activeGame.outs += 1;
                    activeGame.getPitcher().baseStats["OP"] += 1;
                    activeGame.getBattingBoxScore().addValue("GIDP", activeGame.getBatter(), 1);
                }
            }
            for (int baseI = 2; baseI >= 1; baseI--)
            {
                if (activeGame.bases[baseI] != null && activeGame.bases[baseI + 1] == null)
                {
                    ContentDialogResult result = await baserunnerAdvanceQuery(new string[] { String.Format("What happened to {0}?", activeGame.bases[baseI].ToString()), "Advanced", "Thrown Out", "Held" });
                    if (result == ContentDialogResult.Primary)
                    {
                        activeGame.bases[baseI + 1] = activeGame.bases[baseI];
                        activeGame.bases[baseI] = null;
                    }
                    else if (result == ContentDialogResult.Secondary)
                    {
                        activeGame.bases[baseI] = null;
                        activeGame.outs += 1;
                        activeGame.getPitcher().baseStats["OP"] += 1;
                        activeGame.getBattingBoxScore().addValue("GIDP", activeGame.getBatter(), 1);
                    }
                }
            }
            return sac;
        }

        public async Task<bool> fcOut()
        {
            bool score = false;
            if (thirdOccupied())
            {
                ContentDialogResult result = await baserunnerAdvanceQuery(new string[] { String.Format("What happened to {0}, the runner on third?", activeGame.bases[3].ToString()), "Scored", "Thrown Out", "Held" });
                if (result == ContentDialogResult.Primary)
                {
                    score = true;
                    activeGame.bases[3].baseStats["R"] += 1;
                    activeGame.bases[3] = null;
                }
                else if (result == ContentDialogResult.Secondary)
                {
                    activeGame.bases[3] = null;
                    activeGame.outs += 1;
                    activeGame.getPitcher().baseStats["OP"] += 1;
                }
            }
            for (int baseI = 2; baseI >= 1; baseI--)
            {
                if (activeGame.bases[baseI] != null && activeGame.bases[baseI + 1] == null)
                {
                    ContentDialogResult result = await baserunnerAdvanceQuery(new string[] { String.Format("What happened to {0}?", activeGame.bases[baseI].ToString()), "Advanced", "Thrown Out", "Held" });
                    if (result == ContentDialogResult.Primary)
                    {
                        activeGame.bases[baseI + 1] = activeGame.bases[baseI];
                        activeGame.bases[baseI] = null;
                    }
                    else if (result == ContentDialogResult.Secondary)
                    {
                        activeGame.bases[baseI] = null;
                        activeGame.outs += 1;
                        activeGame.getPitcher().baseStats["OP"] += 1;
                    }
                }
            }
            return score;
        }

        /// <summary>
        /// shows a dialog box to ask about a baserunner.
        /// </summary>
        /// <param name="text">[Title, PrimaryText, SecondaryText, CloseText]</param>
        /// <returns>ContentDialogResult</returns>
        private async Task<ContentDialogResult> baserunnerAdvanceQuery(string[] text)
        {
            ContentDialog askBox = new ContentDialog()
            {
                Title = text[0],
                PrimaryButtonText = text[1],
                SecondaryButtonText = text[2],
                CloseButtonText = text[3]
            };
            return await askBox.ShowAsync();
        }

        private async Task<ContentDialogResult> wildPitchAdvanceQuery()
        {
            ContentDialog askBox = new ContentDialog()
            {
                Title = "Did the runners advance?",
                PrimaryButtonText = "Yes!",
                CloseButtonText = "No :("
            };
            return await askBox.ShowAsync();
        }

        private void WalkButton_Click(object sender, RoutedEventArgs e)
        {
            int maxWalkers = 1;
            for(int baseNum = 1; baseNum <= 3; baseNum++)
            {
                if(activeGame.bases[baseNum] == null)
                {
                    maxWalkers = baseNum;
                    break;
                }
            }
            if(maxWalkers == 4)
            {
                activeGame.bases[3].baseStats["R"] += 1;
                activeGame.bases[3] = null;
                activeGame.getBatter().baseStats["RBI"] += 1;
                activeGame.addRuns(1);
                maxWalkers = 3;
            }
            for(int walkies = maxWalkers; walkies >= 2; walkies--)
            {
                activeGame.bases[walkies] = activeGame.bases[walkies - 1];
                activeGame.bases[walkies - 1] = null;
            }
            activeGame.bases[1] = activeGame.getBatter();
            activeGame.getBatter().baseStats["BB"] += 1;
            activeGame.getPitcher().baseStats["BB"] += 1;

            updateGame();
        }

        private void UndoButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BoxBatting_Checked(object sender, RoutedEventArgs e)
        {
            if(activeGame != null) { updateBoxScore(); }
        }

        private async void SubstitutionButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if(button.Name == "PitcherSubButton")
            {
                activeGame.getPitchingTeam().setPitcher(await getPitcherFromInput(String.Format("Pitching in relief for the {0}", activeGame.getPitchingTeam().getName()), true));
            }
            else if(button.Name == "PinchHitButton")
            {
                activeGame.getBattingTeam().setLineupPosition(await getPositionPlayerFromInput(String.Format("Pinch hitting for {0}:", activeGame.getBattingTeam().getBatter().ToString()), true), activeGame.getBattingTeam().getBatterIndex());
            }
            else if(button.Name == "PositionChangeButton")
            {
                if(activeGame.getPitchingTeam().getNextEmptyLineup() > 0)
                {
                    PlayerEdit(activeGame.getPitchingTeam(), true);
                }              
            }
            updateUI();
            menu.mainMenuEnable();
        }

        private async void PlayerEdit(Team team, bool lineup)
        {
            EditPlayerDialog dialog = new EditPlayerDialog(team);
            ContentDialogResult result = await dialog.ShowAsync();
            if(result == ContentDialogResult.Primary)
            {
                dialog.selectedPlayer().positionIndex = dialog.selectedPosition();
            }
        }

        private async void OtherTypeButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if(button.Name == "WildPitchButton")
            {
                ContentDialogResult result = await wildPitchAdvanceQuery();
                if(result == ContentDialogResult.Primary)
                {
                    for(int baseNum = 3; baseNum >= 1; baseNum--)
                    {
                        if(activeGame.bases[baseNum] != null)
                        {
                            if(baseNum == 3)
                            {
                                activeGame.bases[baseNum].baseStats["R"] += 1;
                                activeGame.addRuns(1);
                                
                            }
                            else { activeGame.bases[baseNum + 1] = activeGame.bases[baseNum]; }
                            activeGame.bases[baseNum] = null;
                        }
                    }
                }
                updateUI();
            }
            else if(button.Name == "ErrorButton")
            {
                menu.state = QueryStates.Error;
                menu.baseButtonsEnable(false);
                activeGame.getLineScore().addError(activeGame.topOfInning);
                InfoBox.Text = String.Format("Where did {0} end up after the error?", activeGame.getBatter());
            }       
        }

        private void BoxPitching_Checked(object sender, RoutedEventArgs e)
        {
            if (activeGame != null) { updateBoxScore(); }
        }
    }
}
