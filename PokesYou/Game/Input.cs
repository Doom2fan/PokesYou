using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokesYou.Game {
    public static class Input {
        /// <summary>
        /// The local player's pressed keys (previous tic)
        /// </summary>
        static private Keys locKeysPrev = 0;
        /// <summary>
        /// The local player's pressed keys
        /// </summary>
        static private Keys locKeys = 0;
        /// <summary>
        /// The netcmd keys
        /// </summary>
        [Flags]
        public enum Keys : uint {
            Use         = 1,
            Attack      = 1 << 2,
            AltAttack   = 1 << 3,
            Reload      = 1 << 4,
            Jump        = 1 << 5,
        }

        public static bool IsPressed (Keys key) { return ((locKeysPrev & key) >  0) && ((locKeys & key) == 0); }
        public static bool IsDown    (Keys key) { return  (locKeys     & key) >  0; }
        public static bool IsUp      (Keys key) { return  (locKeys     & key) == 0; }
    }
}
