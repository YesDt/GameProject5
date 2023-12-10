using GameProject5.StateManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;

namespace GameProject5.Screens
{
    public class BossDefeat : GameScreen
    {
        ContentManager _content;
        Video _video;
        VideoPlayer _player;
        bool _isPlaying = false;
        InputAction _skip;

        //private Texture2D _pressEsc;

        public BossDefeat()
        {
            _player = new VideoPlayer();
            _skip = new InputAction(new Buttons[] { Buttons.A }, new Keys[] { Keys.Escape }, true);
        }

        public override void Activate()
        {
            if (_content == null)
            {
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            }
            _video = _content.Load<Video>("bossEnd");
            // _pressEsc = _content.Load<Texture2D>("escToSkip");
            if (!_isPlaying)
            {
                _player.Play(_video);
                _isPlaying = true;
            }

        }

        public override void HandleInput(GameTime gameTime, InputState input)
        {

            PlayerIndex player;
            if (_skip.Occurred(input, null, out player))
            {
                _player.Pause();
                _player.Stop();
                ExitScreen();
            }
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (_player.PlayPosition >= _video.Duration) ExitScreen();
        }

        public override void Deactivate()
        {
            _player.Pause();
            _isPlaying = false;
        }

        public override void Draw(GameTime gameTime)
        {

            if (_isPlaying)
            {
                ScreenManager.SpriteBatch.Begin();
                try
                {
                    var texture = _player.GetTexture();
                    ScreenManager.SpriteBatch.Draw(texture, ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White);
                }
                catch (Exception e) { };

                //ScreenManager.SpriteBatch.Draw(_pressEsc, new Vector2(600, 300), Color.White);
                ScreenManager.SpriteBatch.End();
            }

        }
    }
}
