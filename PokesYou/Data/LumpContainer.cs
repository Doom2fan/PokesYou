namespace PokesYou.Data {
    /// <summary>
    /// Defines a lump container
    /// </summary>
    public interface ILumpContainer {
        /// <summary>
        /// Gets the amount of lumps in the container.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets a list of all lumps in the container.
        /// </summary>
        /// <returns>A string array containing the full paths to all the lumps in the container.</returns>
        string [] GetLumps ();

        /// <summary>
        /// Checks if a lump exists.
        /// </summary>
        /// <param name="file">The lump's name.</param>
        /// <returns>A bool indicating whether the lump exists.</returns>
        bool LumpExists (string file);
        /// <summary>
        /// Checks if a lump with the specified path exists.
        /// </summary>
        /// <param name="file">The lump's path.</param>
        /// <returns>A bool indicating whether the lump exists.</returns>
        bool LumpExistsFullPath (string filePath);

        /// <summary>
        /// Retrieves a lump with the specified name.
        /// </summary>
        /// <param name="filePath">The lump's name</param>
        /// <returns>A Lump with the lump's data -or- null if the specified file does not exist.</returns>
        Lump GetLump (string file);
        /// <summary>
        /// Retrieves a lump with the specified full path.
        /// </summary>
        /// <param name="filePath">The path to the lump</param>
        /// <returns>A Lump -or- null if the specified path does not exist or is not a lump.</returns>
        Lump GetLumpFullPath (string filePath);
    }
}
