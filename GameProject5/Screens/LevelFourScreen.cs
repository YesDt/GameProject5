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
using System.Net.Sockets;


namespace GameProject5.Screens
{
    public class LevelFourScreen : GameScreen, IParticleEmitter
    {
        #region PrivateFields
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        private ContentManager _content;

        private SpriteFont _gameFont;

        private mcSprite _mc = new mcSprite(new Vector2(604, 520));
        private CoinSprite[] _coins;
        private Collectible[] _coinstacks;
        private Collectible _specialCollectable = new Collectible(new Vector2(1250, 200), new BoundingRectangle(1250, 250, 48, 32));
        private Collectible[] _key;
        private Platform[] _platforms;
        private Goal _goal = new Goal(new Vector2(1298, 80), new BoundingRectangle(new Vector2(1298, 80), 30f, 24), 20, 20);
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

        private bool _keyObtained = false;



        private door _doorOne;

        #endregion

        #region PublicFields


        public Texture2D Circle;

        public Texture2D Secret;

        public Texture2D DoorOne;

        public Texture2D Key;

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        FireworkParticleSystem _fireworks;


        #endregion

        public LevelFourScreen()
        {
            string text = File.ReadAllText("progress.txt");
            if (text.Contains("Level: Level 3\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected))
            {
                text = text.Replace("Level: Level 3\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected, "Level: Level 4\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected);
                File.WriteAllText("progress.txt", text);


            }
            else
            {
                using (StreamWriter sw = new StreamWriter("progress.txt"))
                {

                    sw.WriteLine("Level: Level 4\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected);
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

            _tilemap = _content.Load<TileMap>("TheFourthLevel");

            Circle = _content.Load<Texture2D>("circle");

            Secret = _content.Load<Texture2D>("Sprite_secret");


            System.Threading.Thread.Sleep(700);

            ScreenManager.Game.ResetElapsedTime();
            _mc.LoadContent(_content);
            _mc.Wall = 1300;

            DoorOne = _content.Load<Texture2D>("Sprite_door2");

            Key = _content.Load<Texture2D>("Sprite_key");
            _platforms = new Platform[]

            {
                new Platform(new Vector2(0, 810), new BoundingRectangle(new Vector2(0, 810), 1800f, 1000)),
                new Platform(new Vector2(1223, 700), new BoundingRectangle(new Vector2(1223, 700), 72f, 100)),
                new Platform(new Vector2(1044, 575), new BoundingRectangle(new Vector2(1044, 575), 82f, 32)),
                new Platform(new Vector2(1250, 430), new BoundingRectangle(new Vector2(1250, 430), 72f, 48)),
                new Platform(new Vector2(992, 420), new BoundingRectangle(new Vector2(992, 420), 42f, 96)),


                new Platform(new Vector2(203, 750), new BoundingRectangle(new Vector2(203, 750), 48f, 32)),
                new Platform(new Vector2(143, 700), new BoundingRectangle(new Vector2(143, 700), 38f, 32)),
                new Platform(new Vector2(90, 665), new BoundingRectangle(new Vector2(90, 665), 38f, 32)),
                new Platform(new Vector2(10, 630), new BoundingRectangle(new Vector2(10, 630), 64f, 32)),

                new Platform(new Vector2(198, 456), new BoundingRectangle(new Vector2(198, 456), 200f, 56)),
                new Platform(new Vector2(254, 390), new BoundingRectangle(new Vector2(254, 390), 56f, 56)),
                new Platform(new Vector2(310, 300), new BoundingRectangle(new Vector2(310, 300), 72f, 72)),
                new Platform(new Vector2(382, 200), new BoundingRectangle(new Vector2(382, 200), 80f, 300)),
                new Platform(new Vector2(916, 150), new BoundingRectangle(new Vector2(916, 150), 464f, 64)),
                new Platform(new Vector2(916, 150), new BoundingRectangle(new Vector2(916, 150), 128f, 256)),
                new Platform(new Vector2(981, 264), new BoundingRectangle(new Vector2(916, 150), 64f, 192)),

            };

            _enemies = new enemy[]
          {
                new enemy(new Vector2(740, 664), 680, 740, 96),
                new enemy(new Vector2(590, 664), 580, 620, 128),
                new enemy(new Vector2(1052, 24), 1000, 1160, 128),
          };
            foreach (var e in _enemies) e.LoadContent(_content);
            _goal.LoadContent(_content);
            _doorOne = new door(new Vector2(420, 570), new BoundingRectangle(420, 570, 42f, 400), DoorOne, 1.1f);
            _coinCounter = _content.Load<SpriteFont>("CoinsLeft");
            _scoreDisplay = _content.Load<SpriteFont>("scoreFont");
            _coins = new CoinSprite[]
            {
                new CoinSprite(new Vector2(1230, 640)),
                new CoinSprite(new Vector2(1064, 520)),
                new CoinSprite(new Vector2(1064, 520)),
                new CoinSprite(new Vector2(900, 480)),
                new CoinSprite(new Vector2(400, 560)),


            };
            _coinstacks = new Collectible[]
            {
                new Collectible(new Vector2(30, 620), new BoundingRectangle(new Vector2(30, 620), 32, 32)),
                new Collectible(new Vector2(386, 160), new BoundingRectangle(new Vector2(386, 160), 32, 32)),
            };
            _coinsLeft = _coins.Length;
            foreach (var coin in _coins) coin.LoadContent(_content);

            _key = new Collectible[]
            {
                new Collectible(new Vector2(1228, 570), new BoundingRectangle(new Vector2(1228, 570), 32, 220)),
            };
            foreach (var k in _key) k.cTexture = Key;
            _specialGet = _content.Load<SpriteFont>("SpecialGet");

            foreach(var cs in _coinstacks) cs.cTexture = _content.Load<Texture2D>("gold");

            _coinPickup = _content.Load<SoundEffect>("Pickup_Coin15");
            _backgroundMusic = _content.Load<Song>("Level4 music GP6");
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

                    foreach (var proj in _mc.ProjList)
                    {
                        if (proj.Bounds.CollidesWith(_doorOne.Bounds))
                        {
                            proj.projState = state.connected;

                        }
                        if (proj.Bounds.X >= _mc.Wall || proj.Bounds.X <= 0)
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

                foreach (var cs in _coinstacks)
                {
                    if (cs.RecBounds.CollidesWith(_mc.Bounds) || cs.RecBounds.CollidesWith(_mc.FeetBounds))
                    {
                        cs.cTexture.Dispose();
                        cs.destroy();
                       
                        _stackPickup.Play();
                        _mc.coinsCollected += 5;
                        _mc.Health += 50;
                        if (_mc.Health >= 100) _mc.Health = 100;
                        _tempScore += 50;

                    }
                }
                    

                _doorOne.Update(gameTime);

                foreach (var k in _key.ToList())
                {
                   
                    if (k.RecBounds.CollidesWith(_mc.Bounds) || k.RecBounds.CollidesWith(_mc.FeetBounds))
                    {
                        _keyObtained = true;
                        _specialPickup.Play();
                        k.destroy();
                        k.cTexture.Dispose();


                    }
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
                if (_mc.Health <= 0)
                {
                    LoadingScreen.Load(ScreenManager, false, player, new LevelFourScreen());
                }
                if (_mc.Bounds.CollidesWith(_doorOne.Bounds) && (_keyObtained))
                {
                    _doorOne.Opened = true;
                }
                if (_mc.Bounds.CollidesWith(_doorOne.Bounds))
                {
                    if (_mc.Position.X < _doorOne.Bounds.X)
                    {
                        _mc.Position.X -= 20;
                    }
                    else
                    {
                        _mc.Position.X += 20;
                    }
                }
                foreach (var proj in _mc.ProjList)
                {
                    if (proj.Bounds.CollidesWith(_doorOne.Bounds))
                    {
                        proj.projState = state.connected;

                    }
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
                    LoadingScreen.Load(ScreenManager, false, player, new FinalLevelScreen(), new BossIntro());
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
            float playerX = MathHelper.Clamp(_mc.Position.X, 300, 840);
            float offset = 300 - playerX;
            float playerY = MathHelper.Clamp(_mc.Position.Y, 50, 380);
            float offsetY = 50 - playerY;


            Matrix transform;
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

            var spriteBatch = ScreenManager.SpriteBatch;

            transform = Matrix.CreateTranslation(offset, offsetY, 0);
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

            _doorOne.Draw(gameTime, _spriteBatch);

            foreach (var cs in _coinstacks) 
            {
                spriteBatch.Draw(cs.cTexture, cs.Position, new Rectangle(0, 64, 32, 32), Color.White, 0f, new Vector2(16, 16), 2.0f, SpriteEffects.None, 0);
                if (cs.RecBounds.CollidesWith(_mc.Bounds) || cs.RecBounds.CollidesWith(_mc.FeetBounds))
                {
                    _fireworks.placeFirework(cs.Position);
                    _fireworks.placeFirework(cs.Position);
                }
            }

            foreach (var e in _enemies)
            {
                e.Draw(gameTime, spriteBatch);
            }


            if (_specialCollectable.RecBounds.CollidesWith(_mc.Bounds) || _specialCollectable.RecBounds.CollidesWith(_mc.FeetBounds))
            {
                _fireworks.placeFirework(_specialCollectable.Position);
                _fireworks.placeFirework(_specialCollectable.Position);
                _fireworks.placeFirework(_specialCollectable.Position);
            }

            if (!_secretObtained) spriteBatch.Draw(Secret, _specialCollectable.Position, Color.White);
            foreach (var k in _key.ToList())
            {
                spriteBatch.Draw(Key, k.Position, new Rectangle(32, 32, 128, 64), Color.White, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);
               
            }

            _mc.Draw(gameTime, spriteBatch);


            //Debugging purposes
            #region Debugging

            //foreach (var proj in _mc.ProjList)
            //{
            //    spriteBatch.Draw(Circle, new Vector2(proj.Bounds.Left, proj.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(proj.Bounds.Right, proj.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(proj.Bounds.Left, proj.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(proj.Bounds.Right, proj.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //}


            //foreach (Platform plat in _platforms)
            //{
            //    spriteBatch.Draw(Circle, new Vector2(plat.Bounds.Left, plat.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(plat.Bounds.Right, plat.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(plat.Bounds.Left, plat.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(plat.Bounds.Right, plat.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

            //}

            //spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Left, _goal.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Right, _goal.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Left, _goal.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Right, _goal.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);


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

            if (_secretObtained) spriteBatch.DrawString(_specialGet, "Special item obtained: \n Photo of Jackson's family", new Vector2(600, 420), Color.Green);
            if (_keyObtained) spriteBatch.DrawString(_specialGet, "Key Obtained!", new Vector2(2, 100), Color.Blue);

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
