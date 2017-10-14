using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.CMath {
    public static class VecUtils {
        /// <summary>
        /// Converts correct XYZ coordinates to OpenGL's stupid format
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3 GLVec3 (float x, float y, float z) {
            return new Vector3 (x, z, -y);
        }
        /// <summary>
        /// Converts correct XYZ coordinates to OpenGL's stupid format
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Vector3d GLVec3d (double x, double y, double z) {
            return new Vector3d (x, z, -y);
        }

        #region Vector3k
        /// <summary>
        /// Converts correct XYZ coordinates to OpenGL's stupid format
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 ToGLVec3 (this Vector3k vec) {
            return new Vector3 ((float) vec.X, (float) vec.Z, (float) -vec.Y);
        }
        /// <summary>
        /// Converts correct XYZ coordinates to OpenGL's stupid format
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3d ToGLVec3d (this Vector3k vec) {
            return new Vector3d ((double) vec.X, (double) vec.Z, (double) -vec.Y);
        }

        /// <summary>
        /// Converts a Vector3k to OpenTK's Vector3 type
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 ToVec3 (this Vector3k vec) {
            return new Vector3 ((float) vec.X, (float) vec.Y, (float) vec.Z);
        }
        /// <summary>
        /// Converts a Vector3k to OpenTK's Vector3d type
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3d ToVec3d (this Vector3k vec) {
            return new Vector3d ((double) vec.X, (double) vec.Y, (double) vec.Z);
        }
        #endregion

        #region Vector3
        /// <summary>
        /// Converts correct XYZ coordinates to OpenGL's stupid format
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 ToGLVec3 (this Vector3 vec) {
            return new Vector3 (vec.X, vec.Z, -vec.Y);
        }
        /// <summary>
        /// Converts correct XYZ coordinates to OpenGL's stupid format
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3d ToGLVec3d (this Vector3 vec) {
            return new Vector3d (vec.X, vec.Z, -vec.Y);
        }

        /// <summary>
        /// Converts a Vector3 to a Vector3k type
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3k ToVec3k (this Vector3 vec) {
            return new Vector3k (new Accum (vec.X), new Accum (vec.Y), new Accum (vec.Z));
        }
        #endregion

        #region Vector3d
        /// <summary>
        /// Converts correct XYZ coordinates to OpenGL's stupid format
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3 ToGLVec3 (this Vector3d vec) {
            return new Vector3 ((float) vec.X, (float) vec.Z, (float) -vec.Y);
        }
        /// <summary>
        /// Converts correct XYZ coordinates to OpenGL's stupid format
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3d ToGLVec3d (this Vector3d vec) {
            return new Vector3d (vec.X, vec.Z, -vec.Y);
        }

        /// <summary>
        /// Converts a Vector3d to a Vector3k type
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        public static Vector3k ToVec3k (this Vector3d vec) {
            return new Vector3k (new Accum (vec.X), new Accum (vec.Y), new Accum (vec.Z));
        }
        #endregion
    }
}
