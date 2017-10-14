using System;

namespace PokesYou.Game {
    [Flags]
    public enum GameObjFlags : int {
        /// <summary>Remove the actor from anything it might be in, then delete it</summary>
        EuthanizeMe = 1,
    }

    /// <summary>
    /// Base game object class. Everything inherits from it.
    /// </summary>
    public class GameObj {
        public GameObjFlags ObjFlags { get; set; }
    }
}
