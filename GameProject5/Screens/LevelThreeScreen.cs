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

namespace GameProject5.Screens
{
    public class LevelThreeScreen : GameScreen, IParticleEmitter
    {
        #region PrivateFields
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;

        private ContentManager _content;

        private SpriteFont _gameFont;

        private mcSprite _mc = new mcSprite(new Vector2(200, (330 + 744)));
        private CoinSprite[] _coins;
        private Collectible _coinstack = new Collectible(new Vector2(28, 830), new BoundingRectangle(28, 830, 32, 32));
        private Collectible _specialCollectable = new Collectible(new Vector2(120, 810), new BoundingRectangle(120, 810, 48, 32));
        private Collectible _key = new Collectible(new Vector2(1032, 50), new BoundingRectangle(1032, 50, 64, 32));
        private Platform[] _platforms;
        private Goal _goal = new Goal(new Vector2(20, 50), new BoundingRectangle(new Vector2(20, 50), 30f, 24), 20, 2);
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
        private door _doorTwo;
        

        #endregion

        #region PublicFields


        public Texture2D Circle;

        public Texture2D DoorOne;
        public Texture2D DoorTwo;
        public Texture2D Secret;
        public Texture2D Key;

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        FireworkParticleSystem _fireworks;


        #endregion

        public LevelThreeScreen()
        {
            string text = File.ReadAllText("progress.txt");
            if (text.Contains("Level: Level 2\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected))
            {
                text = text.Replace("Level: Level 2\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected, "Level: Level 3\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected);
                File.WriteAllText("progress.txt", text);


            }
            else
            {
                using (StreamWriter sw = new StreamWriter("progress.txt"))
                {

                    sw.WriteLine("Level: Level 3\n Score: " + ScreenManager.score + "\n Coins: " + ScreenManager.TotalCoinsCollected);
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

            _tilemap = _content.Load<TileMap>("TheThirdLevel");

            Circle = _content.Load<Texture2D>("circle");


            System.Threading.Thread.Sleep(700);

            ScreenManager.Game.ResetElapsedTime();
            _mc.LoadContent(_content);
            _mc.Wall = 1070;

            DoorOne = _content.Load<Texture2D>("Sprite_door");
            DoorTwo = _content.Load<Texture2D>("Sprite_door2");
            Secret = _content.Load<Texture2D>("Sprite_secret");
            Key = _content.Load<Texture2D>("Sprite_key");

            _platforms = new Platform[]

            {
                new Platform(new Vector2(0, 453+ 744), new BoundingRectangle(new Vector2(0, 453+ 744), 380f, 1000)),

                new Platform(new Vector2(480, 1197), new BoundingRectangle(new Vector2(480, 1197), 100f, 1000)),

                new Platform(new Vector2(680, 1197), new BoundingRectangle(new Vector2(680, 1197), 400f, 1000)),

                new Platform(new Vector2(970, 1140), new BoundingRectangle(new Vector2(970, 1140), 124f, 90)),

                new Platform(new Vector2(1044, 1060), new BoundingRectangle(new Vector2(1044, 1060), 54f, 90)),

                new Platform(new Vector2(912, 920), new BoundingRectangle(new Vector2(912, 920), 38f, 32)),

                new Platform(new Vector2(632, 870), new BoundingRectangle(new Vector2(632, 870), 280f, 48)),

                new Platform(new Vector2(0, 840), new BoundingRectangle(new Vector2(0, 840), 380f, 48)),

                new Platform(new Vector2(0, 680), new BoundingRectangle(new Vector2(0, 680), 200f, 4)),

                new Platform(new Vector2(300, 840), new BoundingRectangle(new Vector2(300, 840), 80f, 148)),

                new Platform(new Vector2(30, 570), new BoundingRectangle(new Vector2(30, 570), 80f, 72)),

                new Platform(new Vector2(260, 440), new BoundingRectangle(new Vector2(260, 440), 78f, 56)),

                new Platform(new Vector2(360, 380), new BoundingRectangle(new Vector2(360, 380), 78f, 56)),

                new Platform(new Vector2(430, 320), new BoundingRectangle(new Vector2(430, 320), 130f, 56)),

                new Platform(new Vector2(560, 270), new BoundingRectangle(new Vector2(560, 270), 52f, 32)),

                new Platform(new Vector2(600, 210), new BoundingRectangle(new Vector2(600, 210), 32f, 48)),

                new Platform(new Vector2(640, 170), new BoundingRectangle(new Vector2(640, 170), 480f, 48)),

                new Platform(new Vector2(0, 170), new BoundingRectangle(new Vector2(0, 170), 320f, 48)),
                //new Platform(new Vector2(400, 423+ 744), new BoundingRectangle(new Vector2(400, 423+ 744), 60f, 300)),
                //new Platform(new Vector2(460, 400+ 744), new BoundingRectangle(new Vector2(460, 400+ 744), 60f, 300)),
                //new Platform(new Vector2(520, 377+ 744), new BoundingRectangle(new Vector2(520, 377+ 744), 60f, 300)),
                //new Platform(new Vector2(580, 334+ 744), new BoundingRectangle(new Vector2(580, 334+ 744), 60f, 300)),
                //new Platform(new Vector2(200, 100+ 744), new BoundingRectangle(new Vector2(0, 165+ 744), 500f, 30)),
                //new Platform(new Vector2(1020, 100+ 744), new BoundingRectangle(new Vector2(1020, 453+ 744), 300f, 300))

            };

            _enemies = new enemy[]
            {
                new enemy(new Vector2(740, 1064), 680, 740, 128),
                new enemy(new Vector2(700, 748), 632, 900, 128),
                new enemy(new Vector2(680, 30), 660, 852, 128),
            };
            foreach (var e in _enemies) e.LoadContent(_content);
            _goal.LoadContent(_content);
            _doorOne = new door(new Vector2(333, 1042), new BoundingRectangle(331, 1040, 32f, 180), DoorOne, 0.75f);
            _doorTwo = new door(new Vector2(188, 688), new BoundingRectangle(186, 684, 32f, 80), DoorTwo, 0.75f);
            _coinCounter = _content.Load<SpriteFont>("CoinsLeft");
            _scoreDisplay = _content.Load<SpriteFont>("scoreFont");
            _coins = new CoinSprite[]
            {
                new CoinSprite(new Vector2(500, 1147)),

                new CoinSprite(new Vector2(700, 1147)),
                new CoinSprite(new Vector2(720, 1147)),
                new CoinSprite(new Vector2(940, 1107)),
                new CoinSprite(new Vector2(760, 780)),
                new CoinSprite(new Vector2(1030, 1087)),
                new CoinSprite(new Vector2(930, 1057)),

            };
            _coinsLeft = _coins.Length;
            foreach (var coin in _coins) coin.LoadContent(_content);

            _specialGet = _content.Load<SpriteFont>("SpecialGet");

            _coinstack.cTexture = _content.Load<Texture2D>("gold");

            _coinPickup = _content.Load<SoundEffect>("Pickup_Coin15");
            _backgroundMusic = _content.Load<Song>("Level3 music GP6");
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
                        if (proj.Bounds.CollidesWith(plat.Bounds))
                        {
                            proj.projState = state.connected;
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

                if (_coinstack.RecBounds.CollidesWith(_mc.Bounds) || _coinstack.RecBounds.CollidesWith(_mc.FeetBounds))
                {
                    _coinstack.cTexture.Dispose();
                    _coinstack.destroy();
                    _stackPickup.Play();
                    _mc.coinsCollected += 5;
                    _mc.Health += 50;
                    if (_mc.Health >= 100) _mc.Health = 100;
                    _tempScore += 50;

                }

                _doorOne.Update(gameTime);

                _doorTwo.Update(gameTime);


                //if (_mc.Bounds.CollidesWith(_goal.Bounds) || _mc.FeetBounds.CollidesWith(_goal.Bounds))
                //{
                //    MediaPlayer.Stop();
                //    File.WriteAllText("progress.txt", "");
                //    ScreenManager.score += _tempScore;

                //    LoadingScreen.Load(ScreenManager, false, null, new LevelFourScreen());
                //}

                if (_specialCollectable.RecBounds.CollidesWith(_mc.Bounds) || _specialCollectable.RecBounds.CollidesWith(_mc.FeetBounds))
                {
                    _secretObtained = true;
                    _specialPickup.Play();
                    _specialCollectable.destroy();
                    _tempScore += 500;

                }
                if (_key.RecBounds.CollidesWith(_mc.Bounds) || _key.RecBounds.CollidesWith(_mc.FeetBounds))
                {
                    _keyObtained = true;
                    _specialPickup.Play();
                    _key.destroy();
                    Key.Dispose();
                    

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
                if (_mc.Position.Y >= 600 + 736 || _mc.Health <= 0)
                {
                    LoadingScreen.Load(ScreenManager, false, player, new LevelThreeScreen());
                }
                if (_mc.Bounds.CollidesWith(_doorOne.Bounds))
                {
                    if(_mc.Position.X < _doorOne.Bounds.X)
                    {
                        _mc.Position.X -= 20;
                    }
                    else
                    {
                        _mc.Position.X += 20;
                    }
                }
                if (_mc.Bounds.CollidesWith(_doorTwo.Bounds))
                {
                    if(_keyObtained)
                    {
                        _doorTwo.Opened = true;
                        _doorTwo.open();
                        _keyObtained = false;
                    }
                    else
                    {
                        if (_mc.Position.X < _doorTwo.Bounds.X)
                        {
                            _mc.Position.X -= 20;
                        }
                        else
                        {
                            _mc.Position.X += 20;
                        }
                    }
                }
                foreach (var proj in _mc.ProjList)
                {
                    if (proj.Bounds.CollidesWith(_doorOne.Bounds))
                    {
                        proj.projState = state.connected;
                        _doorOne.Opened = true;
                    }
                    if (proj.Bounds.CollidesWith(_doorTwo.Bounds))
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
                if (_mc.Bounds.CollidesWith(_goal.Bounds) || _mc.FeetBounds.CollidesWith(_goal.Bounds))
                {
                    MediaPlayer.Stop();
                    //File.WriteAllText("progress.txt", "");
                    ScreenManager.score += _tempScore;
                    ScreenManager.TotalCoinsCollected += _mc.coinsCollected;
                    LoadingScreen.Load(ScreenManager, false, player, new LevelFourScreen());
                }

                foreach(var e in _enemies)
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
            float playerX = MathHelper.Clamp(_mc.Position.X, 300, 600);
            float offset = 300 - playerX;
            float playerY = MathHelper.Clamp(_mc.Position.Y, 100, 838);
            float offsetY = 100 - playerY;


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

            if (!_secretObtained) spriteBatch.Draw(Secret, _specialCollectable.Position, Color.White);
            if (!_keyObtained) spriteBatch.Draw(Key, _key.Position, new Rectangle(32, 32, 128, 64), Color.White, 0f, new Vector2(0, 0), 0.5f, SpriteEffects.None, 0);

            foreach (var e in _enemies)
            {
                e.Draw(gameTime, spriteBatch);
            }

            _doorOne.Draw(gameTime, _spriteBatch);
            _doorTwo.Draw(gameTime, _spriteBatch);

            spriteBatch.Draw(_coinstack.cTexture, _coinstack.Position, new Rectangle(0, 64, 32, 32), Color.White, 0f, new Vector2(16, 16), 2.0f, SpriteEffects.None, 0);
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

            //spriteBatch.Draw(Circle, new Vector2(_doorOne.Bounds.Left, _doorOne.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_doorOne.Bounds.Right, _doorOne.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_doorOne.Bounds.Left, _doorOne.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_doorOne.Bounds.Right, _doorOne.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);



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
            ////spriteBatch.Draw(Circle, new Vector2(_specialCollectable.RecBounds.Right, _specialCollectable.RecBounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_key.RecBounds.Left, _key.RecBounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_key.RecBounds.Right, _key.RecBounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_key.RecBounds.Left, _key.RecBounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //spriteBatch.Draw(Circle, new Vector2(_key.RecBounds.Right, _key.RecBounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

            ////spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Left, _goal.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            ////spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Right, _goal.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            ////spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Left, _goal.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            ////spriteBatch.Draw(Circle, new Vector2(_goal.Bounds.Right, _goal.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

            //foreach (var e in _enemies)
            //{
            //    spriteBatch.Draw(Circle, new Vector2(e.Bounds.Left, e.Bounds.Top), null, Color.White, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(e.Bounds.Right, e.Bounds.Top), null, Color.Red, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(e.Bounds.Left, e.Bounds.Bottom), null, Color.Blue, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //    spriteBatch.Draw(Circle, new Vector2(e.Bounds.Right, e.Bounds.Bottom), null, Color.Green, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
            //}
            #endregion

            spriteBatch.End();


            spriteBatch.Begin();

            spriteBatch.DrawString(_coinCounter, $"Coins Collected: {_mc.coinsCollected}", new Vector2(2, 2), Color.Gold);
            spriteBatch.DrawString(_scoreDisplay, $"Score: {_tempScore + ScreenManager.score}", new Vector2(2, 50), Color.Orange);
            spriteBatch.Draw(_mc.HealthTexture, _mc.HealthBar, Color.White);
            spriteBatch.Draw(_mc.HealthBarTexture, new Rectangle(47, 420, 103, 50), Color.White);

            if (_secretObtained) spriteBatch.DrawString(_specialGet, "Special item obtained: \n Photo of Jackson's house", new Vector2(600, 420), Color.Green);
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
