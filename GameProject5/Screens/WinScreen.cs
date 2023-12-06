using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GameProject5.StateManagement;
using System.IO;

namespace GameProject5.Screens
{
    public class WinScreen : GameScreen
    {
        private ContentManager _content;
        private Texture2D _backgroundTexture;
        private SpriteFont _finalScore;
        private SpriteFont _scoreBoard;
        private SpriteFont _highScore;
        private bool _highScoreReached = false;

        public WinScreen()
        {
            
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        public override void Activate()
        {
            base.Activate();
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            _finalScore = _content.Load<SpriteFont>("FinalScoreBoard");
            _scoreBoard = _content.Load<SpriteFont>("gamefont");
            _highScore = _content.Load<SpriteFont>("gamefont");
            _backgroundTexture = _content.Load<Texture2D>("gameproject6winscreen");
            ScreenManager.ScoreList.Add(ScreenManager.score);
            ScreenManager.ScoreList = ScreenManager.ScoreList.OrderBy(x => x).ToList();
            ScreenManager.ScoreList.Reverse();
            if (ScreenManager.score == ScreenManager.ScoreList.Max())
            {
                _highScoreReached = true;
            }

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
                LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(), new MainMenuScreen());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;
            var viewport = ScreenManager.GraphicsDevice.Viewport;
            var fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);

            spriteBatch.Begin();

            spriteBatch.Draw(_backgroundTexture, fullscreen,
                new Color(TransitionAlpha, TransitionAlpha, TransitionAlpha));
            spriteBatch.DrawString(_finalScore, $"Final Score: " + ScreenManager.score, new Vector2(100, 100), Color.Gold);
            string scores = "";
            foreach (var s in ScreenManager.ScoreList)
            {
                scores = scores + (s.ToString() + "\n");
            }
            spriteBatch.DrawString(_scoreBoard, scores, new Vector2(100, 200), Color.Blue);
            if(_highScoreReached) spriteBatch.DrawString(_highScore, "WOW! HIGH SCORE!!!", new Vector2(10, 340), Color.Red);







            spriteBatch.End();
        }
    }
}
