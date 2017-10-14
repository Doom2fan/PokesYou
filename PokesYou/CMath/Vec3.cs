using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.CMath {
    public struct Vector3k {
        public static readonly Vector3k Zero = new Vector3k (Accum.Zero, Accum.Zero, Accum.Zero);
        public static readonly Vector3k One = new Vector3k (Accum.One, Accum.One, Accum.One);

        private Accum x, y, z;

        public Vector3k (Accum newX, Accum newY, Accum newZ) {
            x = newX;
            y = newY;
            z = newZ;
        }
        public Vector3k (Vector3k vec) : this (vec.x, vec.y, vec.z) { }

        /// <summary>
        /// Gets or sets the vector's X value
        /// </summary>
        public Accum X {
            get { return x; }
            set { x = value; }
        }
        /// <summary>
        /// Gets or sets the vector's Y value
        /// </summary>
        public Accum Y {
            get { return y; }
            set { y = value; }
        }
        /// <summary>
        /// Gets or sets the vector's Z value
        /// </summary>
        public Accum Z {
            get { return z; }
            set { z = value; }
        }

        #region Properties
        /// <summary>
        /// Gets the length (magnitude) of the vector.
        /// </summary>
        /// <seealso cref="LengthSquared"/>
        public Accum Length {
            get { return FixedMath.Sqrt (x * x + y * y + z * z); }
        }
        /// <summary>
        /// Gets the square of the vector length (magnitude).
        /// </summary>
        /// <remarks>
        /// This property avoids the costly square root operation required by the Length property. This makes it more suitable
        /// for comparisons.
        /// </remarks>
        /// <see cref="Length"/>
        public Accum LengthSquared {
            get { return x * x + y * y + z * z; }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Returns a copy of the Vector3k scaled to unit length.
        /// </summary>
        public Vector3k Normalized () {
            Vector3k v = this;
            v.Normalize ();
            return v;
        }

        /// <summary>
        /// Scales the Vector3k to unit length.
        /// </summary>
        public void Normalize () {
            Accum scale = Accum.One / this.Length;
            x *= scale;
            y *= scale;
            z *= scale;
        }
        #endregion

        #region Operators
        // Unary
        public static Vector3k operator -(Vector3k vec) {
            vec.x = -vec.x;
            vec.y = -vec.y;
            vec.z = -vec.z;
            return vec;
        }

        // Binary
        #region Vector-Integer
        // Addition
        public static Vector3k operator +(Vector3k vec, long val) {
            var valAccum = new Accum (val);
            vec.x += valAccum;
            vec.y += valAccum;
            vec.z += valAccum;
            return vec;
        }
        public static Vector3k operator +(long val, Vector3k vec) {
            var valAccum = new Accum (val);
            vec.x += valAccum;
            vec.y += valAccum;
            vec.z += valAccum;
            return vec;
        }

        // Subtraction
        public static Vector3k operator -(Vector3k vec, int val) {
            var valAccum = new Accum (val);
            vec.x -= valAccum;
            vec.y -= valAccum;
            vec.z -= valAccum;
            return vec;
        }
        public static Vector3k operator -(int val, Vector3k vec) {
            var valAccum = new Accum (val);
            vec.x -= valAccum;
            vec.y -= valAccum;
            vec.z -= valAccum;
            return vec;
        }

        // Multiplication
        public static Vector3k operator *(Vector3k vec, int val) {
            var valAccum = new Accum (val);
            vec.x *= valAccum;
            vec.y *= valAccum;
            vec.z *= valAccum;
            return vec;
        }
        public static Vector3k operator *(int val, Vector3k vec) {
            var valAccum = new Accum (val);
            vec.x *= valAccum;
            vec.y *= valAccum;
            vec.z *= valAccum;
            return vec;
        }

        // Division
        public static Vector3k operator /(Vector3k vec, int val) {
            var valAccum = new Accum (val);
            vec.x /= valAccum;
            vec.y /= valAccum;
            vec.z /= valAccum;
            return vec;
        }
        public static Vector3k operator /(int val, Vector3k vec) {
            var valAccum = new Accum (val);
            vec.x /= valAccum;
            vec.y /= valAccum;
            vec.z /= valAccum;
            return vec;
        }
        #endregion

        #region Vector-Float
        // Addition
        public static Vector3k operator +(Vector3k vec, double val) {
            var valAccum = new Accum (val);
            vec.x += valAccum;
            vec.y += valAccum;
            vec.z += valAccum;
            return vec;
        }
        public static Vector3k operator +(double val, Vector3k vec) {
            var valAccum = new Accum (val);
            vec.x += valAccum;
            vec.y += valAccum;
            vec.z += valAccum;
            return vec;
        }

        // Subtraction
        public static Vector3k operator -(Vector3k vec, double val) {
            var valAccum = new Accum (val);
            vec.x -= valAccum;
            vec.y -= valAccum;
            vec.z -= valAccum;
            return vec;
        }
        public static Vector3k operator -(double val, Vector3k vec) {
            var valAccum = new Accum (val);
            vec.x -= valAccum;
            vec.y -= valAccum;
            vec.z -= valAccum;
            return vec;
        }

        // Multiplication
        public static Vector3k operator *(Vector3k vec, double val) {
            var valAccum = new Accum (val);
            vec.x *= valAccum;
            vec.y *= valAccum;
            vec.z *= valAccum;
            return vec;
        }
        public static Vector3k operator *(double val, Vector3k vec) {
            var valAccum = new Accum (val);
            vec.x *= valAccum;
            vec.y *= valAccum;
            vec.z *= valAccum;
            return vec;
        }

        // Division
        public static Vector3k operator /(Vector3k vec, double val) {
            var valAccum = new Accum (val);
            vec.x /= valAccum;
            vec.y /= valAccum;
            vec.z /= valAccum;
            return vec;
        }
        public static Vector3k operator /(double val, Vector3k vec) {
            var valAccum = new Accum (val);
            vec.x /= valAccum;
            vec.y /= valAccum;
            vec.z /= valAccum;
            return vec;
        }
        #endregion

        #region Vector-Accum
        // Addition
        public static Vector3k operator +(Vector3k vec, Accum val) {
            vec.x += val;
            vec.y += val;
            vec.z += val;
            return vec;
        }
        public static Vector3k operator +(Accum val, Vector3k vec) {
            vec.x += val;
            vec.y += val;
            vec.z += val;
            return vec;
        }

        // Subtraction
        public static Vector3k operator -(Vector3k vec, Accum val) {
            vec.x -= val;
            vec.y -= val;
            vec.z -= val;
            return vec;
        }
        public static Vector3k operator -(Accum val, Vector3k vec) {
            vec.x -= val;
            vec.y -= val;
            vec.z -= val;
            return vec;
        }

        // Multiplication
        public static Vector3k operator *(Vector3k vec, Accum val) {
            vec.x *= val;
            vec.y *= val;
            vec.z *= val;
            return vec;
        }
        public static Vector3k operator *(Accum val, Vector3k vec) {
            vec.x *= val;
            vec.y *= val;
            vec.z *= val;
            return vec;
        }

        // Division
        public static Vector3k operator /(Vector3k vec, Accum val) {
            vec.x /= val;
            vec.y /= val;
            vec.z /= val;
            return vec;
        }
        public static Vector3k operator /(Accum val, Vector3k vec) {
            vec.x /= val;
            vec.y /= val;
            vec.z /= val;
            return vec;
        }
        #endregion

        #region Vector-Vector
        public static Vector3k operator +(Vector3k lhs, Vector3k rhs) {
            lhs.x += rhs.x;
            lhs.y += rhs.y;
            lhs.z += rhs.z;
            return lhs;
        }
        public static Vector3k operator -(Vector3k lhs, Vector3k rhs) {
            lhs.x -= rhs.x;
            lhs.y -= rhs.y;
            lhs.z -= rhs.z;
            return lhs;
        }
        public static Vector3k operator *(Vector3k lhs, Vector3k rhs) {
            lhs.x *= rhs.x;
            lhs.y *= rhs.y;
            lhs.z *= rhs.z;
            return lhs;
        }
        public static Vector3k operator /(Vector3k lhs, Vector3k rhs) {
            lhs.x /= rhs.x;
            lhs.y /= rhs.y;
            lhs.z /= rhs.z;
            return lhs;
        }
        #endregion
        #endregion

        // TODO: Add vector rotation functions
        /*
        /// <summary>
        /// Rotates the vector using the specified values
        /// </summary>
        /// <param name="yaw">The yaw to rotate by</param>
        /// <param name="pitch">The pitch to rotate by</param>
        /// <param name="roll">The roll to rotate by</param>
        public void Rotate (Accum yaw, Accum pitch, Accum roll) {

        }

        /// <summary>
        /// Rotates the vector using the values from the specified vector
        /// </summary>
        /// <param name="val">A rotation vector (X: yaw, Y: pitch, Z: roll)</param>
        public void Rotate (Vector3k val) {
            Rotate (val.x, val.y, val.z);
        }
        */
    }
}
