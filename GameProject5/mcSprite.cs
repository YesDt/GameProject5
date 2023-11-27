using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
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

        private short _animationFrame;

        private Vector2 _direction;

        private List<PunchProjectile> _projList = new List<PunchProjectile>();

        private bool _hasShot = false;
        #endregion

        #region publicFields
        public Vector2 _position = new Vector2();

        public double Attackingtimer = 0;

        public bool Flipped;

        public bool OffGround = false;

        public int coinsCollected;

        public Action action;

        public float Wall;

        public bool Attacking = false;

        public short AnimationFrame => _animationFrame;


        public Vector2 Position => _position;

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
            _position = pos;
            _bounds = new BoundingRectangle(new Vector2(_position.X - 32, _position.Y - 16), 48, 120);
            _feet = new BoundingRectangle(new(_bounds.X, _bounds.Y + 32), 32, 32);
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

        }


        /// <summary>
        /// Updates the Main character
        /// </summary>
        /// <param name="gameTime">The real time elapsed in the game</param>
        public void Update(GameTime gameTime)
        {
            HealthBar = new Rectangle(50, 420, Health, 50);
            _jumpHeight = 150;
            _gravity = 10;
            _direction = new Vector2(200 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);


            //priorKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            CollisionHandling(rectangle);

            for (int i = 0; i < coinsCollected; i++)
            {
                _direction += new Vector2(0.75f, 0);
                if (_direction.X > 300) _direction.X = 300;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.A) ||
                _currentKeyboardState.IsKeyDown(Keys.Left))
            {
                _position += -_direction;
                action = Action.Running;
                Flipped = true;
            }
            if (_currentKeyboardState.IsKeyDown(Keys.D) ||
                _currentKeyboardState.IsKeyDown(Keys.Right))
            {
                _position += _direction;
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
                _position.Y += _velocityY;


            }
            //Jump Function
            if (_currentKeyboardState.IsKeyDown(Keys.Space) && !OffGround)
            {
                _velocityY -= _jumpHeight;
                if (!Attacking) _animationFrame = 0;
                if (!Attacking) _animationTimer = 0;




            }
            _position.Y += _velocityY;

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
                //Attacking = false;
                //Attackingtimer = 0;
            }

            if (Attackingtimer > 0.4)
            {
                Attacking = false;
                Attackingtimer = 0;
                _hasShot = false;
            }


            if (_position.X < 0) _position.X = 0;
            if (_position.X > Wall) _position.X = Wall;

            _bounds.X = _position.X;
            _bounds.Y = _position.Y;
            _feet.X = _position.X;
            _feet.Y = _bounds.Bottom;

            foreach(var proj in _projList.ToList())
            {
                
                if (proj.Expired)
                {
                    _projList.Remove(proj);
                }
                else
                {
                    proj.update(gameTime);
                }

            }

        }


        public void addProjectile()
        {
            var proj = new PunchProjectile(new Vector2(_position.X + 30, _position.Y + 20), this);
            _projList.Add(proj);
        }

        public void CollisionHandling(BoundingRectangle rect)
        {
            rectangle = rect;

            if (_feet.CollidesWith(rect))
            {
                OffGround = false;
                _position.Y = rect.Y - (_bounds.Height + _feet.Height);
            }
            else if (_bounds.CollidesWith(rect))
            {
                if (_bounds.Top == rect.Bottom)
                {
                    _position.Y = rect.Bottom + 1f;
                }
                else if (_bounds.Left == rect.Right)
                {
                    _position.X = rect.Right + 1f;

                }
                else if (_bounds.Right == rect.Left)
                {
                    _position.X = rect.Left - 1f;
                }
            }
            else
            {
                OffGround = true;
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
            //Update animationFrame
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
            
            spriteBatch.Draw(_texture, _position, source, Color.White, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
            foreach (var proj in _projList)
            {
                proj.Draw(gameTime, spriteBatch);
               
            }


        }
        #endregion
    }
}