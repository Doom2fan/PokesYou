using OpenTK;
using OpenTK.Graphics;
using PokesYou.CMath;
using PokesYou.Data;
using PokesYou.Data.Managers;
using PokesYou.G_Console;
using PokesYou.Game;
using PokesYou.Renderer;
using PokesYou.Renderer.OpenTk;
using System;
using System.Diagnostics;
using System.IO;

namespace PokesYou {
    public static class Core {
        private static Stopwatch ticClock;
        private static Stopwatch renderClock;
        private static Stopwatch fpsClock;
        private static bool closing = false;
        private static CmdLineOpts options;
        public static ITicker Ticker { get; private set; }
        public static IRenderEngine RenderEngine { get; private set; }

        static Core () {
            GConsole.Debug.WriteLine ("Core: Constructing PokesYou.Core");
            ticClock = new Stopwatch ();
            renderClock = new Stopwatch ();
            fpsClock = new Stopwatch ();
            options = new CmdLineOpts ();
        }

        public static void Exit () { closing = true; }

        public static double GetTicTimeFrac () { return MathHelper.Clamp (ticClock.ElapsedMilliseconds / Constants.MsecsPerTic, 0d, 1d); }
        public static Accum GetTicTimeFracAccum () { return FixedMath.Clamp (new Accum (ticClock.ElapsedMilliseconds) / new Accum (Constants.MsecsPerTic), Accum.Zero, Accum.One); }

        /// <summary>
        /// Thrown when execution cannot continue.
        /// </summary>
        public class FatalError : Exception {
            public FatalError (string message) : base (message) { }
            public FatalError (string message, Exception innerException) : base (message, innerException) { }
        }
        /// <summary>
        /// Thrown when the game crashes.
        /// </summary>
        public class VeryFatalError : Exception {
            public VeryFatalError (string message) : base (message) { }
            public VeryFatalError (string message, Exception innerException) : base (message, innerException) { }
        }

        private static void InitPatches () {
            // Don't need to try running the loop if the list is null or there aren't any files in it.
            if (options.PatchFiles == null || options.PatchFiles.Count < 1)
                return;

            for (int i = 0; i < options.PatchFiles.Count; i++) {
                ILumpContainer container = null;
                string file = options.PatchFiles [i];

                if (File.Exists (file)) {
                    if (ZipLumpContainer.CheckContainer (file))
                        container = new ZipLumpContainer (file);
                }

                if (container != null) {
                    GConsole.WriteLine (" Adding \"{0}\", {1} lumps", file, container.Count);
                    LumpManager.AddContainer (container);
                } else
                    throw new FatalError (string.Format ("Could not identify patch {0}. Stopping execution.", file));
            }
        }

        private static void DisposeResources () {
            if (RenderEngine != null)
                RenderEngine.Dispose ();

            // Just to be sure.
            ticClock.Reset (); renderClock.Reset (); fpsClock.Reset ();
        }

        public static void Run (string [] args) {
            long ticDelta = 0, renderDelta = 0;
            double fpsTime;

            if (closing)
                return;
            
            if (!CommandLine.Parser.Default.ParseArguments (args, options)) {
                GConsole.WriteLine ("Fatal error: There was an error reading the argument list");
                Console.Read ();
                return;
            }

            try {
                /*Data.UDMF.UDMFParser udmfParser = new Data.UDMF.UDMFParser ();
                udmfParser.Setup ();

                using (var reader = new StreamReader ("TEXTMAP.txt"))
                    udmfParser.Parse (reader);

                Console.ReadKey ();
                return;*/

                GConsole.WriteLine ("Core: Loading engine.PK3");
                ZipLumpContainer container = null;
                string engPK3File = Path.GetFullPath (Path.Combine (Constants.ProgDir, @"engine.pk3"));

                if (File.Exists (engPK3File)) {
                    if (ZipLumpContainer.CheckContainer (engPK3File))
                        container = new ZipLumpContainer (engPK3File);
                }

                if (container == null)
                    throw new FatalError (String.Format ("Could not load engine.PK3. Stopping execution."));

                GConsole.WriteLine (" engine.PK3, {0} lumps", container.Count);
                LumpManager.AddContainer (container);

                GConsole.WriteLine ("Core: Initializing patches");
                InitPatches ();

                GConsole.WriteLine ("Core: Generating fixed-point Sin/Cos/Atan LUTs");
                FixedMath.GenerateTables ();

                GConsole.WriteLine ("Core: Initializing video");
                RenderEngine = OpenTkRenderer.Default;
                RenderEngine.Initialize (800, 600, false);
                RenderEngine.OnFocusChange += Renderer_OnFocusChange;

                GConsole.WriteLine ("Core: Initializing playsim");
                Ticker = new Ticker ();
                Ticker.Initialize ();

                GConsole.WriteLine ("Core: Starting game loop");
                ticClock.Reset ();
                ticClock.Start ();
                renderClock.Reset ();
                renderClock.Start ();

                while (true) {
                    if (closing)
                        return;

                    if (ticClock.ElapsedMilliseconds >= Constants.MsecsPerTic) {
                        ticDelta = ticClock.ElapsedMilliseconds;
                        ticClock.Restart ();
                        Ticker.Update (ticDelta);
                        Input.ClearTotals ();
                    }
                    Input.UpdateInput ();

                    renderDelta = renderClock.ElapsedMilliseconds;
                    renderClock.Restart ();

                    fpsClock.Restart ();
                    RenderEngine.Render (renderDelta);
                    fpsTime = fpsClock.ElapsedMilliseconds;

                    if (RenderEngine.IsUsable ())
                        RenderEngine.WindowTitle = String.Format ("Frame time: {0} ms", fpsTime);
                }
            } catch (FatalError err) {
                DisposeResources ();
                GConsole.WriteLine (err.Message);
                Console.ReadKey ();
            } catch (VeryFatalError err) {
                DisposeResources ();
                GConsole.WriteLine (err.Message);
                Console.ReadKey ();
            }
        }

        private static void Renderer_OnFocusChange (object sender, bool lost) {
            GameState.IsFocused = !lost;
        }
    }
}
