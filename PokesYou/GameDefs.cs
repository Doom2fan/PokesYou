using PokesYou.CMath;

namespace PokesYou {
    public static class Constants {
        /// <summary>
        /// The directory the engine is running from.
        /// </summary>
        public static readonly string ProgDir = System.IO.Path.GetDirectoryName (System.Reflection.Assembly.GetExecutingAssembly ().Location);
        /// <summary>
        /// The game's ticrate.
        /// </summary>
        public const uint TicRate = 35;
        /// <summary>
        /// The amount of seconds per tic.
        /// </summary>
        public const double SecsPerTic = 1 / TicRate;
        /// <summary>
        /// The amount of milliseconds per tic.
        /// </summary>
        public const double MsecsPerTic = 1000 / TicRate;
        /// <summary>
        /// The base gravity amount.
        /// </summary>
        public static readonly Accum BaseGravity = Accum.MakeAccum (0x0004E8F6); // 4.91
        /// <summary>
        /// The maximum coordinates in the XYZ axes. Anything that tries to go past this distance disappears.
        /// </summary>
        public static readonly Accum CoordinatesMax = new Accum ( 32768);
        /// <summary>
        /// The minimum coordinates in the XYZ axes. Anything that tries to go past this distance disappears.
        /// </summary>
        public static readonly Accum CoordinatesMin = new Accum (-32768);
    }

    public static class GameState {
        public static int GameTic = 0;
        public static bool IsFocused = true;
    }
}
