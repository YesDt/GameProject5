using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using GameProject5.Collisions;

namespace GameProject5
{
    public class Goal : Platform
    {
        private Texture2D _texture;
        public Goal(Vector2 Position, BoundingRectangle bounds) : base(Position, bounds)
        {

        }

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("Sprite_Finish");


        }

        public void Draw(SpriteBatch spriteBatch, GameTime gametime)
        {

            spriteBatch.Draw(_texture, Position, null, Color.White, 0f, new Vector2(Position.X - 640, Position.Y - 280), 1f, SpriteEffects.None, 0f);
        }
    }
}
