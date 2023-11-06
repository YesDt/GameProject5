using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameProject5.Collisions;
using GameProject5;

namespace GameProject5
{
    public enum state
    {
        traveling,
        connected
    }

    public class PunchProjectile
    {
        private Texture2D _texture;

        private Vector2 _position;

        private BoundingRectangle _bounds;

        private double _projTimer;

        private double _animationTimer;

        private short _animationFrame;

        public float Speed = 300;

        public state projState = state.traveling;

        public bool Flipped;

        public bool Expired = false;

        public Vector2 Position => _position;

        public BoundingRectangle Bounds => _bounds;

        public PunchProjectile(Vector2 pos, mcSprite mc)
        {
            _position = pos;
            if (mc.Flipped) this.Flipped = true;
        }


        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Sprite_PunchProjectile ");


        }

        public void update(GameTime gameTime)
        {
            if (Flipped)
            {
                _position -= new Vector2(Speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
            }
            else
            {
                _position += new Vector2(Speed * (float)gameTime.ElapsedGameTime.TotalSeconds, 0);
            }

            _bounds = new BoundingRectangle(new Vector2(_position.X - 32, _position.Y - 32), 20, 40);
            _projTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_projTimer >= 1)
            {
                Destroy(this);
            }

        }

        public void Destroy(PunchProjectile p)
        {
            p._texture.Dispose();
            p._bounds = new BoundingRectangle(Vector2.Zero, 0, 0);
            Expired = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = (Flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (projState == state.traveling)
            {
                if (_projTimer < 2)
                {
                    _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                    if (_animationTimer > 0.1)
                    {
                        _animationFrame++;
                        if (_animationFrame > 1) _animationFrame = 0;
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
            var source = new Rectangle(_animationFrame * 250, (int)projState * 512, 268, 480);
            spriteBatch.Draw(_texture, _position, source, Color.White, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
        }
    }
}