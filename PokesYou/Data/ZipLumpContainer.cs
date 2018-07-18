using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

namespace PokesYou.Data {
    public sealed class ZipLumpContainer : ILumpContainer, IDisposable {
        private ZipFile zip;

        /// <summary>
        /// Checks if a specified path is a zip.
        /// </summary>
        /// <param name="path">The path to be checked.</param>
        /// <returns>True if the path is a zip. False if not.</returns>
        public static bool CheckContainer (string path) {
            if (!File.Exists (path))
                return false;

            using (var stream = File.Open (path, FileMode.Open, FileAccess.Read)) {
                try {
                    var zip = new ZipFile (stream);
                    zip.Close ();
                } catch (ZipException) {
                    return false;
                }
            }

            return true;
        }

        private ZipLumpContainer () { }

        /// <summary>
        /// Creates a new LumpContainer for a zip file from the specified file.
        /// </summary>
        /// <param name="path">The file to load.</param>
        public ZipLumpContainer (string path) {
            if (path == null)
                throw new ArgumentNullException ("path");
            if (String.IsNullOrWhiteSpace (path))
                throw new ArgumentException ("Path cannot be empty or whitespace.", "path");
            if (!File.Exists (path))
                throw new ArgumentException ("Path does not point to a valid file.");

            try {
                zip = new ZipFile (path);
            } catch (ZipException ex) {
                throw new ArgumentException ("Path is not a valid ZIP file.", "path", ex);
            }
        }
        /// <summary>
        /// Creates a new LumpContainer for a zip file from a stream.
        /// </summary>
        /// <param name="path">The file to load.</param>
        public ZipLumpContainer (Stream stream) {
            if (stream == null)
                throw new ArgumentNullException ("stream");

            stream.Seek (0, SeekOrigin.Begin);

            try {
                zip = new ZipFile (stream);
            } catch (ZipException ex) {
                throw new ArgumentException ("Path is not a valid ZIP file.", "path", ex);
            }
        }

        #region ILumpContainer
        /// <summary>
        /// Gets the amount of files in the zip.
        /// </summary>
        public int Count {
            get { return (int) zip.Count; }
        }

        /// <summary>
        /// Gets a list of all files in the zip.
        /// </summary>
        /// <returns>A string array containing the full paths to all the files in the zip.</returns>
        public string [] GetLumps () {
            string [] files = new string [zip.Count];

            for (int i = 0; i < zip.Count; i++)
                files [i] = zip [i].Name;

            return files;
        }

        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="file">The file's name.</param>
        /// <returns>A bool indicating whether the file exists.</returns>
        public bool LumpExists (string file) {
            bool hasExt = Path.HasExtension (file);
            for (int i = 0; i < zip.Count; i++) {
                if (string.Compare (file, (hasExt ? Path.GetFileName (zip [i].Name) : Path.GetFileNameWithoutExtension (zip [i].Name)), StringComparison.OrdinalIgnoreCase) == 0 && zip [i].IsFile)
                    return true;
            }

            return false;
        }
        /// <summary>
        /// Checks if a file with the specified path exists.
        /// </summary>
        /// <param name="file">The file's path.</param>
        /// <returns>A bool indicating whether the file exists.</returns>
        public bool LumpExistsFullPath (string file) {
            for (int i = 0; i < zip.Count; i++)
                if (string.Compare (file, zip [i].Name, StringComparison.OrdinalIgnoreCase) == 0 && zip [i].IsFile)
                    return true;

            return false;
        }

        /// <summary>
        /// Retrieves a lump with the specified name.
        /// </summary>
        /// <param name="filePath">The file's name</param>
        /// <returns>A Lump with the file's data -or- null if the specified file does not exist.</returns>
        public Lump GetLump (string file) {
            bool hasExt = Path.HasExtension (file);

            for (int i = 0; i < zip.Count; i++) {
                if (string.Compare (file, (hasExt ? Path.GetFileName (zip [i].Name) : Path.GetFileNameWithoutExtension (zip [i].Name)), StringComparison.OrdinalIgnoreCase) == 0 && zip [i].IsFile) {
                    byte [] buffer = new byte [zip [i].Size];

                    using (var stream = zip.GetInputStream (zip [i]))
                        StreamUtils.ReadFully (stream, buffer);

                    return new Lump (zip [i].Name.Replace ('\\', '/'), buffer);
                }
            }

            return null;
        }
        /// <summary>
        /// Retrieves a lump with the specified full path.
        /// </summary>
        /// <param name="filePath">The path to the file</param>
        /// <returns>A Lump with the file's data -or- null if the specified path does not exist or is not a file.</returns>
        public Lump GetLumpFullPath (string file) {
            file = file.Replace ('\\', '/');
            for (int i = 0; i < zip.Count; i++) {
                if (string.Compare (file, zip [i].Name.Replace ('\\', '/'), StringComparison.OrdinalIgnoreCase) == 0 && zip [i].IsFile) {
                    byte [] buffer = new byte [zip [i].Size];

                    using (var stream = zip.GetInputStream (zip [i]))
                        StreamUtils.ReadFully (stream, buffer);

                    return new Lump (zip [i].Name.Replace ('\\', '/'), buffer);
                }
            }

            return null;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false;

        void Dispose (bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    zip.Close ();
                    zip = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose () {
            Dispose (true);
        }
        #endregion
    }
}
