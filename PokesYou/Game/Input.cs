using OpenTK;
using OpenTK.Input;
using System;

namespace PokesYou.Game {
    public static class Input {
        /** Keyboard info **/
        private static KeyboardState kbState;
        /** Mouse info **/
        private static MouseState mState, mStatePrev;
        private static double m_sensitivity = 1.0, m_yaw = 1.0, m_pitch = 1.0;
        private static bool mouseGrabbed = false;
        private static int mouseUngrabbedX, mouseUngrabbedY;

        /// <summary>
        /// The local player's pressed keys (previous tic)
        /// </summary>
        private static Keys locKeysPrev = 0;
        /// <summary>
        /// The local player's pressed keys
        /// </summary>
        private static Keys locKeys = 0;

        /// <summary>
        /// Gets the local player's pressed keys (previous tic)
        /// </summary>
        public static Keys PrevButtons {
            get { return locKeysPrev; }
        }
        /// <summary>
        /// Gets the local player's pressed keys
        /// </summary>
        public static Keys Buttons {
            get { return locKeys; }
        }

        /// <summary>
        /// Is the mouse currently grabbed?
        /// </summary>
        public static bool IsMouseGrabbed { get { return mouseGrabbed; } }
        /// <summary>
        /// The local player's forward/backwards movement
        /// </summary>
        public static double ForwardMove { get; private set; } = 0.0;
        /// <summary>
        /// The local player's sideways movement
        /// </summary>
        public static double SidewaysMove { get; private set; } = 0.0;
        /// <summary>
        /// The local player's yaw delta
        /// </summary>
        public static double YawDelta { get; private set; } = 0.0;
        /// <summary>
        /// The local player's pitch delta
        /// </summary>
        public static double PitchDelta { get; private set; } = 0.0;
        /// <summary>
        /// The local player's "total" yaw delta
        /// </summary>
        public static double TotalYawDelta { get; private set; } = 0.0;
        /// <summary>
        /// The local player's "total" pitch delta
        /// </summary>
        public static double TotalPitchDelta { get; private set; } = 0.0;
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

        /// <summary>
        /// Checks if a key was pressed
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>Returns true if the key was pressed</returns>
        public static bool KeyPressed (Keys key) {
            return ((locKeysPrev & key) > 0) && ((locKeys & key) == 0);
        }
        /// <summary>
        /// Checks if a key was pressed
        /// </summary>
        /// <param name="key">The key to check for</param>
        /// <returns>Returns true if the key was pressed</returns>
        public static bool KeyDown (Keys key) {
            return (locKeys & key) >  0;
        }
        public static bool KeyUp (Keys key) {
            return (locKeys & key) == 0;
        }

        public static void GrabMouse () {
            if (mouseGrabbed)
                return;

            var cursor = OpenTK.Input.Mouse.GetCursorState ();
            mouseUngrabbedX = cursor.X;
            mouseUngrabbedY = cursor.Y;
            Core.RenderEngine.SetCursor (Support.CursorIcon.Empty);
            Core.RenderEngine.SetCursorVisibility (false);

            mouseGrabbed = true;
        }

        public static void ReleaseMouse () {
            if (!mouseGrabbed)
                return;

            Core.RenderEngine.SetCursor (Support.CursorIcon.Default);
            Core.RenderEngine.SetCursorVisibility (true);
            OpenTK.Input.Mouse.SetPosition (mouseUngrabbedX, mouseUngrabbedY);

            mouseGrabbed = false;
        }

        public static void ClearTotals () {
            TotalYawDelta = TotalPitchDelta = 0;
        }

        /// <summary>
        /// Updates the input state
        /// </summary>
        public static void UpdateInput () {
            if (!GameState.IsFocused) {
                if (mouseGrabbed)
                    ReleaseMouse ();
                return;
            }

            if (!mouseGrabbed)
                GrabMouse ();

            mStatePrev = mState;
            kbState = Core.RenderEngine.GetKeyboardState ();
            mState = Core.RenderEngine.GetMouseState ();
            
            locKeysPrev = locKeys;
            ForwardMove = SidewaysMove = 0;

            if (kbState.IsKeyDown (Key.W))
                ForwardMove += 1;
            if (kbState.IsKeyDown (Key.S))
                ForwardMove += -1;
            if (kbState.IsKeyDown (Key.D))
                SidewaysMove += 1;
            if (kbState.IsKeyDown (Key.A))
                SidewaysMove += -1;

            // Mouse
            locKeys = (mState.LeftButton  == ButtonState.Pressed) ? (locKeys | Keys.Attack)    : (locKeys & ~Keys.Attack);
            locKeys = (mState.RightButton == ButtonState.Pressed) ? (locKeys | Keys.AltAttack) : (locKeys & ~Keys.AltAttack);

            YawDelta = (mStatePrev.X - mState.X) * m_yaw * m_sensitivity;
            TotalYawDelta += YawDelta;
            PitchDelta = (mStatePrev.Y - mState.Y) * m_pitch * m_sensitivity;
            TotalPitchDelta += PitchDelta;
            Core.RenderEngine.CenterCursor ();
            // Keyboard
            if (kbState.IsKeyDown (Key.Escape))
                Core.Exit ();
        }
    }
}
