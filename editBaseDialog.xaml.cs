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

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SOM_Score_Assistant
{
    public sealed partial class editBaseDialog : ContentDialog
    {
        public editBaseDialog(Dictionary<int, PositionPlayer> bases)
        {
            this.InitializeComponent();

            for (int baseNum = 1; baseNum <= 3; baseNum += 1)
            {
                if(bases[baseNum] != null)
                {
                    BaserunnerSelect.Items.Add(new Tuple<PositionPlayer,int>(bases[baseNum], baseNum));
                }
            }
            BaserunnerSelect.SelectedIndex = 0;
        }

        public Tuple<PositionPlayer, int> getSelectedRunner()
        {
            return (Tuple<PositionPlayer, int>)BaserunnerSelect.SelectedItem;
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
