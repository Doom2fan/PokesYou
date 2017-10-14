using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using PokesYou.Game;
using PokesYou.Renderer.OpenTk.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace PokesYou.Renderer.OpenTk {
    public class OpenTkRenderer : GameWindow, IRenderEngine {
        public static readonly OpenTkRenderer Default = new OpenTkRenderer (800, 600, GraphicsMode.Default, "PokesYou", GameWindowFlags.Default, DisplayDevice.Default, 3, 0, GraphicsContextFlags.Default);
        
        protected internal Camera localCamera { get; set; }

        protected bool IsInitialized = false;

        #region Properties
        public int WindowWidth { get { EnsureUsable (); return this.Size.Width; } }
        public int WindowHeight { get { EnsureUsable (); return this.Size.Height; } }
        public bool IsFullscreen {
            get { EnsureUsable (); return WindowState == WindowState.Fullscreen; }
            set { EnsureUsable (); WindowState = value ? WindowState.Fullscreen : 0; }
        }
        public bool WindowVSync {
            get { EnsureUsable (); if (VSync == VSyncMode.On || VSync == VSyncMode.Adaptive) { return true; } else { return false; } }
            set { EnsureUsable (); VSync = value ? VSyncMode.On : VSyncMode.Off; }
        }

        public string WindowTitle {
            get { EnsureUsable (); return this.Title; }
            set { EnsureUsable (); this.Title = value; }
        }
        #endregion

        #region Event handlers
        protected void OpenTkRenderer_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            if (!e.Cancel) {
                Dispose ();
                Core.Exit ();
            }
        }
        #endregion

        public OpenTkRenderer (int width, int height, GraphicsMode mode, string title, GameWindowFlags options, DisplayDevice device, int major, int minor, GraphicsContextFlags flags)
            : base (width, height, mode, title, options, device, major, minor, flags) {
            
        }

        #region Private functions
        protected void EnsureUsable () {
            if (!IsInitialized) throw new InvalidOperationException ("Initialize () must be called before using " + GetType ().Name);
            if (IsDisposed) throw new ObjectDisposedException (GetType ().Name);
        }
        #endregion

        #region Public functions
        public void Initialize (int width, int height, bool fullscreen) {
            EnsureUndisposed ();
            this.Size = new Size (width, height);
            this.WindowState = fullscreen ? WindowState.Fullscreen : 0;
            this.Visible = true;
            this.Closing += OpenTkRenderer_Closing;
            this.IsInitialized = true;
            this.VSync = VSyncMode.Off;
            
            OnLoad (EventArgs.Empty);
            OnResize (EventArgs.Empty);
        }

        protected override void OnLoad (EventArgs e) {
            base.OnLoad (e);

            this.MakeCurrent ();
            GL.ClearColor (Color.MidnightBlue);  // Set options
            GL.ClearDepth (1.0f);
            GL.DepthFunc (DepthFunction.Less);
            GL.BlendFunc (BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.AlphaFunc (AlphaFunction.Gequal, 0.5f);

            GL.Enable (EnableCap.DepthTest); // Enable capabilities
            GL.Enable (EnableCap.AlphaTest);
            GL.Enable (EnableCap.CullFace);
            GL.Enable (EnableCap.Blend);
            // Enable lighting
            GL.Enable (EnableCap.Lighting);
            GL.Enable (EnableCap.Light0);
            GL.ShadeModel (ShadingModel.Smooth);

            Vector4 ambient = new Vector4 (0.2f, 0.2f, 0.2f, 1.0f);
            Vector4 diffuse = new Vector4 (0.5f, 0.5f, 0.5f, 1.0f);
            Vector4 specular = new Vector4 (0.8f, 0.8f, 0.8f, 1.0f);
            GL.Light (LightName.Light0, LightParameter.Ambient, ambient);
            GL.Light (LightName.Light0, LightParameter.Diffuse, diffuse);
            GL.Light (LightName.Light0, LightParameter.Specular, specular);
        }

        protected override void OnResize (EventArgs e) {
            base.OnResize (e);

            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView (MathHelper.PiOver4, (float) (Width / (double) Height), 1, 1 << 16);
            GL.MatrixMode (MatrixMode.Projection);
            GL.LoadMatrix (ref perspective);

            GL.Viewport (0, 0, Width, Height);
        }

        protected override void OnUnload (EventArgs e) {
        }
        
        public void Render (long ticDelta) {
            EnsureUsable ();
            if (!Exists || IsExiting)
                return;
            ProcessEvents ();
            if (!Exists || IsExiting)
                return;

            ProcessEvents ();

            // Rendering code:
            double timeFrac = Core.GetTicTimeFrac ();
            localCamera = Core.Ticker.LocalPlayer.Pawn.Camera;
            this.MakeCurrent ();

            GL.Clear (ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode (MatrixMode.Modelview);
            GL.LoadIdentity ();
            GL.Rotate ((double) -localCamera.Pitch, 1, 0, 0);
            GL.Rotate ((double) -localCamera.Angle, 0, 1, 0);
            GL.Translate ((double) -localCamera.Position.X, (double) -(localCamera.Position.Z + localCamera.ViewHeight), (double) localCamera.Position.Y);

            // Position the light.
            /*Vector4 position = new Vector4 (0.0f, 25.0f, 0.0f, 1.0f);
            GL.Light (LightName.Light0, LightParameter.Position, position);*/

            this.ProcessGeometry ();
            this.ProcessActors ();

            this.SwapBuffers ();
        }

        public void ChangeResolution (int width, int height) {
            EnsureUsable ();
            this.Size = new Size (width, height);
        }
        public KeyboardState GetKeyboardState () {
            EnsureUsable ();
            return OpenTK.Input.Keyboard.GetState ();
        }
        public MouseState GetMouseState () {
            EnsureUsable ();
            return OpenTK.Input.Mouse.GetState ();
        }

        public void SetCursorVisibility (bool visible) {
            CursorVisible = visible;
        }
        public void SetCursor (Support.CursorIcon icon) {
            if (icon == null || icon.Height == -1 || icon.Width == -1 || icon.Data == null)
                Cursor = MouseCursor.Default;
            else
                Cursor = new MouseCursor (icon.X, icon.Y, icon.Width, icon.Height, icon.Data);
        }
        public void CenterCursor () {
            Point pt = PointToScreen (new Point (Width / 2, Height / 2));
            var mouse = OpenTK.Input.Mouse.GetCursorState ();
            if (mouse.X != pt.X || mouse.Y != pt.Y)
                OpenTK.Input.Mouse.SetPosition (pt.X, pt.Y);
        }
        #endregion
    }
}
