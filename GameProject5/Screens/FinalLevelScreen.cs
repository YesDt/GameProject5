using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject5.Collisions;
using GameProject5.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Input;
using GameProject5.Particles;
using System.IO;
using static System.TimeZoneInfo;
using System.Reflection.Metadata;
using System.Threading;

namespace GameProject5.Screens
{
    public class FinalLevelScreen: GameScreen, IParticleEmitter
    {
        #region PrivateFields
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        private ContentManager _content;

        private SpriteFont _gameFont;

        private mcSprite _mc = new mcSprite(new Vector2(200, 200));
        private Boss _boss = new Boss(new Vector2(700, 291));
        private CoinSprite[] _coins;

        private Platform[] _platforms;


        private TileMap _tilemap;



        private SpriteFont _coinCounter;
        private SpriteFont _scoreDisplay;
        private int _coinsLeft;
        private int _tempScore = 0;



        private Song _backgroundMusic;
        private SoundEffect _coinPickup;


        private bool _noCoinsLeft { get; set; } = false;


        private readonly Random _random = new Random();

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;


        #endregion



        #region PublicFields


        public Texture2D Circle;

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        FireworkParticleSystem _fireworks;


        #endregion

        public FinalLevelScreen()
        {

            string text = File.ReadAllText("progress.txt");
            if (text.Contains("Level: Level 4\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected))
            {
                text = text.Replace("Level: Level 4\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected, "Level: Final Level\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected);
                File.WriteAllText("progress.txt", text);


            }
            else
            {
                using (StreamWriter sw = new StreamWriter("progress.txt"))
                {

                    sw.WriteLine("Level: Final Level\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected);
                }
            }





            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);
        }


        #region PublicOverrides

        public override void Activate()
        {
            _graphics = ScreenManager.Game.GraphicsDevice;
            _spriteBatch = ScreenManager.SpriteBatch;

            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            _gameFont = _content.Load<SpriteFont>("gamefont");

            _tilemap = _content.Load<TileMap>("theFinalLevel");


            Circle = _content.Load<Texture2D>("circle");


            Thread.Sleep(700);

            ScreenManager.Game.ResetElapsedTime();
            _mc.LoadContent(_content);
            _mc.Wall = 750;
            _boss.LoadContent(_content);
            _boss.Boundary = 750;
            _platforms = new Platform[]
                {
        new Platform(new Vector2(0, 443), new BoundingRectangle(new Vector2(0, 443), 1200f, 2)),
                };
          ;

            _coinCounter = _content.Load<SpriteFont>("CoinsLeft");
            _scoreDisplay = _content.Load<SpriteFont>("scoreFont");
            _coins = new CoinSprite[]
            {
                new CoinSprite(new Vector2(300, 300)),
                new CoinSprite(new Vector2(700, 300)),
                new CoinSprite(new Vector2(5, 300)),
                new CoinSprite(new Vector2(80, 250)),
                new CoinSprite(new Vector2(543, 300)),
                new CoinSprite(new Vector2(723, 300)),

            };
            _coinsLeft = _coins.Length;
            foreach (var coin in _coins) coin.LoadContent(_content);



            _coinPickup = _content.Load<SoundEffect>("Pickup_Coin15");
            _backgroundMusic = _content.Load<Song>("Boss GP");
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

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
            else
                _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                

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
                        _mc.Health += 10;
                        if (_mc.Health >= 100) _mc.Health = 100;
                        _tempScore += 10;


                    }

                }
                if (_coinsLeft == 0)
                {
                    _noCoinsLeft = true;
                }

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
                if (_mc.Health <= 0)
                {
                    LoadingScreen.Load(ScreenManager, false, player, new FinalLevelScreen());
                }
                
                foreach (var proj in _mc.ProjList)
                {
                    if (proj.Bounds.X >= _mc.Wall || proj.Bounds.X <= 0)
                    {
                        proj.projState = state.connected;

                    }
                    if (proj.Bounds.CollidesWith(_boss.Bounds) && !_boss.Attacking)
                    {
                        proj.projState = state.connected;
                        proj.Destroy(proj);
                        _boss.Health -= 5;
                    }
                }
                foreach (var plat in _platforms)
                {
                    if (plat.Bounds.CollidesWith(_mc.FeetBounds) || plat.Bounds.CollidesWith(_mc.Bounds))
                    {
                        _mc.CollisionHandling(plat.Bounds);
                    }
                }
                _boss.Update(gameTime, _mc);
                if (_mc.Bounds.CollidesWith(_boss.Bounds) && _boss.Action == BossAction.ShoulderCharge && !_mc.Recovering)
                {
                    _mc.Position.Y -= 50;
                    _mc.Health -= 40;
                    _mc.Hurt = true;
                }
                
                _mc.Update(gameTime);
                foreach (var proj in _boss.ProjList)
                {
                    if (proj.Bounds.X >= _boss.Boundary || proj.Bounds.X <= 0)
                    {
                        proj.Connected = true;

                    }
                    if (proj.Bounds.CollidesWith(_mc.Bounds))
                    {
                        proj.Connected = true;
                        _mc.Health -= 20;
                    }
                }
                if (_boss.Health <= 0)
                {
                    _tempScore += 5000;
                    ScreenManager.score += _tempScore;
                    ScreenManager.TotalCoinsCollected += _mc.coinsCollected;
                    MediaPlayer.Stop();
                    File.WriteAllText("progress.txt", "");
                    LoadingScreen.Load(ScreenManager, false, null, new WinScreen());
                }


            }


        }

        public override void Draw(GameTime gameTime)
        {
            
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

  
            spriteBatch.Begin();


            _tilemap.Draw(gameTime, _spriteBatch);
            foreach (var coin in _coins)
            {
                coin.Draw(gameTime, spriteBatch);

                if (!coin.Collected && coin.Bounds.CollidesWith(_mc.Bounds))
                {

                    _fireworks.placeFirework(coin.CoinPosition);

                }
            }


            _mc.Draw(gameTime, spriteBatch);
            _boss.Draw(gameTime, spriteBatch);


            spriteBatch.DrawString(_coinCounter, $"Coins Collected: {_mc.coinsCollected}", new Vector2(2, 2), Color.Gold);
            spriteBatch.DrawString(_scoreDisplay, $"Score: {_tempScore + ScreenManager.score}", new Vector2(2, 50), Color.Orange);
            spriteBatch.Draw(_mc.HealthTexture, _mc.HealthBar, Color.White);
            spriteBatch.Draw(_mc.HealthBarTexture, new Rectangle(45, 420, 105, 50), Color.White);
            spriteBatch.Draw(_boss.HealthTexture, _boss.HealthBar, Color.White);
            spriteBatch.Draw(_boss.HealthBarTexture, new Rectangle(592, 420, 106, 50), Color.White);

            //foreach (var f in _boss.ProjList)
            //{
            //    spriteBatch.Draw(Circle, new Vector2(f.Bounds.Left, f.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(f.Bounds.Right, f.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(f.Bounds.Left, f.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(f.Bounds.Right, f.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //}


            //spriteBatch.Draw(circle, new Vector2(_mc.Bounds.Left, _mc.Bounds.Bottom), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_boss.Bounds.Left, _boss.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_boss.Bounds.Right, _boss.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_boss.Bounds.Left, _boss.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_boss.Bounds.Right, _boss.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(circle, _platforms.Position, null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0 || _pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
        #endregion
    }
}
