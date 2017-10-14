using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.CMath {
    /// <summary>
    /// Defines a signed 16.16 fixed-polong value
    /// </summary>
    public struct Accum {
        private long longValue;

        public Accum (long val) { longValue = val << 16; }
        public Accum (float val) { longValue = (long) (val * FracUnit); }
        public Accum (double val) { longValue = (long) (val * FracUnit); }
        public Accum (Accum val) { longValue = val.longValue; }
        public static Accum MakeAccum (long val) {
            Accum k = new Accum ();
            k.longValue = val;
            return k;
        }

        #region Static properties
        public const long MinLong = unchecked((long) 0x8000000000000000);
        public const long MaxLong = 0x7FFFFFFFFFFFFFFF;
        public const long FracUnit = 1 << 16;
        public static readonly Accum Min  = MakeAccum (MinLong);
        public static readonly Accum Max  = MakeAccum (MaxLong);
        public static readonly Accum One  = MakeAccum (FracUnit);
        public static readonly Accum Zero = MakeAccum (0);
        #endregion

        #region Properties
        public long Value {
            get { return longValue; }
            set { longValue = value; }
        }
        #endregion

        public override string ToString () {
            return (((float) longValue) / FracUnit).ToString ();
        }

        #region Unary operators
        public static Accum operator -(Accum val) { return MakeAccum (-val.longValue); }
        public static Accum operator ~(Accum val) { return MakeAccum (~val.longValue); }
        #endregion

        #region Accum operators
        /// <summary>
        /// Adds the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of addition.</returns>
        public static Accum operator +(Accum lhs, Accum rhs) {
            return MakeAccum (lhs.longValue + rhs.longValue);
        }
        /// <summary>
        /// Subtracts the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of subtraction.</returns>
        public static Accum operator -(Accum lhs, Accum rhs) {
            return MakeAccum (lhs.longValue - rhs.longValue);
        }
        /// <summary>
        /// Multiplies the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of multiplication.</returns>
        public static Accum operator *(Accum lhs, Accum rhs) { // TODO: Change this to multiply the integer and fractional parts separately to reduce errors
            return MakeAccum ((long) (((long) lhs.longValue * rhs.longValue) >> 16));
        }
        /// <summary>
        /// Divides the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of division.</returns>
        public static Accum operator /(Accum lhs, Accum rhs) {
            return MakeAccum (SafeDivision (lhs.longValue, rhs.longValue));
        }

        /// <summary>
        /// Divides an accum. (Safe) (Meant for longernal use by FixedMath and extension functions!)
        /// </summary>
        public static long SafeDivision (long a, long b) {
            if ((ulong) (System.Math.Abs (a)) >> 16 >= (ulong) System.Math.Abs (b))
                return (a ^ b) < 0 ? Accum.MinLong : Accum.MaxLong;

            return (long) Division (a, b);
        }
        /// <summary>
        /// Divides an accum. (Safe) (Meant for longernal use by FixedMath and extension functions!)
        /// </summary>
        public static long Division (long a, long b) { return (long) (((long) a << 16) / b); }

        /// <summary>
        /// Bitwise XORs the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of XOR</returns>
        public static Accum operator ^(Accum lhs, Accum rhs) {
            return MakeAccum (lhs.longValue ^ rhs.longValue);
        }
        /// <summary>
        /// Bitwise ORs the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of OR</returns>
        public static Accum operator |(Accum lhs, Accum rhs) {
            return MakeAccum (lhs.longValue | rhs.longValue);
        }
        /// <summary>
        /// Bitwise ANDs the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of AND</returns>
        public static Accum operator &(Accum lhs, Accum rhs) {
            return MakeAccum (lhs.longValue & rhs.longValue);
        }

        public static bool operator ==(Accum lhs, Accum rhs) { return lhs.longValue == rhs.longValue; }
        public static bool operator !=(Accum lhs, Accum rhs) { return lhs.longValue != rhs.longValue; }
        public static bool operator >(Accum lhs, Accum rhs) { return lhs.longValue > rhs.longValue; }
        public static bool operator <(Accum lhs, Accum rhs) { return lhs.longValue < rhs.longValue; }
        public static bool operator >=(Accum lhs, Accum rhs) { return lhs.longValue >= rhs.longValue; }
        public static bool operator <=(Accum lhs, Accum rhs) { return lhs.longValue <= rhs.longValue; }

        #endregion

        #region Long operators
        public static bool operator ==(Accum lhs, long rhs) { return lhs.longValue == (rhs << 16); }
        public static bool operator !=(Accum lhs, long rhs) { return lhs.longValue != (rhs << 16); }
        public static bool operator >(Accum lhs, long rhs) { return lhs.longValue > (rhs << 16); }
        public static bool operator <(Accum lhs, long rhs) { return lhs.longValue < (rhs << 16); }
        public static bool operator >=(Accum lhs, long rhs) { return lhs.longValue >= (rhs << 16); }
        public static bool operator <=(Accum lhs, long rhs) { return lhs.longValue <= (rhs << 16); }

        public static bool operator ==(long lhs, Accum rhs) { return (lhs << 16) == rhs.longValue; }
        public static bool operator !=(long lhs, Accum rhs) { return (lhs << 16) != rhs.longValue; }
        public static bool operator >(long lhs, Accum rhs) { return (lhs << 16) > rhs.longValue; }
        public static bool operator <(long lhs, Accum rhs) { return (lhs << 16) < rhs.longValue; }
        public static bool operator >=(long lhs, Accum rhs) { return (lhs << 16) >= rhs.longValue; }
        public static bool operator <=(long lhs, Accum rhs) { return (lhs << 16) <= rhs.longValue; }
        
        public static Accum operator <<(Accum lhs, int rhs) { return MakeAccum (lhs.longValue << rhs); }
        public static Accum operator >>(Accum lhs, int rhs) { return MakeAccum (lhs.longValue >> rhs); }

        /*
        /// <summary>
        /// Adds the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of addition.</returns>
        public static Accum operator +(Accum lhs, long rhs) {
            return MakeAccum (lhs.longValue + rhs << 16);
        }
        /// <summary>
        /// Adds the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of addition.</returns>
        public static long operator +(long lhs, Accum rhs) {
            return MakeAccum ((lhs << 16) + rhs.longValue);
        }

        /// <summary>
        /// Subtracts the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of subtraction.</returns>
        public static Accum operator -(Accum lhs, long rhs) {
            return MakeAccum (lhs.longValue - rhs << 16);
        }
        /// <summary>
        /// Subtracts the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of subtraction.</returns>
        public static long operator -(long lhs, Accum rhs) {
            return MakeAccum ((lhs << 16) - rhs.longValue);
        }

        /// <summary>
        /// Divides the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of division.</returns>
        public static Accum operator -(Accum lhs, long rhs) {
            ;
            return MakeAccum (lhs.longValue - rhs);
        }
        /// <summary>
        /// Divides the specified instances.
        /// </summary>
        /// <param name="lhs">Left operand.</param>
        /// <param name="rhs">Right operand.</param>
        /// <returns>Result of division.</returns>
        public static long operator -(long lhs, Accum rhs) {
            return MakeAccum (SafeDivision ((lhs << 16), rhs.longValue));
        }
        */

        #endregion

        #region Casts
        //public static implicit operator Accum (long val) { return new Accum (val); }
        //public static implicit operator Accum (float val) { return new Accum (val); }
        //public static implicit operator Accum (double val) { return new Accum (val); }
        public static explicit operator long (Accum val) { return val.longValue >> 16; }
        public static explicit operator float (Accum val) { return ((float) val.longValue) / FracUnit; }
        public static explicit operator double (Accum val) { return ((double) val.longValue) / FracUnit; }
        #endregion
    }
}
