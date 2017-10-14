using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PokesYou.CMath {
    public struct ByteAngles {
        public const uint Angle_45 = 0x20000000;
        public const uint Angle_90 = 0x40000000;
        public const uint Angle_180 = 0x80000000;
        public const uint Angle_270 = 0xc0000000;
        public const uint Angle_MAX = 0xffffffff;
        public const uint Angle_1 = Angle_45 / 45;
        public const uint Angle_60 = Angle_180 / 3;
    }
    
    public static class FixedMath {
        public static readonly Accum PI = Accum.MakeAccum (205887); // 205887 = (int) (3.141592653589793238462643383279502884 * 65536)
        public static readonly Accum PI_2 = Accum.MakeAccum (205887 * 2);
        public static readonly Accum PI_4 = Accum.MakeAccum (205887 * 4);
        public static readonly Accum PIOver2 = Accum.MakeAccum (205887 / 2);
        public static readonly Accum PIOver4 = Accum.MakeAccum (205887 / 4);

        /*static FixedMath () {
            GenerateTables ();
        }*/

        #region Trigonometry
        private const int SineLUTSize = 131072;
        private static Accum [] SineLUT = new Accum [SineLUTSize + 1]; // The +1 is so the linear interpolation doesn't fuck shit up
        private static bool tablesGenerated = false;
        // Sin/Cos: pi*2: 411774; LUT size: 131072;

        // LUT generation
        public static void GenerateTables () {
            for (int i = 0; i < SineLUT.Length; i++) {
                SineLUT [i] = new Accum (Math.Sin ((Math.PI * 2 / SineLUTSize) * i));
            }

            tablesGenerated = true;
        }

        public static Accum Sin (Accum x) {
            if (!tablesGenerated)
                GenerateTables ();

            long xVal = x.Value;
            while (xVal > PI_2.Value)
                xVal -= PI_2.Value;
            while (xVal < 0)
                xVal += PI_2.Value;

            long x1 = (long) (Math.Floor (((double) xVal / Accum.FracUnit) / (Math.PI * 2 / SineLUTSize))), //(Floor (xVal / (PI_2 / SineLUTSize))),
                 x2 = x1 + 1;

            Accum y1 = SineLUT [x1],
                  y2 = SineLUT [x2],
                  y = y1 + (y2 - y1) * (x * new Accum (SineLUTSize) / PI_2 - new Accum (x1));

            return y1;
        }
        public static Accum Cos (Accum x) { return Sin (x + PIOver2); }

        public static Accum SinDegrees (Accum x) { return Sin (DegreesToRadians (x)); }
        public static Accum CosDegrees (Accum x) { return Sin (DegreesToRadians (x) + PIOver2); }
        /*public static Accum AtanDegrees (Accum x) { return DegreesToRadians (Atan (x)); }
        public static Accum Atan2Degrees (Accum x, Accum y) { return DegreesToRadians (Atan2 (x, y); }*/
        #endregion

        #region Basic math functions
        // Absolute
        /// <summary>
        /// Returns the absolute value of a number
        /// </summary>
        /// <param name="x">A number</param>
        public static Accum Abs (Accum val) {
            return val.Value < 0 ? -val : val;
        }

        // Potentiation (?)
        /// <summary>
        /// Returns the square of a number
        /// </summary>
        /// <param name="x">A number</param>
        public static Accum Square (Accum x) {
            return x * x;
        }

        /// <summary>
        /// Returns x to the nth potency
        /// </summary>
        /// <param name="x">A number</param>
        /// <param name="n">An exponent</param>
        public static Accum Pow (Accum x, int n) {
            Accum ret = x;
            for (; n > 0; n--)
                ret.Value *= x.Value;

            return ret;
        }

        // Square root
        /// <summary>
        /// Returns the square root of a number
        /// </summary>
        /// <param name="x">A number</param>
        public static Accum Sqrt (Accum x) {
            if (x == Accum.One)
                return new Accum (Accum.FracUnit);
            if (x <= Accum.Zero)
                return Accum.Zero;

            long val = 150 * Accum.FracUnit,
                xVal = x.Value;
            for (int i = 0; i < 15; i++)
                val = (val + Accum.SafeDivision (xVal, val)) >> 1;

            return Accum.MakeAccum (val);
        }

        // Rounding
        /// <summary>
        /// Rounds a number downwards to the next integer. (Towards negative infinity)
        /// </summary>
        /// <param name="x">The number to round.</param>
        /// <returns>Returns x rounded downwards to the next integer.</returns>
        public static Accum Floor (Accum x) {
            return Accum.MakeAccum (
                (x.Value < 0) ?
                    ((x.Value + (Accum.FracUnit - 1)) & unchecked((long) 0xFFFFFFFFFFFF0000)) :
                    (x.Value & 0xFFFF0000)
            );
        }

        /// <summary>
        /// Rounds a number upwards to the next integer. (Towards positive infinity)
        /// </summary>
        /// <param name="x">The number to round.</param>
        /// <returns>Returns x rounded upwards to the next integer.</returns>
        public static Accum Ceil (Accum x) {
            return Accum.MakeAccum (
                (x.Value < 0) ?
                    (x.Value & unchecked((long) 0xFFFFFFFFFFFF0000)) :
                    ((x.Value + (Accum.FracUnit - 1)) & unchecked((long) 0xFFFFFFFFFFFF0000))
            );
        }

        /// <summary>
        /// Truncates a number, dropping the fractional part.
        /// </summary>
        /// <param name="x">The number to truncate</param>
        /// <returns>Returns x without the fractional part</returns>
        public static Accum Truncate (Accum x) {
            return new Accum (x.Value & 0xFFFF0000);
        }

        /// <summary>
        /// Rounds a number to the nearest integer.
        /// </summary>
        /// <param name="x">The number to round.</param>
        /// <returns>Returns x rounded to the nearest integer.</returns>
        public static Accum Round (Accum x) {
            return new Accum ((x.Value + (Accum.FracUnit / 2)) & 0xFFFF0000);
        }
        #endregion

        #region Clamping and wrapping
        // Clamping
        /// <summary>
        /// Clamps a number between a minimum and a maximum.
        /// </summary>
        /// <param name="n">The number to clamp.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>min, if n is lower than min; max, if n is higher than max; n otherwise.</returns>
        public static Accum Clamp (Accum n, Accum min, Accum max) {
            return Accum.MakeAccum (Math.Max (Math.Min (n.Value, max.Value), min.Value));
        }

        /// <summary>
        /// Clamps a number between a minimum and a maximum.
        /// </summary>
        /// <param name="n">The number to clamp.</param>
        /// <param name="min">The minimum allowed value.</param>
        /// <param name="max">The maximum allowed value.</param>
        /// <returns>min, if n is lower than min; max, if n is higher than max; n otherwise.</returns>
        public static Accum ClampInt (Accum n, int min, int max) {
            return Accum.MakeAccum (Math.Max (Math.Min (n.Value, max << 16), min << 16));
        }
        #endregion

        #region Conversions
        private static readonly Accum radToDegAccum = new Accum (180) / new Accum (PI);
        private static readonly Accum degToRadAccum = new Accum (PI) / new Accum (180);

        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        /// <param name="degrees">An angle in degrees.</param>
        /// <returns>The angle expressed in radians.</returns>
        public static Accum DegreesToRadians (Accum degrees) {
            return degrees * degToRadAccum;
        }

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        /// <param name="radians">An angle in radians.</param>
        /// <returns>The angle expressed in degrees.</returns>
        public static Accum RadiansToDegrees (Accum radians) {
            return radians * radToDegAccum;
        }
        #endregion
    }
}
