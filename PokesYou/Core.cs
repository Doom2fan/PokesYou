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
        private static ITicker game;
        private static IRenderEngine renderer;
        private static Stopwatch ticClock;
        private static Stopwatch renderClock;
        private static Stopwatch fpsClock;
        private static bool Closing = false;
        private static CmdLineOpts options;
        public static ITicker Ticker { get { return game; } }
        public static IRenderEngine RenderEngine { get { return renderer; } }

        static Core () {
            GConsole.Debug.WriteLine ("Core: Constructing PokesYou.Core");
            ticClock = new Stopwatch ();
            renderClock = new Stopwatch ();
            fpsClock = new Stopwatch ();
            options = new CmdLineOpts ();
        }

        public static void Exit () { Closing = true; }

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
                    throw new FatalError (String.Format ("Could not identify patch {0}. Stopping execution.", file));
            }
        }

        private static void DisposeResources () {
            if (renderer != null)
                renderer.Dispose ();

            // Just to be sure.
            ticClock.Reset (); renderClock.Reset (); fpsClock.Reset ();
        }

        public static void Run (string [] args) {
            long ticDelta = 0, renderDelta = 0,
                lastFPSTime = 0, fpsTime = 0,
                fpsCount = 0, curFpsCount = 0;

            if (Closing)
                return;
            
            if (!CommandLine.Parser.Default.ParseArguments (args, options)) {
                GConsole.WriteLine ("Fatal error: There was an error reading the argument list");
                Console.Read ();
                return;
            }

            try {
                GConsole.WriteLine ("Core: Initializing patches");
                InitPatches ();

                GConsole.WriteLine ("Core: Generating fixed-point Sin/Cos/Atan LUTs");
                FixedMath.GenerateTables ();

                GConsole.WriteLine ("Core: Initializing video");
                renderer = new OpenTkRenderer (800, 600, GraphicsMode.Default, "PokesYou", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.Default);
                renderer.Initialize (800, 600, false);

                GConsole.WriteLine ("Core: Initializing ticker");
                game = new Ticker ();
                game.Initialize ();

                GConsole.WriteLine ("Core: Starting game loop");
                ticClock.Reset ();
                ticClock.Start ();
                renderClock.Reset ();
                renderClock.Start ();
                fpsClock.Start ();

                while (true) {
                    if (Closing)
                        return;
                    if (ticClock.ElapsedMilliseconds >= Constants.MsecsPerTic) {
                        ticDelta = ticClock.ElapsedMilliseconds;
                        ticClock.Restart ();
                        game.Update (ticDelta);
                        Input.ClearTotals ();
                    }
                    if (fpsClock.ElapsedMilliseconds >= 1000) {
                        curFpsCount = fpsCount;
                        fpsCount = 0;
                        lastFPSTime = fpsTime = 0;
                        fpsClock.Restart ();
                    }
                    if (Input.IsMouseGrabbed != true)
                        Input.GrabMouse ();
                    Input.UpdateInput ();

                    renderDelta = renderClock.ElapsedMilliseconds;
                    renderClock.Restart ();

                    lastFPSTime = fpsTime;
                    fpsTime = fpsClock.ElapsedMilliseconds;
                    renderer.Render (renderDelta);
                    fpsCount++;

                    //renderer.WindowTitle = String.Format ("FPS: {0} ms ({1})", fpsTime - lastFPSTime, curFpsCount);
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
    }
}
