using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameProject5.Collisions;
using Microsoft.Xna.Framework;

namespace GameProject5.Collisions
{
    /// <summary>
    /// A struct representing certain bounds
    /// </summary>
    public struct BoundingCircle
    {
        /// <summary>
        /// The center of the bounding circle
        /// </summary>
        public Vector2 Center;

        /// <summary>
        /// The radius of the bounding circle
        /// </summary>
        public float Radius;

        /// <summary>
        /// Constructs a new bounding circle
        /// </summary>
        /// <param name="center">The center</param>
        /// <param name="radius">The radius</param>
        public BoundingCircle(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }

        /// <summary>
        /// Tests for a collision between this and another bounding circle
        /// </summary>
        /// <param name="other">The other bounding circle</param>
        /// <returns>true for collision, false otherwise</returns>
        public bool CollidesWith(BoundingCircle other)
        {
            return CollisionHelper.Collides(this, other);
        }

        /// <summary>
        /// Tests for a collision between this and a bounding rectangle
        /// </summary>
        /// <param name="other">The other bounding circle</param>
        /// <returns>true for collision, false otherwise</returns>
        public bool CollidesWith(BoundingRectangle other)
        {
            return CollisionHelper.Collides(this, other);
        }
    }
}