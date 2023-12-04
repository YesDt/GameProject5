using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject5.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace GameProject5
{
    public enum BossAction
    {
        Idle = 0,
        ShoulderCharge = 1,
        FingerFlick = 2,
        
    }
    public class Boss
    {
        #region privateFields
        private Texture2D _texture;

        private BoundingRectangle _bounds;

        

        private double _animationTimer;

        public double _attackingTimer;

        private short _animationFrame;

        private double _passiveTimer = 0;

        private bool _hasShot = false;

        private Vector2 _direction;

        private Random random = new Random();



        #endregion

        #region publicFields
        public Vector2 Position = new Vector2();

        //public double AttackingTimer = 0;

        public double RecoveryTimer = 0;

        public bool Flipped = false;

        public BossAction Action = BossAction.Idle;

        public float Boundary;

        public int Health = 100;

        public bool Attacking = false;

        public List<BossFingerFlick> ProjList = new List<BossFingerFlick>();

        public short AnimationFrame => _animationFrame;


        public BoundingRectangle Bounds => _bounds;


        public Texture2D HealthTexture;
        public Texture2D HealthBarTexture;
        public Rectangle HealthBar;

        #endregion

        #region publicMethods

        public Boss(Vector2 pos)
        {
            Position = pos;
            _bounds = new BoundingRectangle(new Vector2(Position.X - 32, Position.Y - 16), 48, 130);

        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Sprite_boss");
            BossFingerFlick.LoadContent(content);
            HealthTexture = content.Load<Texture2D>("BossHealth");
            HealthBarTexture = content.Load<Texture2D>("BossHealthBar");
        }

        public void Update(GameTime gameTime, mcSprite mc)
        {
            
            _direction = Vector2.Zero;
            HealthBar = new Rectangle(600, 420, Health, 50);
            if (Action == BossAction.Idle)
            {
                _bounds = new BoundingRectangle(new Vector2(Position.X, Position.Y), 48, 128);
                if (mc.Position.X < Position.X) Flipped = true;
                else Flipped = false;
                _passiveTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_passiveTimer > 2)
                {
                    int randomNum = random.Next(1, 3);
                    if (randomNum == 1)
                    {
                        Action = BossAction.ShoulderCharge;
                        _animationFrame = 0;
                        randomNum = 0;
                    }
                    else
                    {
                        Action = BossAction.FingerFlick;
                        _animationFrame = 0;
                        randomNum = 0;
                    }
                    _passiveTimer = 0;
                }
            }
            if (Action == BossAction.ShoulderCharge)
            {
                _bounds = new BoundingRectangle(new Vector2(Position.X, Position.Y + 64), 48, 56);
                
                Attacking = true;
                _attackingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (mc.Position.X < Position.X && _attackingTimer <= 2) Flipped = true;

                if (_attackingTimer > 3)
                {
                    _direction = new Vector2(300 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
                    if (Flipped) Position -= _direction;
                    else Position += _direction;

                }

                if (Flipped && Position.X < 0)
                {
                    Position.X += 20;
                    _direction = Vector2.Zero;
                    _attackingTimer = 0;
                    RecoveryTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    Attacking = false;
                    Action = BossAction.Idle;
                }
                if (!Flipped && Position.X > Boundary)
                {
                    Position.X -= 20;
                    _direction = Vector2.Zero;
                    _attackingTimer = 0;
                    Attacking = false;
                    Action = BossAction.Idle;
                }
          
            }
            if (Action == BossAction.FingerFlick)
            {
                Attacking = true;
                _attackingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_attackingTimer > 2 && !_hasShot)
                { 
                    addFF();
                    _hasShot = true; 

                }
                if (_attackingTimer >= 4)
                {
                    _attackingTimer = 0;
                    Action = BossAction.Idle;
                    _hasShot = false;
                    Attacking = false;

                }
            }

            _bounds.X = Position.X;
            _bounds.Y = Position.Y;
            foreach (var proj in ProjList.ToList())
            {

                if (proj.Expired)
                {
                    ProjList.Remove(proj);
                }
                else
                {
                    proj.update(gameTime);
                }

            }
        }

        

        public void addFF()
        {
            if (!Flipped)
            {
                var proj = new BossFingerFlick(new Vector2(Position.X , Position.Y - 100), this);
                ProjList.Add(proj);
            }
            else
            {
                var proj = new BossFingerFlick(new Vector2(Position.X, Position.Y - 100), this);
                ProjList.Add(proj);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = (Flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (Action == BossAction.ShoulderCharge)
            {
                if (_attackingTimer <= 2)
                {
                    _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_animationTimer > 0.2)
                    {
                        if (_animationFrame < 1) _animationFrame++;
                        
                        _animationTimer -= 0.2;
                    }
                }

                else
                {
                    _animationFrame = 2;
                }
            }
            if (Action == BossAction.FingerFlick)
            {
                if (!_hasShot)
                {
                    _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_animationTimer > 0.1)
                    {
                        _animationFrame++;
                        if (_animationFrame > 1) _animationFrame = 1;
                        _animationTimer -= 0.1;
                    }
                }
                else
                {
                    _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_animationTimer > 0.1)
                    {
                        _animationFrame++;
                        if (_animationFrame > 3) _animationFrame = 3;
                        _animationTimer -= 0.1;
                    }
                }
            }
            else
            {
                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

                if (_animationTimer > 0.2)
                {
                    _animationFrame++;
                    if (_animationFrame > 3)
                    {
                        _animationFrame = 0;

                    }
                    _animationTimer -= 0.2;
                }
            }

            var source = new Rectangle(_animationFrame * 250, (int)Action * 512, 268, 512);
            if (Action == BossAction.ShoulderCharge && _attackingTimer >= 2 && _attackingTimer <= 2.5) spriteBatch.Draw(_texture, Position, source, Color.Red, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
            else if (Action == BossAction.FingerFlick && _attackingTimer >= 1.5 && _attackingTimer <= 2) spriteBatch.Draw(_texture, Position, source, Color.Red, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
            else spriteBatch.Draw(_texture, Position, source, Color.White, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);

            foreach (var proj in ProjList)
            {
                proj.Draw(gameTime, spriteBatch);

            }
            #endregion
        }
    }
}
