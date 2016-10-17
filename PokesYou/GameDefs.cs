namespace PokesYou {
    public static class Constants {
        /// <summary>
        /// The game's ticrate
        /// </summary>
        public const uint TicRate = 35;
        /// <summary>
        /// The amount of seconds per tic
        /// </summary>
        public const double SecsPerTic = 1 / TicRate;
        /// <summary>
        /// The amount of milliseconds per tic
        /// </summary>
        public const long MsecsPerTic = 1000 / TicRate;
    }
}
