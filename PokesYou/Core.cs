using PokesYou.Game;
using PokesYou.Renderer;
using PokesYou.Renderer.OpenTk;
using System;
using System.Diagnostics;

namespace PokesYou {
    public static class Core {
        private static ITicker game;
        private static IRenderEngine renderer;
        private static Stopwatch ticClock;
        private static Stopwatch renderClock;
        private static long ticDelta;
        private static long renderDelta;

        static Core () {
            game = new Ticker ();
            renderer = new OpenTkRenderer ();
            ticClock = new Stopwatch ();
            renderClock = new Stopwatch ();
        }

        public static void Run () {
            ticClock.Reset ();
            while (true) {
                if (ticClock.ElapsedMilliseconds >= Constants.MsecsPerTic) {
                    ticDelta = ticClock.ElapsedMilliseconds;
                    ticClock.Restart ();
                    game.Update (ticDelta);
                }
                renderDelta = renderClock.ElapsedMilliseconds;
                renderClock.Restart ();
                renderer.Render (renderDelta);
            }
        }
    }
}
