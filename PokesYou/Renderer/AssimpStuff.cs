using Assimp;
using PokesYou.Data;
using PokesYou.Data.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PokesYou.Renderer {
    class LumpIOSystem : IOSystem {
        public override IOStream OpenFile (String pathToFile, FileIOMode fileMode) {
            return new LumpIOStream (this, pathToFile, fileMode);
        }
    }

    internal class LumpIOStream : IOStream {
        private LumpIOSystem parent;
        private Lump lmp;
        private Stream stream;

        public override bool IsValid { get { return stream != null; } }

        public LumpIOStream (LumpIOSystem parent, String pathToFile, FileIOMode fileMode)
            : base (pathToFile, fileMode) {
            this.parent = parent;

            switch (fileMode) {
                case FileIOMode.Read:
                case FileIOMode.ReadBinary:
                case FileIOMode.ReadText:
                    OpenRead (pathToFile, fileMode);
                    break;
                case FileIOMode.Write:
                case FileIOMode.WriteBinary:
                case FileIOMode.WriteText:
                    throw new NotImplementedException ();
            }
        }

        public override long Write (byte [] dataToWrite, long count) { throw new NotImplementedException (); } // Nope.

        public override long Read (byte [] dataRead, long count) {
            if (dataRead == null)
                throw new ArgumentOutOfRangeException ("dataRead", "Array to store data in cannot be null.");

            if (count < 0 || dataRead.Length < count)
                throw new ArgumentOutOfRangeException ("count", "Number of bytes to read is greater than data store size.");

            if (stream == null || !stream.CanRead)
                throw new IOException ("Stream is not readable.");

            stream.Read (dataRead, (int) stream.Position, (int) count);

            return count;
        }

        public override ReturnCode Seek (long offset, Origin seekOrigin) {
            if (stream == null || !stream.CanSeek)
                throw new IOException ("Stream does not support seeking.");

            SeekOrigin orig = SeekOrigin.Begin;
            switch (seekOrigin) {
                case Origin.Set:
                    orig = SeekOrigin.Begin;
                    break;
                case Origin.Current:
                    orig = SeekOrigin.Current;
                    break;
                case Origin.End:
                    orig = SeekOrigin.End;
                    break;
            }

            stream.Seek (offset, orig);

            return ReturnCode.Success;

        }

        public override long GetPosition () {
            if (stream == null)
                return -1;

            return stream.Position;
        }

        public override long GetFileSize () {
            if (stream == null)
                return 0;

            return stream.Length;
        }

        public override void Flush () { throw new NotImplementedException (); } // Nope.

        protected override void Dispose (bool disposing) {
            if (!IsDisposed && disposing) {
                if (stream != null)
                    stream.Close ();

                stream = null;
                lmp = null;

                base.Dispose (disposing);
            }
        }

        private void OpenRead (String pathToFile, FileIOMode fileMode) {
            String fileName = Path.GetFileName (pathToFile);

            if (LumpManager.LumpExistsFullPath (pathToFile)) {
                lmp = LumpManager.GetLumpFullPath (pathToFile);
                stream = lmp.AsStream;
            }
        }
    }
}
