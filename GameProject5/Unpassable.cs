using GameProject5.Collisions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject5
{
    public class Unpassable : Platform
    {
        public float OriginX;

        public float OriginY;

        public Unpassable(Vector2 Position, BoundingRectangle bounds, float oX, float oY) : base(Position, bounds)
        {
            OriginX = oX;
            OriginY = oY;
        }
        public void Update(GameTime gameTime)
        {

        }
    }
}
