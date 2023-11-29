using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject5.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using SharpDX.Direct3D9;

namespace GameProject5
{

    public enum doorState
    { 
        standing,
        open,
    }

    public class door
    {
        private Vector2 _position;
        private BoundingRectangle _bounds;

        private double _animationTimer;
        private short _animationFrame;

        public bool Opened = false;
        public Texture2D Texture;

        public doorState state;
        public BoundingRectangle Bounds => _bounds;


        public door(Vector2 position, BoundingRectangle bounds, Texture2D texture)
        {
            _position = position;
            _bounds = bounds;
            Texture = texture;
        }

        public void Update(GameTime gameTime)
        {
            if(Opened)
            {
                state = doorState.open;
                open();
            }
        }

        public void open()
        {
            _bounds = new BoundingRectangle(Vector2.Zero, 0, 0);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if(Opened)
            {
                _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_animationTimer > 0.1)
                {
                    _animationFrame++;
                    if (_animationFrame > 3) _animationFrame = 3;
                    _animationTimer -= 0.1;
                }

            }
            var source = new Rectangle(_animationFrame * 250, 0 * 512, 240, 420);
            spriteBatch.Draw(Texture, _position, source, Color.White, 0f, new Vector2(80, 80), 0.75f, SpriteEffects.None, 0);
        }
    }
}
