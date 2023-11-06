using GameProject5.Collisions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject5
{
    public class Platform
    {
        private Vector2 _position;

        public Vector2 Position => _position;

        public BoundingRectangle Bounds;

        public Platform(Vector2 Position, BoundingRectangle bounds)
        {
            _position = Position;
            Bounds = bounds;
        }

        public void Update(GameTime gameTime)
        {

        }
    }
}