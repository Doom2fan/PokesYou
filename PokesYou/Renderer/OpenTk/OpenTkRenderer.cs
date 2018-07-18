using OpenTK;
using PokesYou.G_Console;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using PokesYou.Game;
using PokesYou.Renderer.OpenTk.Internal;
using System;
using System.Collections.Generic;
using System.Drawing;
using PokesYou.CMath;

namespace PokesYou.Renderer.OpenTk {
    public class OpenTkRenderer : GameWindow, IRenderEngine {
        public static readonly OpenTkRenderer Default = new OpenTkRenderer (800, 600, GraphicsMode.Default, "PokesYou", GameWindowFlags.Default, DisplayDevice.Default, 3, 1, GraphicsContextFlags.ForwardCompatible);

        protected internal Camera localCamera { get; set; }
        Matrix4 projectionMatrix;

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
        public event OnFocusChangeEventHandler OnFocusChange;

        protected void OpenTkRenderer_Closing (object sender, System.ComponentModel.CancelEventArgs e) {
            if (!e.Cancel) {
                Dispose ();
                Core.Exit ();
            }
        }

        private void OpenTkRenderer_FocusedChanged (object sender, EventArgs e) {
            OnFocusChange?.Invoke (this, !this.Focused);
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
            this.FocusedChanged += OpenTkRenderer_FocusedChanged;
            this.IsInitialized = true;
            this.VSync = VSyncMode.Off;

            OnLoad (EventArgs.Empty);
            OnResize (EventArgs.Empty);
        }

        public bool IsUsable () { return IsInitialized && !IsDisposed; }

        Shader shader;
        Model nanosuit;
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
            //GL.Enable (EnableCap.CullFace);
            GL.Enable (EnableCap.Blend);
            // Enable lighting
            //GL.Enable (EnableCap.Lighting);
            //GL.Enable (EnableCap.Texture2D);


            ///shader = new Shader (@"Shaders\GLSL\default.vert", @"Shaders\GLSL\default.frag");
            shader = new Shader (@"Shaders\GLSL\test.vert", @"Shaders\GLSL\test.frag");

            nanosuit = Model.LoadModel (@"models\nanosuit.obj");
        }

        protected override void OnResize (EventArgs e) {
            base.OnResize (e);

            projectionMatrix = Matrix4.CreatePerspectiveFieldOfView (MathHelper.DegreesToRadians (90), (float) (Width / (double) Height), 0.1f, (float) (1 << 24));

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
            
            shader.Use ();
            
            Vector3 cameraPos = localCamera.Position.ToGLVec3 ();

            float yaw = MathHelper.DegreesToRadians ((float) localCamera.Angle), pitch = MathHelper.DegreesToRadians (MathHelper.Clamp ((float) localCamera.Pitch, -89.9f, 89.9f));
            Matrix4 cameraFrontMatrix = Matrix4.Identity;
            cameraFrontMatrix *= Matrix4.CreateRotationZ (pitch);
            cameraFrontMatrix *= Matrix4.CreateRotationY (yaw);

            Vector3 cameraFront = Vector3.Transform (new Vector3 (1.0f, 0.0f, 0.0f), cameraFrontMatrix).Normalized ();
            Vector3 cameraUp = new Vector3 (0.0f, 1.0f, 0.0f);
            Matrix4 view = Matrix4.LookAt (cameraPos, cameraPos + cameraFront, cameraUp);
            
            Matrix4 model = Matrix4.Identity;
            model *= Matrix4.CreateTranslation (0.0f, 0.0f, 2.0f);
            model *= Matrix4.CreateScale (0.25f, 0.25f, 0.25f);

            shader.Use ();
            shader.SetMatrix ("ViewMatrix", view, false);
            shader.SetMatrix ("ProjectionMatrix", projectionMatrix, false);
            shader.SetMatrix ("ViewMatrix", model, false);

            shader.SetVector3 ("uCameraPos", cameraPos);
            shader.SetVector3 ("uCameraDir", cameraFront);

            shader.SetVector3 ("dirLight.direction", -0.2f, -1.0f, -0.3f);
            shader.SetVector3 ("dirLight.ambient", 0.05f, 0.05f, 0.05f);
            shader.SetVector3 ("dirLight.diffuse",  0.4f,  0.4f,  0.4f);
            shader.SetVector3 ("dirLight.specular", 0.5f,  0.5f,  0.5f);

            nanosuit.Draw (shader);

            /*this.ProcessGeometry ();
            this.ProcessActors ();*/

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
