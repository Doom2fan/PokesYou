using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PokesYou.Support {
    /// <summary>
    /// Represents a mouse cursor
    /// </summary>
    public class CursorIcon {
        #region Variables
        protected static readonly CursorIcon def = null;
        protected static readonly CursorIcon empty = new CursorIcon (0, 0, 16, 16, new byte [16 * 16 * 4]);

        protected byte [] imgData;
        protected int width, height;
        protected int x, y;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a null instance
        /// </summary>
        internal protected CursorIcon () {
            imgData = null;
            width = height = x = y = -1;
        }

        /// <summary>
        /// Creates a new instance from a byte array
        /// </summary>
        /// <param name="hotX">The cursor's hotspot's X position</param>
        /// <param name="hotY">The cursor's hotspot's Y position</param>
        /// <param name="w">The cursor's width</param>
        /// <param name="h">The cursor's height</param>
        /// <param name="data">The cursor's bytes (BGRA)</param>
        public CursorIcon (int hotX, int hotY, int w, int h, byte [] data) {
            if (data.Length != w * h * 4)
                throw new ArgumentException ();

            x = hotX; y = hotY;
            width = w; height = h;
            imgData = data;
        }
        /// <summary>
        /// Creates a new instance from a <see cref="Bitmap"/>
        /// </summary>
        /// <param name="hotX">The cursor's hotspot's X position</param>
        /// <param name="hotY">The cursor's hotspot's Y position</param>
        /// <param name="image">The cursor's image data</param>
        public CursorIcon (int hotX, int hotY, Bitmap image) :
            this (hotX, hotY, image.Width, image.Height, null) {
            var bytes = image.LockBits (
                    new Rectangle (0, 0, image.Width, image.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            
            imgData = new byte [image.Width * image.Height * 4];
            Marshal.Copy (bytes.Scan0, imgData, 0, imgData.Length);
        }
        #endregion

        #region Properties
        public static CursorIcon Default { get { return def; } }
        public static CursorIcon Empty { get { return empty; } }

        internal byte [] Data { get { return imgData; } }
        internal int Width { get { return width; } }
        internal int Height { get { return height; } }
        internal int X { get { return x; } }
        internal int Y { get { return y; } }
        #endregion
    }
}
