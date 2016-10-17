using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.Game {
    public interface ITicker {
        void Update (long ticDelta);
    }

    public class Ticker : ITicker {
        public void Update (long ticDelta) {

        }
    }
}
