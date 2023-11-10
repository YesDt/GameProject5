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




        private KeyboardState currentKeyboardState;
        private KeyboardState priorKeyboardState;


        private BoundingRectangle _bounds;

        private BoundingRectangle _feet;


        private float _velocityY = 0;

        private float _gravity;

        private float _jumpHeight;

        private double _animationTimer;

        

        private short _animationFrame;



        private Vector2 direction;
        #endregion

        #region publicFields
        public Vector2 _position = new Vector2();

        public double Attackingtimer = 0;

        public bool Flipped;

        public bool OffGround = false;

        public int coinsCollected;

        public Action action;

        public float Wall;

        public bool Attacked = false;

        public short AnimationFrame => _animationFrame;


        public Vector2 Position => _position;

        /// <summary>
        /// Boundaries for the bounding rectangle of the sprite
        /// </summary>
        public BoundingRectangle Bounds => _bounds;

        public BoundingRectangle FeetBounds => _feet;


        public BoundingRectangle rectangle;
        #endregion

        #region publicMethods

        public mcSprite(Vector2 pos)
        {
            _position = pos;
            _bounds = new BoundingRectangle(new Vector2(_position.X - 32, _position.Y - 16), 48, 120);
            _feet = new BoundingRectangle(new(_bounds.X, _bounds.Y + 32), 48, 32);
        }

        /// <summary>
        /// Loads the Main character sprite
        /// </summary>
        /// <param name="content">ContentManager</param>
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Sprite_MC1");


        }


        /// <summary>
        /// Updates the Main character
        /// </summary>
        /// <param name="gameTime">The real time elapsed in the game</param>
        public void Update(GameTime gameTime)
        {
            _jumpHeight = 150;
            _gravity = 10;
            direction = new Vector2(200 * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);


            priorKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            //if (_position.Y < 300)
            //{
            //    offGround = true;
            //}
            //if (_position.Y >= 300)
            //{
            //    _position.Y = 300;
            //    offGround = false;
            //}

            CollisionHandling(rectangle);

            for (int i = 0; i < coinsCollected; i++)
            {
                direction += new Vector2(0.75f, 0);
                if (direction.X > 300) direction.X = 300;
            }
            if (currentKeyboardState.IsKeyDown(Keys.A) ||
                currentKeyboardState.IsKeyDown(Keys.Left))
            {
                _position += -direction;
                action = Action.Running;
                Flipped = true;
            }
            if (currentKeyboardState.IsKeyDown(Keys.D) ||
                currentKeyboardState.IsKeyDown(Keys.Right))
            {
                _position += direction;
                action = Action.Running;
                Flipped = false;
            }
            if (!(currentKeyboardState.IsKeyDown(Keys.A) ||
                currentKeyboardState.IsKeyDown(Keys.Left)) &&
                !(currentKeyboardState.IsKeyDown(Keys.D) ||
                currentKeyboardState.IsKeyDown(Keys.Right))
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
            if (currentKeyboardState.IsKeyDown(Keys.Space) && !OffGround)
            {
                _velocityY -= _jumpHeight;
                if (!Attacked) _animationFrame = 0;
                if (!Attacked) _animationTimer = 0;




            }
            _position.Y += _velocityY;

            if (!OffGround)
            {
                _velocityY = 0;
                //CollisionHandling(rectangle);
            }

            if (currentKeyboardState.IsKeyDown(Keys.Enter) && !Attacked)
            {

                Attacked = true;
                _animationFrame = 0;
                _animationTimer = 0;


            }

            if (Attacked)
            {
                action = Action.Attacking;
                Attackingtimer += gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (Attackingtimer > 0.4)
            {
                Attacked = false;
                Attackingtimer = 0;
            }


            if (_position.X < 0) _position.X = 0;
            if (_position.X > Wall) _position.X = Wall;

            _bounds.X = _position.X;
            _bounds.Y = _position.Y;
            _feet.X = _position.X;
            _feet.Y = _bounds.Bottom;

        }


        //public void Collisions(BoundingRectangle rect)
        //{
        //    if (collidingAbove) _position.Y = rect.Bottom - 0.1f;
        //    if (collidingLeft) _position.X = rect.Right - 0.1f;
        //    if (collidingRight) _position.X = rect.Left - 0.1f;
        //}

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
            if (Attacked)
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


        }
        #endregion
    }
}