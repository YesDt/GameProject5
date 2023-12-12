using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject5.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameProject5.Screens
{
    public class JudgementScreen : GameScreen
    {
        private ContentManager _content;
        private Texture2D _backgroundTextureOne;
        private Texture2D _backgroundTextureTwo;
        private Texture2D _backgroundTextureThree;
        private Texture2D _backgroundTextureFour;


        public JudgementScreen()
        {

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate()
        {
            base.Activate();
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _backgroundTextureOne = _content.Load<Texture2D>("cheapbozoending");
            _backgroundTextureTwo = _content.Load<Texture2D>("mehending");
            _backgroundTextureThree = _content.Load<Texture2D>("prettygoodending");
            _backgroundTextureFour = _content.Load<Texture2D>("incredibleending");

            //string text = File.ReadAllText("Scores.txt");
            //foreach(var s in ScreenManager.ScoreList)
            //{
            //    File.WriteAllText((s + "\n"), "Scores.txt");
            //}

        }

        public override void Unload()
        {
            base.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                ScreenManager.score = 0;
                ScreenManager.TotalCoinsCollected = 0;
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            //if (ScreenManager.TotalCoinsCollected <= 38) spriteBatch.Draw(_backgroundTextureOne, fullscreen,
            //    new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            //else if (ScreenManager.TotalCoinsCollected <= 44) spriteBatch.Draw(_backgroundTextureTwo, fullscreen,
            //    new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            //else if (ScreenManager.TotalCoinsCollected < 52) spriteBatch.Draw(_backgroundTextureThree, fullscreen,
            //    new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            //else spriteBatch.Draw(_backgroundTextureFour, fullscreen,
            //    new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            if (ScreenManager.TotalCoinsCollected >= 52) spriteBatch.Draw(_backgroundTextureFour, fullscreen,
               new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            else if (ScreenManager.TotalCoinsCollected >= 45) spriteBatch.Draw(_backgroundTextureThree, fullscreen,
                new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            else if (ScreenManager.TotalCoinsCollected >= 40) spriteBatch.Draw(_backgroundTextureTwo, fullscreen,
                new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            else spriteBatch.Draw(_backgroundTextureOne, fullscreen,
                new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));


            spriteBatch.End();
        }
    }
}
