using GameProject5.Collisions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject5
{
    public class BossFingerFlick
    {
        private static Texture2D _texture;

        public Vector2 Position;

        private BoundingRectangle _bounds;

        private double _animationTimer;

        private short _animationFrame;

        private float _gravity = 1;

        public float Speed = 350;


        public bool Flipped;

        public bool Connected = false;

        public bool Expired = false;

        // public Vector2 Position => _position;

        public BoundingRectangle Bounds => _bounds;

        public BossFingerFlick(Vector2 pos, Boss b)
        {
            Position = pos;
            if (b.Flipped) this.Flipped = true;
        }


        public static void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Sprite_bossMiddleFinger");


        }

        public void update(GameTime gameTime)
        {
        
            if (Flipped)
            {
                
                Position += new Vector2(-(Speed * (float)gameTime.ElapsedGameTime.TotalSeconds), _gravity);
                if (Connected)
                {
                    Speed = 0;
                    Position -= Vector2.Zero;
                }
            }
            else
            {
                Position += new Vector2(Speed * (float)gameTime.ElapsedGameTime.TotalSeconds, _gravity);
                if (Connected)
                {
                    Speed = 0;
                    Position += Vector2.Zero;
                }
            }

            if (Connected) Destroy(this);

            _bounds = new BoundingRectangle(new Vector2(Position.X, Position.Y), 48, 56);
            

        }

        public void Destroy(BossFingerFlick f)
        {

            f._bounds = new BoundingRectangle(Vector2.Zero, 0, 0);
            Expired = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            SpriteEffects spriteEffects = (Flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            if (Connected)
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
                _animationFrame = 0;
            }
            var source = new Rectangle(_animationFrame * 250, 0, 268, 440);
            if (Connected) spriteBatch.Draw(_texture, Position, source, Color.White, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
            else spriteBatch.Draw(_texture, Position, source, Color.White, 0f, new Vector2(80, 120), 0.5f, spriteEffects, 0);
        }
    }
}
