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
    public sealed partial class AddPlayerDialog : ContentDialog
    {
        public AddPlayerDialog(string title, bool required)
        {
            this.InitializeComponent();
            Input.Focus(FocusState.Programmatic);
            Title = title;
        }

        public string enteredText
        {
            get => (string)Input.Text;
        }

        public Handedness handedness
        {
            get
            {
                foreach(RadioButton radio in new RadioButton[] { LeftHandedSelect, SwitchSelect, RightSelect })
                {
                    if ((bool)radio.IsChecked) 
                    {
                        switch (radio.Content)
                        {
                            case "Left":
                                return Handedness.Left;
                            case "Switch":
                                return Handedness.Switch;
                            case "Right":
                                return Handedness.Right;
                            default:
                                break;
                        } 
                    }
                }
                return Handedness.None;
            }
        }

        public int positionIndex
        {
            get { return PositionDropdown.SelectedIndex; }
        }

        private void Confirm_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void Cancel_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }
    }
}
