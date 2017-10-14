using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace PokesYou.Data.Managers {
    public static class TextureManager {
        private static Dictionary<string, Texture> textures;

        static TextureManager () {
            textures = new Dictionary<string, Texture> ();
        }

        /// <summary>
        /// Loads a texture from a lump.
        /// </summary>
        /// <param name="srcPath">The lump to load.</param>
        /// <returns>The loaded texture.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="srcPath"/> is null, empty or whitespace.</exception>
        public static Texture LoadTexture (string srcPath, bool fullPath = true) {
            Lump texData;

            if (fullPath)
                texData = LumpManager.GetLumpFullPath (srcPath);
            else
                texData = LumpManager.GetLump (srcPath);

            using (var bmp = new Bitmap (texData.AsStream))
                return Texture.CreateTexture (bmp);
        }

        /// <summary>
        /// Loads a texture from an Image.
        /// </summary>
        /// <param name="srcPath">The source file/lump of the texture.</param>
        /// <param name="src">The Image to load.</param>
        /// <returns>The loaded texture.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="srcPath"/> is null, empty or whitespace -or- <paramref name="src"/> is null.</exception>
        public static Texture LoadTexture (string srcPath, Image src) {
            if (String.IsNullOrWhiteSpace (srcPath))
                throw new ArgumentNullException ("The texture's source path cannot be null, empty or whitespace.");
            if (src == null)
                throw new ArgumentNullException ("The specified Image is null.");

            Texture tex;
            using (Bitmap bmp = new Bitmap (src))
                tex = Texture.CreateTexture (bmp);

            return tex;
        }

        /// <summary>
        /// Loads a texture from a Bitmap.
        /// </summary>
        /// <param name="srcPath">The source file/lump of the texture.</param>
        /// <param name="src">The Bitmap to load.</param>
        /// <returns>The loaded texture.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="srcPath"/> is null, empty or whitespace -or- <paramref name="src"/> is null.</exception>
        public static Texture LoadTexture (string srcPath, Bitmap src) {
            if (String.IsNullOrWhiteSpace (srcPath))
                throw new ArgumentNullException ("The texture's source path cannot be null, empty or whitespace.");
            if (src == null)
                throw new ArgumentNullException ("The specified Bitmap is null.");

            return Texture.CreateTexture (src);
        }

        /// <summary>
        /// Loads a texture from a stream.
        /// </summary>
        /// <param name="srcPath">The source file/lump of the texture.</param>
        /// <param name="src">The stream to load.</param>
        /// <returns>The loaded texture.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="srcPath"/> is null, empty or whitespace -or- <paramref name="src"/> is null.</exception>
        public static Texture LoadTexture (string srcPath, Stream src) {
            if (String.IsNullOrWhiteSpace (srcPath))
                throw new ArgumentNullException ("The texture's source path cannot be null, empty or whitespace.");
            if (src == null)
                throw new ArgumentNullException ("The specified stream is null.");

            Texture tex;
            using (Bitmap bmp = new Bitmap (src))
                tex = Texture.CreateTexture (bmp);
            return tex;
        }
    }
}
