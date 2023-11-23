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

namespace GameProject5.Screens
{
    public class LevelThreeScreen : GameScreen, IParticleEmitter
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

        private bool _noCoinsLeft { get; set; } = false;


        private readonly Random _random = new Random();

        private float _pauseAlpha;
        private readonly InputAction _pauseAction;

        private bool _secretObtained = false;

        private Cube cube;

        #endregion

        #region PublicFields


        public Texture2D Circle;

        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        FireworkParticleSystem _fireworks;


        #endregion

        public LevelThreeScreen()
        {
            string text = File.ReadAllText("progress.txt");
            if (text.Contains("Level: Level 2\n Score: " + ScreenManager.score))
            {
                text = text.Replace("Level: Level 2", "Level: Level 2\n Score: " + ScreenManager.score);
                File.WriteAllText("progress.txt", text);


            }
            else
            {
                using (StreamWriter sw = new StreamWriter("progress.txt"))
                {

                    sw.WriteLine("Level: Level 2\n Score: " + ScreenManager.score);
                }
            }

            //ScreenManager.gameState = GameState.LevelTwo;
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            _pauseAction = new InputAction(
                new[] { Buttons.Start, Buttons.Back },
                new[] { Keys.Back, Keys.Escape }, true);
        }
    }
}
