﻿using System;
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

        private mcSprite _mc = new mcSprite(new Vector2(200, 330));
        private CoinSprite[] _coins;
        private Collectible _coinstack = new Collectible(new Vector2(20, 150), new BoundingRectangle(0, 110, 32, 32));
        private Collectible _specialCollectable = new Collectible(new Vector2(180, 150), new BoundingRectangle(180, 110, 48, 32));
        private Platform[] _platforms;
        private Goal _goal = new Goal(new Vector2(1150, 210), new BoundingRectangle(new Vector2(1150, 210), 30f, 260), 640, 280);
        private enemy[] _enemies;


        //private Texture2D _level2;
        private TileMap _tilemap;

        

        private SpriteFont _coinCounter;
        private SpriteFont _scoreDisplay;
        private int _coinsLeft;
        private int _tempScore = 0;

        private SpriteFont _specialGet;

        private Song _backgroundMusic;
        private SoundEffect _coinPickup;
        private SoundEffect _stackPickup;
        private SoundEffect _specialPickup;
        private SoundEffect _hurt;

        private bool _noCoinsLeft { get; set; } = false;


        private readonly Random _random = new Random();

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        private bool _secretObtained = false;



        #endregion



        #region PublicFields


        public Texture2D Circle;

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        FireworkParticleSystem _fireworks;

        public Texture2D Secret;

        #endregion

        public LevelTwoScreen()
        {
            string text = File.ReadAllText("progress.txt");
            if (text.Contains("Level: Level 1\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected))
            {
                text = text.Replace("Level: Level 1\n Score: " + ScreenManager.score + "\n Coins:" + ScreenManager.TotalCoinsCollected, "Level: Level 2\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected);
                File.WriteAllText("progress.txt", text);


            }
            else
            {
                using (StreamWriter sw = new StreamWriter("progress.txt"))
                {

                    sw.WriteLine("Level: Level 2\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected);
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

            _tilemap = _content.Load<TileMap>("TheSecondLevel");

            Circle = _content.Load<Texture2D>("circle");

            Secret = _content.Load<Texture2D>("Sprite_secret");

            Thread.Sleep(1000);

            ScreenManager.Game.ResetElapsedTime();
            _mc.LoadContent(_content);
            _mc.Wall = 1150;

            _platforms = new Platform[]

            {
                new Platform(new Vector2(200, 453), new BoundingRectangle(new Vector2(0, 453), 380f, 300)),

                new Platform(new Vector2(400, 423), new BoundingRectangle(new Vector2(400, 423), 60f, 300)),
                new Platform(new Vector2(450, 400), new BoundingRectangle(new Vector2(450, 400), 64f, 300)),
                new Platform(new Vector2(510, 377), new BoundingRectangle(new Vector2(510, 377), 69f, 300)),
                new Platform(new Vector2(570, 354), new BoundingRectangle(new Vector2(570, 354), 72f, 300)),
                new Platform(new Vector2(0, 178), new BoundingRectangle(new Vector2(0, 178), 500f, 30)),
                new Platform(new Vector2(1020, 100), new BoundingRectangle(new Vector2(1020, 453), 300f, 300))

            };


            _goal.LoadContent(_content);

            _coinCounter = _content.Load<SpriteFont>("CoinsLeft");
            _scoreDisplay = _content.Load<SpriteFont>("scoreFont");
            _coins = new CoinSprite[]
            {
                new CoinSprite(new Vector2(300, 300)),

                new CoinSprite(new Vector2(5, 300)),
                new CoinSprite(new Vector2(10, 350)),

            };
            _coinsLeft = _coins.Length;
            foreach (var coin in _coins) coin.LoadContent(_content);

            _enemies = new enemy[]
            {
                new enemy(new Vector2(100, 50), 90, 200, 128),
            };
            foreach (var e in _enemies) e.LoadContent(_content);

            _specialGet = _content.Load<SpriteFont>("SpecialGet");

            _coinstack.cTexture = _content.Load<Texture2D>("gold");

            _coinPickup = _content.Load<SoundEffect>("Pickup_Coin15");
            _backgroundMusic = _content.Load<Song>("GP4Level2");
            _stackPickup = _content.Load<SoundEffect>("Pickup_Coin5");
            _specialPickup = _content.Load<SoundEffect>("Pickup_Special");
            _hurt = _content.Load<SoundEffect>("Hit_Hurt");
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
                    if (plat.Bounds.CollidesWith(_mc.FeetBounds) || plat.Bounds.CollidesWith(_mc.Bounds))
                    {
                        _mc.CollisionHandling(plat.Bounds);


                    }
                    else
                    {
                        _mc.OffGround = true;
                    }
                    foreach(var proj in _mc.ProjList)
                    {
                        if (proj.Bounds.CollidesWith(plat.Bounds))
                        {
                            proj.projState = state.connected;
                        }
                        foreach (var e in _enemies)
                        {
                            if (proj.Bounds.CollidesWith(e.Bounds))
                            {
                                PunchProjectile._collide.Play();
                                proj.projState = state.connected;
                                proj.Destroy(proj);
                                e._death.Play();
                                e.Health = 0;
                                _tempScore += 50;
                            }
                        }
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
                        _mc.Health += 10;
                        if (_mc.Health >= 100) _mc.Health = 100;
                        _tempScore += 10;
                      

                    }

                }
                if (_coinsLeft == 0)
                {
                    _noCoinsLeft = true;
                }

                if(_coinstack.RecBounds.CollidesWith(_mc.Bounds) || _coinstack.RecBounds.CollidesWith(_mc.FeetBounds))
                {
                    _coinstack.cTexture.Dispose();
                    _coinstack.destroy();
                    _stackPickup.Play();
                    _mc.coinsCollected += 5;
                    _mc.Health += 50;
                    if (_mc.Health >= 100) _mc.Health = 100;
                    _tempScore += 50;
                 
                }



               


               

                if (_specialCollectable.RecBounds.CollidesWith(_mc.Bounds) || _specialCollectable.RecBounds.CollidesWith(_mc.FeetBounds))
                {
                    _secretObtained = true;
                    _specialPickup.Play();
                    _specialCollectable.destroy();
                    _tempScore += 500;
                   
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
                foreach (var proj in _mc.ProjList)
                {
                    if (proj.Bounds.X >= _mc.Wall || proj.Bounds.X <= 0)
                    {
                        proj.projState = state.connected;
                    }
                }
                if (_mc.Bounds.CollidesWith(_goal.Bounds) || _mc.FeetBounds.CollidesWith(_goal.Bounds))
                {
                    MediaPlayer.Stop();
                    //File.WriteAllText("progress.txt", "");
                    ScreenManager.score += _tempScore;
                    ScreenManager.TotalCoinsCollected += _mc.coinsCollected;
                    LoadingScreen.Load(ScreenManager, false, player, new LevelThreeScreen());
                }
                if (_mc.Position.Y >= 600)
                {
                    LoadingScreen.Load(ScreenManager, false, player, new LevelTwoScreen());
                }
                foreach (var e in _enemies)
                {
                    e.Update(gameTime, _mc);
                    if (_mc.Bounds.CollidesWith(e.Searching) && !e.Dead)
                    {

                        e.AboutToAttack();
                    }
                    foreach (var b in e.BulletList)
                    {
                        if (_mc.Bounds.CollidesWith(b.Bounds) && !_mc.Recovering)
                        {
                            _mc.Health -= 10;
                            _mc.Hurt = true;
                            _hurt.Play();
                            b.Destroy(b);
                        }
                    }


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
            
            spriteBatch.Draw(_coinstack.cTexture, new Vector2(20, 150), new Rectangle(0, 64, 32, 32), Color.White, 0f, new Vector2(16, 16), 2.0f, SpriteEffects.None, 0);
            if (_coinstack.RecBounds.CollidesWith(_mc.Bounds) || _coinstack.RecBounds.CollidesWith(_mc.FeetBounds))
            {
                _fireworks.placeFirework(_coinstack.Position);
                _fireworks.placeFirework(_coinstack.Position);
            }

            if (_specialCollectable.RecBounds.CollidesWith(_mc.Bounds) || _specialCollectable.RecBounds.CollidesWith(_mc.FeetBounds))
            {
                _fireworks.placeFirework(_specialCollectable.Position);
                _fireworks.placeFirework(_specialCollectable.Position);
                _fireworks.placeFirework(_specialCollectable.Position);
            }
            if (!_secretObtained) spriteBatch.Draw(Secret, _specialCollectable.Position, null, Color.White, 0f, new Vector2(16, 32), 1f, SpriteEffects.None, 0);
            _mc.Draw(gameTime, spriteBatch);
            foreach (var e in _enemies)
            {
                e.Draw(gameTime, spriteBatch);
            }

            //Debugging purposes
            #region Debugging
            //foreach (Platform plat in _platforms)
            //{
            //    spriteBatch.Draw(Circle, new Vector2(plat.Bounds.Left, plat.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(plat.Bounds.Right, plat.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(plat.Bounds.Left, plat.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(plat.Bounds.Right, plat.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

            //}
            //spriteBatch.Draw(Circle, new Vector2(_mc.Bounds.Left, _mc.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_mc.Bounds.Right, _mc.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_mc.Bounds.Left, _mc.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_mc.Bounds.Right, _mc.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_mc.FeetBounds.Left, _mc.FeetBounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_mc.FeetBounds.Right, _mc.FeetBounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_mc.FeetBounds.Left, _mc.FeetBounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_mc.FeetBounds.Right, _mc.FeetBounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

            //spriteBatch.Draw(Circle, new Vector2(_coinstack.RecBounds.Left, _coinstack.RecBounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_coinstack.RecBounds.Right, _coinstack.RecBounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_coinstack.RecBounds.Left, _coinstack.RecBounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_coinstack.RecBounds.Right, _coinstack.RecBounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);


            //spriteBatch.Draw(Circle, new Vector2(_specialCollectable.RecBounds.Left, _specialCollectable.RecBounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_specialCollectable.RecBounds.Right, _specialCollectable.RecBounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_specialCollectable.RecBounds.Left, _specialCollectable.RecBounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_specialCollectable.RecBounds.Right, _specialCollectable.RecBounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

            //spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Left, _goal.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Right, _goal.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Left, _goal.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Right, _goal.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            #endregion

            spriteBatch.End();


            spriteBatch.Begin();

            spriteBatch.DrawString(_coinCounter, $"Coins Collected: {_mc.coinsCollected}", new Vector2(2, 2), Color.Gold);
            spriteBatch.DrawString(_scoreDisplay, $"Score: {_tempScore + ScreenManager.score}", new Vector2(2, 50), Color.Orange);
            spriteBatch.Draw(_mc.HealthTexture, _mc.HealthBar, Color.White);
            spriteBatch.Draw(_mc.HealthBarTexture, new Rectangle(47, 420, 103, 50), Color.White);
           
            if (_secretObtained) spriteBatch.DrawString(_specialGet, "Special item obtained: \n gambling debt papers" , new Vector2(600, 420), Color.Green);

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