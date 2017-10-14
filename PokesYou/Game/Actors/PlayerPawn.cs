namespace PokesYou.Game.Actors {
    public class PlayerPawn : Actor {
        protected PlayerPawn () : base () { Player = null; }
        public PlayerPawn (ActorState state) : base (state) { Player = null; }

        /// <summary>
        /// Gets or sets the player this PlayerPawn belongs to. Will return null if no player owns it.
        /// </summary>
        public Player Player { get; set; }
    }
}
