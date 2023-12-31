﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject5.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

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

        private Random random = new Random();

        

      

        #endregion

        #region publicFields
        public Vector2 _position = new Vector2();

        public double Attackingtimer = 0;

        public bool Flipped = false;

        public EnemyAction Action = EnemyAction.Idle;

        public float BoundaryOne;

        public float BoundaryTwo;

        public float Radius;

        public float Health = 100;

        public bool Attacking = false;

        public bool Dead = false;

        public List<bullet> BulletList = new List<bullet>();

        public short AnimationFrame => _animationFrame;


        public Vector2 Position => _position;

        public BoundingRectangle Bounds => _bounds;

        public BoundingCircle Searching => _searching;

        public SoundEffect _death;
        #endregion

        #region publicMethods

        public enemy(Vector2 pos, float bOne, float bTwo, float radius)
        {
            _position = pos;
            Radius = radius;
            _bounds = new BoundingRectangle(new Vector2(_position.X - 32, _position.Y - 16), 48, 130);
            _searching = new BoundingCircle(new Vector2(_position.X, _position.Y), Radius);
            BoundaryOne = bOne;
            BoundaryTwo = bTwo;
        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Sprite_enemy");
            bullet.LoadContent(content);
            _death = content.Load<SoundEffect>("EnemyHit_Hurt");
        }

        public void Update(GameTime gameTime, mcSprite mc)
        {
            _direction = new Vector2(200 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
            //int randNum = random.Next(1, 3);
            if (Action == EnemyAction.Idle)
            {
                if (mc.Position.X < Position.X ) Flipped = true;
                else Flipped = false;
                _passiveTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_passiveTimer >= 2.5 && Action != EnemyAction.Attacking && Action != EnemyAction.Dying)
                {
                    Action = EnemyAction.Running;
                    _passiveTimer = 0;
                }
            }
            if (Action == EnemyAction.Running)
            {
                _passiveTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (!Flipped)
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
                    Flipped = false;
                }
                if (_position.X >= BoundaryTwo)
                {
                    _position.X = BoundaryTwo - 20;
                    Flipped = true;
                }
                if (_passiveTimer >= 2.5 && Action != EnemyAction.Attacking && Action != EnemyAction.Dying)
                {
                    Action = EnemyAction.Idle;
                    _passiveTimer = 0;
                }
            }

            if (Attacking)
            {
                _passiveTimer = 0;
                _searching = new BoundingCircle(Vector2.Zero, 0);
                Action = EnemyAction.Attacking;
            }
            if (Action == EnemyAction.Attacking)
            {
                if (mc.Position.X < Position.X ) Flipped = true;
                else Flipped = false;
                _attackingTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_attackingTimer >= 3 && !_hasShot)
                {
                    addBullet();
                    _hasShot = true;
                


                }
                if (_attackingTimer >= 4)
                {
                    _attackingTimer = 0;
                    if(!Attacking) Action = EnemyAction.Idle;
                    _hasShot = false;
                    Attacking = false;
                    _searching = new BoundingCircle(new Vector2(_position.X, _position.Y), Radius);
                }
                
            }
            if (Health <= 0)
            {
                _passiveTimer = 0;
                Dead = true;
            }
            if (Dead)
            {
                Attacking = false;
                _bounds = new BoundingRectangle(new Vector2(10000,10000), 0, 0);
                _searching = new BoundingCircle(new Vector2(10000, 10000), 0);
                Action = EnemyAction.Dying;
            }
            _bounds.X = _position.X;
            _bounds.Y = _position.Y;
            foreach (var proj in BulletList.ToList())
            {

                if (proj.Expired)
                {
                    BulletList.Remove(proj);
                }
                else
                {
                    proj.update(gameTime);
                }

            }
            _searching = new BoundingCircle(new Vector2(_position.X, _position.Y), Radius);
        }

        public void AboutToAttack()
        {
            Attacking = true;
        }

        public void addBullet()
        { 
            if (!Flipped)
            {
                var proj = new bullet(new Vector2(Position.X + 60, Position.Y + 3), this);
                BulletList.Add(proj);
            }
            else
            {
                var proj = new bullet(new Vector2(Position.X - 60, Position.Y + 3), this);
                BulletList.Add(proj);
            }
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
                    if (_animationTimer > 0.1)
                    {
                        _animationFrame++;
                        if (_animationFrame > 3) _animationFrame = 3;
                        _animationTimer -= 0.1;
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
               
                _attackingTimer = 0;
                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 0.1)
                {
                    _animationFrame++;
                    if (_animationFrame > 3) _animationFrame = 3;
                    _animationTimer -= 0.1;
                }
            }

            var source = new Rectangle(_animationFrame * 250, (int)Action * 512, 268, 512);
            if(Attacking && _attackingTimer >= 2.5 && _attackingTimer <= 3) spriteBatch.Draw(_texture, _position, source, Color.Red, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
            else spriteBatch.Draw(_texture, _position, source, Color.White, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);

            foreach (var proj in BulletList)
            {
                proj.Draw(gameTime, spriteBatch);

            }
            #endregion
        }
    }
}
