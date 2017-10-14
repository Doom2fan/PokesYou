using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace PokesYou.Data {
    /// <summary>
    /// A 32bpp ARGB texture.
    /// </summary>
    public class Texture {
        protected byte [] data;
        protected Bitmap bmp;
        
        /// <summary>Gets a copy of the texture's data as a Bitmap.</summary>
        public Bitmap Bitmap { get { return bmp.Clone (new Rectangle (Point.Empty, bmp.Size), PixelFormat.Format32bppArgb); } }
        /// <summary>Gets a copy of the texture's data as a Stream.</summary>
        public Stream Bytes { get { return new MemoryStream (data, false); } }
        /// <summary>Gets a copy of the texture's data as a byte array.</summary>
        public byte [] BytesAsArray { get { return (byte []) data.Clone (); } }
        /// <summary>Gets the texture's width.</summary>
        public int Width { get; protected set; }
        /// <summary>Gets the texture's height.</summary>
        public int Height { get; protected set; }

        protected Texture () {}

        /// <summary>
        /// Creates a new Texture from a Bitmap.
        /// TextureManager.LoadTexture should be used instead.
        /// </summary>
        /// <param name="src">The Bitmap to use.</param>
        /// <returns>A new Texture instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="src"/> is null.</exception>
        public static Texture CreateTexture (Bitmap src) {
            if (src == null)
                throw new ArgumentNullException ("src");

            if (src.PixelFormat != PixelFormat.Format32bppArgb)
                src = src.Clone (new Rectangle (Point.Empty, src.Size), PixelFormat.Format32bppArgb);

            Texture tex = new Texture ();

            tex.Width = src.Width;
            tex.Height = src.Height;
            tex.bmp = src.Clone (new Rectangle (Point.Empty, src.Size), PixelFormat.Format32bppArgb);

            tex.data = new byte [(tex.Width * tex.Height) * 4];
            BitmapData bmpData = src.LockBits (new Rectangle (Point.Empty, src.Size), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy (bmpData.Scan0, tex.data, 0, (tex.Width * tex.Height) * 4);
            src.UnlockBits (bmpData);
            bmpData = null;

            return tex;
        }

        /// <summary>
        /// Creates a new Texture from a byte array.
        /// This function is provided just in case - you shouldn't really use this.
        /// </summary>
        /// <param name="src">The image's bytes.</param>
        /// <param name="width">The image's width.</param>
        /// <param name="height">The image's height.</param>
        /// <returns>A new Texture instance.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="src"/> is null.</exception>
        /// <exception cref="ArgumentException">The amount of bytes in <paramref name="src"/> is not equal to <code><paramref name="width"/> * <paramref name="height"/> * 4</code>.</exception>
        public static Texture CreateTexture (byte[] src, int width, int height) {
            if (src == null)
                throw new ArgumentNullException ("src");
            if (src.Length != (width * height * 4))
                throw new ArgumentException ("Src's byte count is not equal to width * height * 4");

            Texture tex = new Texture ();
            
            tex.Width = width;
            tex.Height = height;
            tex.data = src;
            tex.bmp = new Bitmap (width, height, PixelFormat.Format32bppArgb);
            
            BitmapData bmpData = tex.bmp.LockBits (new Rectangle (0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            Marshal.Copy (tex.data, 0, bmpData.Scan0, (width * height) * 4);
            tex.bmp.UnlockBits (bmpData);
            bmpData = null;

            return tex;
        }
    }
}
