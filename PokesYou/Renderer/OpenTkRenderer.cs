using OpenTK;
using System;
using System.Drawing;

namespace PokesYou.Renderer.OpenTk {
    public class OpenTkRenderer : GameWindow, IRenderEngine {
        public new int Width  { get { return this.Size.Width;  } }
        public new int Height { get { return this.Size.Height; } }

        public void Render (long ticDelta) {

        }

        public void ChangeResolution (int width, int height) {
            this.Size = new Size (width, height);
        }

        public void FullscreenToggle (bool fullscreen) {
            this.FullscreenToggle (fullscreen);
        }
    }
}
