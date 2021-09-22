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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SOM_Score_Assistant
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StatsSummaryPage : Page
    {
        public StatsSummaryPage()
        {
            this.InitializeComponent();
            Team team = MainPage.activeGame.getTeam(MainPage.home);

            int batIndex = 0;
            int pitchIndex = 0;

            foreach(Tuple<PositionPlayer,int> tuple in team.allPositionPlayers)
            {
                initBatterTableRow(batIndex, tuple.Item1, tuple.Item2);
                batIndex++;
            }

            foreach(Pitcher pitcher in team.getAllPitchers())
            {
                initPitcherTableRow(pitchIndex, pitcher);
                pitchIndex++;
            }
        }

        private void initBatterTableRow(int rowIndex, PositionPlayer player, int lineupNumber)
        {
            //init background color with line beneath
            Border rowBorder = new Border();
            rowBorder.Background = new SolidColorBrush((rowIndex % 2 == 1) ? Windows.UI.Colors.White : Windows.UI.Colors.LightGray);
            rowBorder.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            rowBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            BattingTable.Children.Add(rowBorder);
            Grid.SetRow(rowBorder, rowIndex);
            Grid.SetColumn(rowBorder, 0);
            Grid.SetColumnSpan(rowBorder, BattingTable.ColumnDefinitions.Count);

            TextBox numBox = createBoxInRow(BattingTable, rowIndex, 0);
            numBox.Text = lineupNumber.ToString();

            TextBox nameBox = createBoxInRow(BattingTable, rowIndex, 1);
            nameBox.Text = player.getName();

            TextBox posBox = createBoxInRow(BattingTable, rowIndex, 2);
            posBox.Text = positionIntToString(player.positionIndex);


            for (int statIndex = 0; statIndex < player.fullStats.statsArray.Length; statIndex++)
            {
                TextBox box = createBoxInRow(BattingTable, rowIndex, statIndex + player.fullStats.offset);
                box.Text = player.fullStats.getStat(statIndex).ToString();
            }
        }

        private void initPitcherTableRow(int rowIndex, Pitcher player)
        {
            //init background color with line beneath
            Border rowBorder = new Border();
            rowBorder.Background = new SolidColorBrush((rowIndex % 2 == 1) ? Windows.UI.Colors.White : Windows.UI.Colors.LightGray);
            rowBorder.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            rowBorder.BorderThickness = new Thickness(0, 0, 0, 1);
            PitchingTable.Children.Add(rowBorder);
            Grid.SetRow(rowBorder, rowIndex);
            Grid.SetColumn(rowBorder, 0);
            Grid.SetColumnSpan(rowBorder, PitchingTable.ColumnDefinitions.Count);

            for (int statIndex = 0; statIndex < player.fullStats.statsArray.Length; statIndex++)
            {
                TextBox box = createBoxInRow(PitchingTable, rowIndex, statIndex + player.fullStats.offset);

                box.Text = player.fullStats.getStat(statIndex).ToString();
            }
        }

        private TextBox createBoxInRow(Grid table, int rowIndex, int columnIndex)
        {
            TextBox box = new TextBox();
            box.TextAlignment = TextAlignment.Left;
            box.Background = null;
            box.Text = "0";
            box.Height = 32;
            box.Width = 64;

            Border boxBorder = new Border();
            boxBorder.BorderThickness = new Thickness(0, 0, 2, 0);
            boxBorder.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
            boxBorder.Background = null;

            table.Children.Add(box);
            table.Children.Add(boxBorder);
            Grid.SetColumn(box, columnIndex);
            Grid.SetColumn(boxBorder, columnIndex);
            Grid.SetRow(box, rowIndex);
            Grid.SetRow(boxBorder, rowIndex);

            return box;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //save all values to players
        }

        private string positionIntToString(int positionInt)
        {
            string position = "PH";

            switch (positionInt)
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
            return position;
        }
    }
}
