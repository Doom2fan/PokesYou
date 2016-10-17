using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.Renderer {
    public interface IRenderEngine {
        void Render (long ticDelta);
        void ChangeResolution (int width, int height);
        void FullscreenToggle (bool fullscreen);
    }
}
