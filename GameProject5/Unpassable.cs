using GameProject5.Collisions;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameProject5
{
    public class Unpassable
    {
        public BoundingRectangle Bounds;


        public Unpassable(BoundingRectangle bounds)
        {
            Bounds = bounds;
        }
        public void Update(GameTime gameTime)
        {

        }
    }
}
