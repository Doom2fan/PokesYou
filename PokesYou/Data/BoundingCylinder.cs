using PokesYou.CMath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.Data {
    public struct BoundingCylinder {
        private Vector3k _position;
        public Accum Radius { get; set; }
        public Accum Height { get; set; }
        public Vector3k Position {
            get { return _position; }
            set { _position = value; }
        }

        // These are here for convenience. And stuff.
        public Accum X {
            get { return _position.X; }
            set { _position.X = value; }
        }
        public Accum Y {
            get { return _position.Y; }
            set { _position.Y = value; }
        }
        public Accum Z {
            get { return _position.Z; }
            set { _position.Z = value; }
        }
        public Accum Top { get { return _position.Z + Height; } }

        public BoundingCylinder (Accum radius, Accum height, Vector3k pos) {
            Radius = radius;
            Height = height;
            _position = pos;
        }

        public Vector3k IntersectionDistXY (BoundingCylinder b) {
            return new Vector3k (this.X - b.X, this.Y - b.Y, Accum.Zero);
        }

        public Accum IntersectionDistZ (BoundingCylinder b) {
            if (this.Z >= b.Z) // If the bottom of this bounding cylinder is inside b.
                return b.Top - this.Z;
            else if (this.Top <= b.Top) // Or if the top is inside b.
                return -(b.Z - this.Top);

            return Accum.Zero; // This shouldn't really happen.
        }

        public Vector3k IntersectionDist (BoundingCylinder b) {
            Vector3k dist = new Vector3k (this.X - b.X, this.Y - b.Y, Accum.Zero);
            dist.Z = IntersectionDistZ (b);

            return dist;
        }
        
        /// <summary>
        /// Checks for intersection with a bounding cylinder in the XY axes.
        /// </summary>
        /// <param name="b">The bounding cylinder to check for intersection with.</param>
        /// <returns>Returns a boolean indicating whether an intersection happened.</returns>
        public bool IntersectsXY (BoundingCylinder b) {
                return (FixedMath.Abs (IntersectionDistXY (b).LengthSquared) < FixedMath.Square (this.Radius + b.Radius));
        }

        /// <summary>
        /// Checks for intersection with a bounding cylinder in the Z axis.
        /// </summary>
        /// <param name="b">The bounding cylinder to check for intersection with.</param>
        /// <returns>Returns a boolean indicating whether an intersection happened.</returns>
        public bool IntersectsZ (BoundingCylinder b) {
            return (MathUtils.IsBetween (this.Z, b.Z, b.Top) || MathUtils.IsBetween (this.Top, b.Z, b.Top));
        }

        /// <summary>
        /// Checks for intersection with a bounding cylinder.
        /// </summary>
        /// <param name="b">The bounding cylinder to check for intersection with.</param>
        /// <returns>Returns a boolean indicating whether an intersection happened.</returns>
        public bool Intersects (BoundingCylinder b) {
            return (IntersectsXY (b) || IntersectsZ (b));
        }
    }
}
