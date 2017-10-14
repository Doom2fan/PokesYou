using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.Data.Managers {
    public static class LumpManager {
        private static List<ILumpContainer> containers;

        static LumpManager () {
            containers = new List<ILumpContainer> ();
        }

        public static List<ILumpContainer> Containers { get { return containers; } }

        /// <summary>
        /// Removes all the containers from the list.
        /// </summary>
        public static void ClearList () { containers.Clear (); }

        /// <summary>
        /// Adds a container to the container list.
        /// </summary>
        /// <param name="container">The container to add.</param>
        /// <returns>True if the container was added successfully. False if the container couldn't be added -or- the container is already in the list.</returns>
        /// <exception cref="ArgumentNullException">Thrown if container is null.</exception>
        public static bool AddContainer (ILumpContainer container) {
            if (container == null)
                throw new ArgumentNullException ("container");

            if (containers.Contains (container))
                return false;

            containers.Add (container);
            return true;
        }

        /// <summary>
        /// Checks if a lump exists.
        /// </summary>
        /// <param name="file">The lump's name.</param>
        /// <returns>A bool indicating whether the lump exists.</returns>
        public static bool LumpExists (string file) {
            for (int i = containers.Count - 1; i >= 0; i--)
                if (containers [i].LumpExists (file))
                    return true;

            return false;
        }
        /// <summary>
        /// Checks if a lump with the specified path exists.
        /// </summary>
        /// <param name="file">The lump's path.</param>
        /// <returns>A bool indicating whether the lump exists.</returns>
        public static bool LumpExistsFullPath (string filePath) {
            for (int i = containers.Count - 1; i >= 0; i--)
                if (containers [i].LumpExistsFullPath (filePath))
                    return true;

            return false;
        }

        /// <summary>
        /// Retrieves a lump with the specified name.
        /// </summary>
        /// <param name="filePath">The lump's name</param>
        /// <returns>A Lump with the lump's data -or- null if the specified file does not exist.</returns>
        public static Lump GetLump (string file) {
            Lump lump;
            for (int i = containers.Count - 1; i >= 0; i--) {
                lump = containers [i].GetLump (file);
                if (lump != null)
                    return lump;
            }

            return null;
        }
        /// <summary>
        /// Retrieves a lump with the specified full path.
        /// </summary>
        /// <param name="filePath">The path to the lump</param>
        /// <returns>A Lump -or- null if the specified path does not exist or is not a lump.</returns>
        public static Lump GetLumpFullPath (string filePath) {
            Lump lump;
            for (int i = containers.Count - 1; i >= 0; i--) {
                lump = containers [i].GetLumpFullPath (filePath);
                if (lump != null)
                    return lump;
            }

            return null;
        }
    }
}
