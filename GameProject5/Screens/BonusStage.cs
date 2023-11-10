using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject5.Particles;
using GameProject5.StateManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace GameProject5.Screens
{
    public class BonusStage : GameScreen, IParticleEmitter
    {
        #region PrivateFields
        private GraphicsDevice _graphics;
        private SpriteBatch _spriteBatch;


        private ContentManager _content;

        private SpriteFont _gameFont;

        private mcSprite _mc = new mcSprite(new Vector2(200, 250));
        private List<PunchProjectile> _p;
        private CoinSprite[] _coins;
        private Platform _platforms;


        private Texture2D _level;

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

        public BonusStage()
        {

        }
    }
}
