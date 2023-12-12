using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.TimeZoneInfo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using GameProject5.Screens;
using GameProject5.StateManagement;

namespace GameProject5.Screens
{
    public class PlayGameOptions : MenuScreen
    {




        public PlayGameOptions() : base("PlayGameOptions")
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            var continueEntry = new MenuEntry("Continue");
            var newGameEntry = new MenuEntry("Start New Game");

            continueEntry.Selected += ContinueMenuEntrySelected;
            newGameEntry.Selected += NewGameMenuEntrySelected;


            var back = new MenuEntry("Back");

            back.Selected += OnCancel;

            MenuEntries.Add(continueEntry);
            MenuEntries.Add(newGameEntry);
            MenuEntries.Add(back);

        }


        private void ContinueMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {

            string text = File.ReadAllText("progress.txt");
            if (text.Contains("Level: Level 2")) LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new LevelTwoScreen());
            else if (text.Contains("Level: Level 3")) LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new LevelThreeScreen());
            else if (text.Contains("Level: Level 4")) LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new LevelFourScreen());
            else if (text.Contains("Level: Final Level")) LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new FinalLevelScreen());
            else
            {
                LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new LevelOneScreen());
            }
        }

        private void NewGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.score = 0;
            ScreenManager.TotalCoinsCollected = 0;
            File.WriteAllText("progress.txt", "");
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new LevelOneScreen(), new NewGameCSScreen());

        }

    }
}