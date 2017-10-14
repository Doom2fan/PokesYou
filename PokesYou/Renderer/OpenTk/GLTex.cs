using OpenTK.Graphics.OpenGL;
using PokesYou.Data;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace PokesYou.Renderer.OpenTk {
    public static class GLTexManager {
        private static Dictionary<Texture, GLTex> textures = new Dictionary<Texture, GLTex> (15000);

        public static GLTex GetOrAddTex (Texture tex) {
            GLTex gltex;
            if (textures.TryGetValue (tex, out gltex))
                return gltex;
            else {
                 gltex = GLTex.CreateGLTex (tex);
                textures.Add (tex, gltex);
                return gltex;
            }
        }

        public static GLTex AddTex (Texture tex) {
            GLTex gltex = GLTex.CreateGLTex (tex);
            textures.Add (tex, gltex);
            return gltex;
        }
    }

    public struct GLTex {
        public Texture Data { get; private set; }
        public int Id { get; private set; }

        private GLTex (Texture tex, int idx) {
            Data = tex;
            Id = idx;
        }

        /// <summary>
        /// Creates a GL texture from a Texture object.
        /// Do not use this directly. Use GLTexManager.GetOrAddTex instead.
        /// </summary>
        /// <param name="tex">The texture to use</param>
        /// <returns>A GLTex object</returns>
        public static GLTex CreateGLTex (Texture tex) {
            int idx;
            GL.GenTextures (1, out idx);
            GL.BindTexture (TextureTarget.Texture2D, idx);

            var bmp = tex.Bitmap;
            BitmapData bmpData = bmp.LockBits (new Rectangle (0, 0, tex.Width, tex.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D (TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmpData.Width, bmpData.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmpData.Scan0);
            bmp.UnlockBits (bmpData);
            bmpData = null;

            GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) TextureMinFilter.NearestMipmapLinear);
            GL.TexParameter (TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) TextureMagFilter.Nearest);
            GL.GenerateMipmap (GenerateMipmapTarget.Texture2D);

            return new GLTex (tex, idx);
        }
    }
}
