namespace PokesYou.Data {
    /// <summary>
    /// Defines a class that contains file data.
    /// </summary>
    public class Lump {
        public string Path { get; protected set; }
        public byte [] Data { get; protected set; }
        public System.IO.Stream AsStream { get { return new System.IO.MemoryStream (Data, false); } }

        protected Lump () { }

        /// <summary>
        /// Defines a new lump.
        /// </summary>
        /// <param name="data">The lump's data.</param>
        /// <param name="dispose">Whether the input stream should be closed and disposed.</param>
        public Lump (string path, System.IO.Stream data, bool dispose = false) {
            Path = path;
            Data = new byte [data.Length];
            if (data.Position != 0)
                data.Seek (0, System.IO.SeekOrigin.Begin);
            data.Read (Data, 0, (int) data.Length);

            if (dispose) {
                data.Close ();
                data.Dispose ();
            }

            data = null;
        }

        /// <summary>
        /// Defines a new lump.
        /// </summary>
        /// <param name="data">The lump's data.</param>
        public Lump (string path, byte [] data) {
            Path = path;
            Data = new byte [data.LongLength];
            data.CopyTo (Data, 0);

            data = null;
        }
    }
}
