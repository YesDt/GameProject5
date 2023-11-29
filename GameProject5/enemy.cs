using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject5.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject5
{
    public enum EnemyAction
    {
        Idle = 0,
        Running = 1,
        Attacking = 2,
        Dying = 3,
    }
    public class enemy
    {
        #region privateFields
        private Texture2D _texture;

        private BoundingRectangle _bounds;

        private BoundingCircle _searching;

        private double _animationTimer;

        public double _attackingTimer;

        private short _animationFrame;

        private double _passiveTimer = 0;

        private Vector2 _direction;

        private bool _hasShot = false;

        private bool _flipped = false;

        private Random random = new Random();

        private List<bullet> _projList = new List<bullet>();

        #endregion

        #region publicFields
        public Vector2 _position = new Vector2();

        public double Attackingtimer = 0;

        public bool Flipped;

        public EnemyAction Action = EnemyAction.Idle;

        public float BoundaryOne;

        public float BoundaryTwo;

        public float Health = 100;

        public bool Attacking = false;


        public short AnimationFrame => _animationFrame;


        public Vector2 Position => _position;

        public BoundingRectangle Bounds => _bounds;

        public BoundingCircle Searching => _searching;


        #endregion

        #region publicMethods

        public enemy(Vector2 pos, float bOne, float bTwo)
        {
            _position = pos;
            _bounds = new BoundingRectangle(new Vector2(_position.X - 32, _position.Y - 16), 48, 130);
            _searching = new BoundingCircle(new Vector2(_position.X, _position.Y), 64);
            BoundaryOne = bOne;
            BoundaryTwo = bTwo;
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Sprite_enemy");
        }

        public void Update(GameTime gameTime)
        {
            _direction = new Vector2(200 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
            //int randNum = random.Next(1, 3);
            if (Action == EnemyAction.Idle)
            {
                _passiveTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_passiveTimer >= 3 && Action != EnemyAction.Attacking && Action != EnemyAction.Dying)
                {
                    Action = EnemyAction.Running;
                    _passiveTimer = 0;
                }
            }
            if (Action == EnemyAction.Running)
            {
                _passiveTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (!_flipped)
                {
                    _position += _direction;
                }
                else
                {
                    _position -= _direction;
                }

                if (_position.X <= BoundaryOne)
                {
                    _position.X = BoundaryOne + 20;
                    _flipped = false;
                }
                if (_position.X >= BoundaryTwo)
                {
                    _position.X = BoundaryTwo - 20;
                    _flipped = true;
                }
                if (_passiveTimer >= 3 && Action != EnemyAction.Attacking && Action != EnemyAction.Dying)
                {
                    Action = EnemyAction.Idle;
                    _passiveTimer = 0;
                }
            }

            //if (randNum == 1 && Action != EnemyAction.Attacking && Action != EnemyAction.Dying && _animationTimer == 0)
            //{
            //    Action = EnemyAction.Idle;
            //}
            //else if (randNum == 2 && Action != EnemyAction.Attacking && Action != EnemyAction.Dying && _animationTimer == 0)
            //{
            //    Action = EnemyAction.Running;
            //    if (!_flipped)
            //    {
            //        _position += _direction;
            //    }
            //    else
            //    {
            //        _position -= _direction;
            //    }

            //    if (_position.X <= BoundaryOne)
            //    {
            //        _position.X = BoundaryOne + 20;
            //        _flipped = false;
            //    }
            //    if (_position.X >= BoundaryTwo)
            //    {
            //        _position.X = BoundaryTwo - 20;
            //        _flipped = true;
            //    }
            //}
            if (Attacking) Action = EnemyAction.Attacking;
            if (Action == EnemyAction.Attacking)
            {
                _attackingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_attackingTimer >= 4)
                {
                    _hasShot = true;
                    _attackingTimer = 0;
                }
            }
            if (Health <= 0)
            {
                Action = EnemyAction.Dying;
            }
            if (Action == EnemyAction.Dying)
            {
                _bounds = new BoundingRectangle(Vector2.Zero, 0, 0);
                _searching = new BoundingCircle(Vector2.Zero, 0);
            }
            _bounds.X = _position.X;
            _bounds.Y = _position.Y;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = (Flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (Action == EnemyAction.Attacking)
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
                    if (_animationTimer > 0.2)
                    {
                        _animationFrame++;
                        if (_animationFrame > 3) _animationFrame = 3;
                        _animationTimer -= 0.2;
                    }
                }
            }
            if (Action == EnemyAction.Idle || Action == EnemyAction.Running)
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

            if (Action == EnemyAction.Dying)
            {
                _animationFrame = 0;
                _animationTimer = 0;
                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 0.1)
                {
                    _animationFrame++;
                    if (_animationFrame > 3) _animationFrame = 3;
                    _animationTimer -= 0.1;
                }
            }

            var source = new Rectangle(_animationFrame * 250, (int)Action * 512, 360, 440);
            if(_attackingTimer >= 3 && _attackingTimer <= 4) spriteBatch.Draw(_texture, _position, source, Color.Red, 0f, new Vector2(80, 140), 0.5f, spriteEffects, 0);
            spriteBatch.Draw(_texture, _position, source, Color.White, 0f, new Vector2(80, 140), 0.5f, spriteEffects, 0);
            #endregion
        }
    }
}
