using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.TimeZoneInfo;
using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.IO;
using GameProject5.Collisions;
using GameProject5.Particles;
using GameProject5.Screens;
using GameProject5.StateManagement;


namespace GameProject5.Screens
{
    public class LevelTwoScreen : GameScreen, IParticleEmitter
    {
        #region PrivateFields
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        private ContentManager _content;

        private SpriteFont _gameFont;

        private mcSprite _mc = new mcSprite(new Vector2(200, 300));
        private CoinSprite[] _coins;
        private Platform[] _platforms;
        //private Goal _goal = new Goal(new Vector2(600, 423), new BoundingRectangle(new Vector2(600, 423), 300f, 300));


        //private Texture2D _level2;
        private TileMap _tilemap;

        public Texture2D circle;

        private SpriteFont _coinCounter;
        private int _coinsLeft;

        private Song _backgroundMusic;
        private SoundEffect _coinPickup;

        private bool _noCoinsLeft { get; set; } = false;


        private readonly Random _random = new Random();

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        #endregion



        #region PublicFields
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        FireworkParticleSystem _fireworks;
        #endregion

        public LevelTwoScreen()
        {
            string text = File.ReadAllText("progress.txt");
            if (text.Contains("Level: Level 1"))
            {
                text = text.Replace("Level: Level 1", "Level: Level 2");
                File.WriteAllText("progress.txt", text);


            }
            else
            {
                using (StreamWriter sw = new StreamWriter("progress.txt"))
                {

                    sw.WriteLine("Level: Level 2");
                }
            }

            //ScreenManager.gameState = GameState.LevelTwo;
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);
        }


        #region PublicOverrides
        // Load graphics content for the game
        public override void Activate()
        {
            _graphics = ScreenManager.Game.GraphicsDevice;
            _spriteBatch = ScreenManager.SpriteBatch;



            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _gameFont = _content.Load<SpriteFont>("gamefont");
            //_level2 = _content.Load<Texture2D>("level2");
            _tilemap = _content.Load<TileMap>("TheSecondLevel");

            circle = _content.Load<Texture2D>("circle");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
            _mc.LoadContent(_content);
            _mc.Wall = 1150;

            _platforms = new Platform[]

            {
                new Platform(new Vector2(200, 423), new BoundingRectangle(new Vector2(200 - 200, 423), 300f, 300)),

                new Platform(new Vector2(400, 423), new BoundingRectangle(new Vector2(400, 423), 60f, 300)),
                new Platform(new Vector2(460, 390), new BoundingRectangle(new Vector2(460, 390), 60f, 300)),
                new Platform(new Vector2(520, 357), new BoundingRectangle(new Vector2(520, 357), 60f, 300)),
                new Platform(new Vector2(580, 324), new BoundingRectangle(new Vector2(580, 324), 60f, 300)),
                new Platform(new Vector2(200, 100), new BoundingRectangle(new Vector2(0, 165), 500f, 30))

            };


            //_goal.LoadContent(_content);

            _coinCounter = _content.Load<SpriteFont>("CoinsLeft");
            _coins = new CoinSprite[]
            {
                new CoinSprite(new Vector2(300, 300)),

                new CoinSprite(new Vector2(5, 300)),
                new CoinSprite(new Vector2(10, 350)),

            };
            _coinsLeft = _coins.Length;
            foreach (var coin in _coins) coin.LoadContent(_content);



            _coinPickup = _content.Load<SoundEffect>("Pickup_Coin15");
            _backgroundMusic = _content.Load<Song>("GP4Level2");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(_backgroundMusic);

            _fireworks = new FireworkParticleSystem(ScreenManager.Game, 5);
            _fireworks.Initialize();
            ScreenManager.Game.Components.Add(_fireworks);

        }


        public override void Deactivate()
        {
            ScreenManager.Game.Components.Remove(_fireworks);
            base.Deactivate();
        }

        public override void Unload()
        {
            _content.Unload();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                foreach (Platform plat in _platforms)
                {
                    if (plat.Bounds.CollidesWith(_mc.FeetBounds))
                    {
                        _mc.CollisionHandling(plat.Bounds);


                    }
                    else
                    {
                        _mc.OffGround = true;
                    }


                }

                foreach (var coin in _coins)
                {
                    if (!coin.Collected && coin.Bounds.CollidesWith(_mc.Bounds))
                    {
                        Velocity = coin.CoinPosition - Position;
                        Position = coin.CoinPosition;


                        coin.Collected = true;
                        _coinPickup.Play();
                        _coinsLeft--;
                        _mc.coinsCollected++;

                    }

                }
                if (_coinsLeft == 0)
                {
                    _noCoinsLeft = true;
                }



                //if (_mc.Bounds.CollidesWith(_goal.Bounds))
                //{
                //    MediaPlayer.Stop();
                //    File.WriteAllText("progress.txt", "");
                //     LoadingScreen.Load(ScreenManager, false, null, new MaintainenceScreen());
                //}
            }
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            var keyboardState = input.CurrentKeyboardStates[playerIndex];
            var gamePadState = input.CurrentGamePadStates[playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            PlayerIndex player;
            if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
            {
                MediaPlayer.Pause();
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                MediaPlayer.Resume();

                _mc.Update(gameTime);
                if (_mc.Position.Y >= 600)
                {
                    LoadingScreen.Load(ScreenManager, false, player, new LevelTwoScreen());
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            float playerX = MathHelper.Clamp(_mc.Position.X, 300, 700);
            float offset = 300 - playerX;



            Matrix transform;
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            transform = Matrix.CreateTranslation(offset, 0, 0);
            _fireworks.Transform = transform;
            spriteBatch.Begin(transformMatrix: transform);


            _tilemap.Draw(gameTime, _spriteBatch);
            foreach (var coin in _coins)
            {
                coin.Draw(gameTime, spriteBatch);

                if (!coin.Collected && coin.Bounds.CollidesWith(_mc.Bounds))
                {

                    _fireworks.placeFirework(coin.CoinPosition);

                }
            }
            //_goal.Draw(spriteBatch, gameTime);


            _mc.Draw(gameTime, spriteBatch);

            //Debugging purposes
            foreach (Platform plat in _platforms)
            {
                spriteBatch.Draw(circle, new Vector2(plat.Bounds.Left, plat.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(circle, new Vector2(plat.Bounds.Right, plat.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(circle, new Vector2(plat.Bounds.Left, plat.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(circle, new Vector2(plat.Bounds.Right, plat.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(circle, new Vector2(_mc.Bounds.Left, _mc.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(circle, new Vector2(_mc.Bounds.Right, _mc.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(circle, new Vector2(_mc.Bounds.Left, _mc.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(circle, new Vector2(_mc.Bounds.Right, _mc.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(circle, new Vector2(_mc.FeetBounds.Left, _mc.FeetBounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(circle, new Vector2(_mc.FeetBounds.Right, _mc.FeetBounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(circle, new Vector2(_mc.FeetBounds.Left, _mc.FeetBounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                spriteBatch.Draw(circle, new Vector2(_mc.FeetBounds.Right, _mc.FeetBounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            }

            spriteBatch.End();


            spriteBatch.Begin();

            spriteBatch.DrawString(_coinCounter, $"Coins Left: {_coinsLeft}", new Vector2(2, 2), Color.Gold);


            spriteBatch.End();

            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
        #endregion
    }
}