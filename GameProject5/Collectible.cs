using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject5.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameProject5
{
    public class Collectible
    {
        private Vector2 _position;
        private BoundingRectangle _recBounds;
        private BoundingCircle _circleBounds;
        public Texture2D cTexture;


        public BoundingRectangle RecBounds => _recBounds;

        public BoundingCircle CircleBounds => _circleBounds;

        public Collectible(Vector2 pos, BoundingRectangle rBounds)
        {
            _position = pos;
            _recBounds = rBounds;
        }

        public Collectible(Vector2 pos, BoundingCircle cBounds)
        {
            _position = pos;
            _circleBounds = cBounds;
        }

        public void destroy()
        {
            _recBounds = new BoundingRectangle(0, 0, 0, 0);
            _circleBounds = new BoundingCircle(Vector2.Zero, 0);
        }
    }
}
