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
    public sealed partial class AddTeamDialog : ContentDialog
    {
        public AddTeamDialog(string title)
        {
            this.InitializeComponent();
            InputAwayName.Focus(FocusState.Programmatic);
            Title = title;
        }

        public string[] enteredText
        {
            get => new string[]
            {
                (string)InputAwayName.Text, (string)InputAwayTrigram.Text, (string)InputHomeName.Text, (string)InputHomeTrigram.Text
            };
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }
    }
}
