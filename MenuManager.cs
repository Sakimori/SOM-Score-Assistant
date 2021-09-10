using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace SOM_Score_Assistant
{
    class MenuManager
    {
        public QueryStates state = QueryStates.MainMenu;
        public bool locked = false;
        public int queryBase = 0;
        public List<Button> activeButtons = new List<Button>();
        private Dictionary<string, Button> buttons;

        public MenuManager(Dictionary<string, Button> inButtons)
        {
            buttons = inButtons;
            setupButtonEnable();
        }

        public void disableAll()
        {
            foreach (Button button in buttons.Values)
            {
                button.IsEnabled = false;
                button.Visibility = Visibility.Collapsed;
            }
            int count = activeButtons.Count;
            for(int i = 0; i < count; i++)
            {
                activeButtons.RemoveAt(0);
            }
        }

        private void enableActive()
        {
            foreach (Button button in activeButtons)
            {
                button.Visibility = Visibility.Visible;
                button.IsEnabled = true;
            }
        }

        public void enableSelected(List<string> buttonNames)
        {
            foreach(string name in buttonNames)
            {
                if (buttons.ContainsKey(name))
                {
                    buttons[name].Visibility = Visibility.Visible;
                    buttons[name].IsEnabled = true;
                    activeButtons.Add(buttons[name]);
                }
            }
        }
        public void mainMenuEnable()
        {
            disableAll();
            activeButtons.Add(buttons["HitButton"]);
            activeButtons.Add(buttons["OutButton"]);
            activeButtons.Add(buttons["WalkButton"]);
            activeButtons.Add(buttons["StealButton"]);
            activeButtons.Add(buttons["OtherButton"]);
            enableActive();
        }

        public void otherButtonsEnable()
        {
            disableAll();
            activeButtons.Add(buttons["PinchHitButton"]);
            activeButtons.Add(buttons["PitcherSubButton"]);
            activeButtons.Add(buttons["PositionChangeButton"]);
            activeButtons.Add(buttons["ErrorButton"]);
            activeButtons.Add(buttons["WildPitchButton"]);
            activeButtons.Add(buttons["BasesBackButton"]);
            enableActive();
        }

        public void setupButtonEnable()
        {
            disableAll();
            activeButtons.Add(buttons["SetupButton"]);
            enableActive();
        }
        /// <summary>
        /// If hit is true, buttons will say "single" "double" etc. Otherwise, they will have base names on them.
        /// </summary>
        public void baseButtonsEnable(bool hit)
        {
            disableAll();
            buttons["FirstBaseButton"].Content = hit ? "Single" : "First";
            buttons["SecondBaseButton"].Content = hit ? "Double" : "Second";
            buttons["ThirdBaseButton"].Content = hit ? "Triple" : "Third";
            buttons["NoneBaseButton"].Content = hit ? "Dinger" : "Home";
            activeButtons.Add(buttons["FirstBaseButton"]);
            activeButtons.Add(buttons["SecondBaseButton"]);
            activeButtons.Add(buttons["ThirdBaseButton"]);
            activeButtons.Add(buttons["NoneBaseButton"]);
            activeButtons.Add(buttons["BasesBackButton"]);
            enableActive();
        }

        public void baseButtonsNoCancel(bool hit)
        {
            disableAll();
            buttons["FirstBaseButton"].Content = hit ? "Single" : "First";
            buttons["SecondBaseButton"].Content = hit ? "Double" : "Second";
            buttons["ThirdBaseButton"].Content = hit ? "Triple" : "Third";
            buttons["NoneBaseButton"].Content = hit ? "Dinger" : "Home";
            activeButtons.Add(buttons["FirstBaseButton"]);
            activeButtons.Add(buttons["SecondBaseButton"]);
            activeButtons.Add(buttons["ThirdBaseButton"]);
            activeButtons.Add(buttons["NoneBaseButton"]);
            enableActive();
        }

        public void fielderButtonsEnable()
        {
            disableAll();
            activeButtons.Add(buttons["Button0"]);
            activeButtons.Add(buttons["Button1"]);
            activeButtons.Add(buttons["Button2"]);
            activeButtons.Add(buttons["Button3"]);
            activeButtons.Add(buttons["Button4"]);
            activeButtons.Add(buttons["Button5"]);
            activeButtons.Add(buttons["Button6"]);
            activeButtons.Add(buttons["Button7"]);
            activeButtons.Add(buttons["ButtonP"]);
            activeButtons.Add(buttons["BasesBackButton"]);
            enableActive();
        }

        public void outTypeButtonsEnable()
        {
            disableAll();
            activeButtons.Add(buttons["StrikeoutButton"]);
            activeButtons.Add(buttons["FlyoutButton"]);
            activeButtons.Add(buttons["GroundoutButton"]);
            activeButtons.Add(buttons["FCButton"]);
            activeButtons.Add(buttons["BasesBackButton"]);
            enableActive();
        }
    }

    enum QueryStates
    {
        Baserunner,
        BaserunnerOnOut,
        FC,
        Batter,
        Fielder,
        OutType,
        Error,
        MainMenu
    }
}
