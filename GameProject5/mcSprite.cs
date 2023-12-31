﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using GameProject5.Collisions;

namespace GameProject5
{
    /// <summary>
    /// States the main character will be in
    /// </summary>
    public enum Action
    {
        Idle = 0,
        Running = 1,
        Jumping = 2,
        Attacking = 3,
    }


    /// <summary>
    /// Class for the main character sprite
    /// </summary>
    public class mcSprite
    {
        #region privateFields
        private Texture2D _texture;




        private KeyboardState _currentKeyboardState;
        //private KeyboardState priorKeyboardState;

        private BoundingRectangle _bounds;

        private BoundingRectangle _feet;

        private float _velocityY = 0;

        private float _gravity;

        private float _jumpHeight;

        private double _animationTimer;

        private double _flickerTimer;

        private short _animationFrame;

        private Vector2 _direction;

        public List<PunchProjectile> ProjList = new List<PunchProjectile>();

        private bool _hasShot = false;

        private SoundEffect _jump;

        private SoundEffect _punch;
        #endregion

        #region publicFields
        public Vector2 Position = new Vector2();

        public double Attackingtimer = 0;

        public bool Flipped;

        public bool OffGround = false;

        public int coinsCollected;

        public Action action;

        public float Wall;

        public bool Attacking = false;

        public bool Hurt = false;

        public bool Recovering = false;

        public short AnimationFrame => _animationFrame;

        public double RecoveryTime = 2;

        //public Vector2 Position => Position;

        public BoundingRectangle Bounds => _bounds;

        public BoundingRectangle FeetBounds => _feet;


        public BoundingRectangle rectangle;


        public Rectangle HealthBar;

        public int Health = 100;

        public Texture2D HealthTexture;
        public Texture2D HealthBarTexture;
        #endregion

        #region publicMethods

        public mcSprite(Vector2 pos)
        {
            Position = pos;
            _bounds = new BoundingRectangle(new Vector2(Position.X - 32, Position.Y - 16), 48, 120);
            _feet = new BoundingRectangle(new(_bounds.X -32, _bounds.Y + 32), 48, 32);
        }

        /// <summary>
        /// Loads the Main character sprite
        /// </summary>
        /// <param name="content">ContentManager</param>
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Sprite_MC1");
           
            PunchProjectile.LoadContent(content);

            HealthTexture = content.Load<Texture2D>("PlayerHealth");
            HealthBarTexture = content.Load<Texture2D>("healthBar");
            _jump = content.Load<SoundEffect>("Jump");
            _punch = content.Load<SoundEffect>("Punchsound");

        }


        /// <summary>
        /// Updates the Main character
        /// </summary>
        /// <param name="gameTime">The real time elapsed in the game</param>
        public void Update(GameTime gameTime)
        {
            HealthBar = new Rectangle(50, 420, Health, 50);
            _jumpHeight = 150;
            _gravity = 7;
            _direction = new Vector2(200 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);


            //priorKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            CollisionHandling(rectangle);

            for (int i = 0; i < coinsCollected; i++)
            {
                _direction += new Vector2(0.75f, 0);
                if (_direction.X > 300) _direction.X = 300;
                foreach (var proj in ProjList.ToList())
                {
                    proj.Speed += 10;
                    
                }
            }
            if (_currentKeyboardState.IsKeyDown(Keys.A) ||
                _currentKeyboardState.IsKeyDown(Keys.Left))
            {
                Position += -_direction;
                action = Action.Running;
                Flipped = true;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.D) ||
                _currentKeyboardState.IsKeyDown(Keys.Right))
            {
                Position += _direction;
                action = Action.Running;
                Flipped = false;
            }
            if (!(_currentKeyboardState.IsKeyDown(Keys.A) ||
                _currentKeyboardState.IsKeyDown(Keys.Left)) &&
                !(_currentKeyboardState.IsKeyDown(Keys.D) ||
                _currentKeyboardState.IsKeyDown(Keys.Right))
                )
            {
                action = Action.Idle;
            }


            //Gravity Function
            if (OffGround)
            {
                action = Action.Jumping;
                _velocityY += _gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                Position.Y += _velocityY;


            }
            //Jump Function
            if (_currentKeyboardState.IsKeyDown(Keys.Space) && !OffGround)
            {
                _jump.Play();
                _velocityY -= _jumpHeight;
                if (!Attacking) _animationFrame = 0;
                if (!Attacking) _animationTimer = 0;




            }
            Position.Y += _velocityY;

