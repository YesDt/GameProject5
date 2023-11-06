using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GameProject5.Collisions
{
    public class CollisionHelper
    {
        /// <summary>
        /// Detects collision between 2 bounding circles
        /// </summary>
        /// <param name="a">The first bounding circle</param>
        /// <param name="b">The second bounding circle</param>
        /// <returns>true for collision, false for otherwise</returns>
        public static bool Collides(BoundingCircle a, BoundingCircle b)
        {
            return Math.Pow(a.Radius, 2) + Math.Pow(b.Radius, 2) >=
                Math.Pow(a.Center.X - b.Center.X, 2) +
                Math.Pow(a.Center.Y - b.Center.Y, 2);
        }

        /// <summary>
        /// Detects collision between 2 bounding rectangles
        /// </summary>
        /// <param name="a">The first bounding rectangle</param>
        /// <param name="b">The second bounding rectangle</param>
        /// <returns>true for collision, false for otherwise</returns>
        public static bool Collides(BoundingRectangle a, BoundingRectangle b)
        {
            return !(a.Right < b.Left || a.Left > b.Right ||
                     a.Top > b.Bottom || a.Bottom < b.Top);
        }

        /// <summary>
        /// Detects collision between a bounding circle and a bounding rectangle
        /// </summary>
        /// <param name="a">The bounding circle</param>
        /// <param name="b">The bounding rectangle</param>
        /// <returns>true for collision, false for otherwise</returns>
        public static bool Collides(BoundingCircle c, BoundingRectangle r)
        {
            float nearestX = Math.Clamp(c.Center.X, r.Left, r.Right);
            float nearestY = Math.Clamp(c.Center.Y, r.Top, r.Bottom);
            return Math.Pow(c.Radius, 2) >=
                Math.Pow(c.Center.X - nearestX, 2) +
                Math.Pow(c.Center.Y - nearestY, 2);
        }

        public static bool Collides(BoundingRectangle r, BoundingCircle c) => Collides(c, r);
    }
}