using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.TimeZoneInfo;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject5.Screens;

namespace GameProject5.Screens
{
    public class Controls : MenuScreen
    {


        private readonly MenuEntry _controlsEntry;
        private readonly MenuEntry _jumpEntry;

        public Controls() : base("Controls")
        {
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _controlsEntry = new MenuEntry(string.Empty);
            _jumpEntry = new MenuEntry(string.Empty);

            setControlsEntryText();
            var back = new MenuEntry("Back");

            back.Selected += OnCancel;

            MenuEntries.Add(_controlsEntry);
            MenuEntries.Add(_jumpEntry);
            MenuEntries.Add(back);

        }


        private void setControlsEntryText()
        {
            _controlsEntry.Text = "A & D or the left & right arrow keys to move.";
            _jumpEntry.Text = "Press space to jump.";
        }

    }
}