            if (!OffGround)
            {
                _velocityY = 0;
            }

            if (_currentKeyboardState.IsKeyDown(Keys.Enter) && !Attacking && Attackingtimer == 0)
            {
                Attacking = true;
                _animationFrame = 0;
                _animationTimer = 0;


            }

            if (Attacking)
            {
                action = Action.Attacking;
                Attackingtimer += gameTime.ElapsedGameTime.TotalSeconds;
                
                
            }
            if (Attackingtimer >= 0.2 && Attackingtimer <= 0.3 && !_hasShot)
            {
                addProjectile();
                _hasShot = true;
            }

            if (Attackingtimer > 0.4)
            {
                Attacking = false;
                Attackingtimer = 0;
                _hasShot = false;
            }


            if (Position.X < 0) Position.X = 0;
            if (Position.X > Wall) Position.X = Wall;

            _bounds.X = Position.X;
            _bounds.Y = Position.Y;
            _feet.X = Position.X;
            _feet.Y = _bounds.Bottom;

            foreach(var proj in ProjList.ToList())
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

            Recover(gameTime);
            
        }


        public void addProjectile()
        {
            if (!Flipped)
            {
                var proj = new PunchProjectile(new Vector2(Position.X + 60, Position.Y + 10), this);
                ProjList.Add(proj);
                _punch.Play();
            }
           else
            {
                var proj = new PunchProjectile(new Vector2(Position.X - 60, Position.Y + 10), this);
                ProjList.Add(proj);
                _punch.Play();
            }
        }

        public void CollisionHandling(BoundingRectangle rect)
        {
            rectangle = rect;

            if (_feet.CollidesWith(rect) && _bounds.Top < rect.Top)
            {
                OffGround = false;
                Position.Y = rect.Y - (_bounds.Height + _feet.Height);
            }
            else if (_bounds.CollidesWith(rect))
            {


                if (Position.Y < rect.Bottom  && _bounds.Left > rect.Left && _bounds.Right < rect.Right)
                {
                    Position.Y += 20;
                }
                else if (Position.X < rect.Right  && _bounds.Right > rect.Right && _bounds.Bottom > rect.Top && _bounds.Top < rect.Bottom)
                {
                    Position.X += 20;

                }
                else if (_bounds.Right > rect.Left  && _bounds.Left < rect.Left && _bounds.Bottom > rect.Top && _bounds.Top < rect.Bottom)
                {
                    Position.X -= 20;
                }
            }
            else
            {
                OffGround = true;
            }

        }

        public void Recover(GameTime gameTime)
        {
            if (Hurt)
            {
                Hurt = false;
                Recovering = true;

            }
            else
            {
                RecoveryTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (RecoveryTime >= 2)
                {
                    Recovering = false;
                    RecoveryTime = 0;
                }
            }
           
        }

        /// <summary>
        /// Draws the main character
        /// </summary>
        /// <param name="gameTime">The real time elapsed in the game</param>
        /// <param name="spriteBatch">SpriteBatch</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            SpriteEffects spriteEffects = (Flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (Attacking)
            {

                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 0.1)
                {
                    _animationFrame++;
                    if (_animationFrame > 3) _animationFrame = 3;
                    _animationTimer -= 0.1;
                }

            }
            else if (OffGround)
            {

                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 0.1)
                {
                    _animationFrame++;

                    if (_animationFrame > 3) _animationFrame = 3;
                    _animationTimer -= 0.1;
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

            var source = new Rectangle(_animationFrame * 250, (int)action * 512, 268, 512);
            if (Recovering && (RecoveryTime > 0 && RecoveryTime < 2))
            {
                _flickerTimer += gameTime.ElapsedGameTime.TotalSeconds;
                {
                    if (_flickerTimer > 0.2)
                    {
                        spriteBatch.Draw(_texture, Position, source, new Color(Color.White, 0f), 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
                        _flickerTimer = 0;

                    }
                    else
                    {
                        spriteBatch.Draw(_texture, Position, source, Color.Blue, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
                    }
                }
              
            }
            else spriteBatch.Draw(_texture, Position, source, Color.White, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
            foreach (var proj in ProjList)
            {
                proj.Draw(gameTime, spriteBatch);
               
            }


        }
        #endregion
    }
}