using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        public MainPage()
        {
            this.InitializeComponent();
            activeGame = new Game(new Team("Angels", "LAA"), new Team("Rockies", "COL"));
            awayTrigram.Text = activeGame.getTeam(false).getTrigram();
            homeTrigram.Text = activeGame.getTeam(true).getTrigram();
        }

        private void confirmButtonClick(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            int score = rand.Next(6);
            activeGame.addRuns(score);
            updateScoreDisplay();
            activeGame.endHalfInning();
        }

        private void updateScoreDisplay()
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
    }
}
