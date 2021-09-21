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
        public StatsSummaryPage(Team team)
        {
            this.InitializeComponent();

            
        }

        private void initBatterTableRow(Grid table, int rowIndex, PositionPlayer player)
        {
            bool set = false;
            List<TextBox> removeList = new List<TextBox>();
            foreach (object child in table.Children)
            {
                if (child.GetType().Equals(typeof(Border)))
                {
                    Border border = (Border)child;
                    if (Grid.GetRow(border) == rowIndex)
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

            foreach (TextBlock block in removeList)
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
    }
}
