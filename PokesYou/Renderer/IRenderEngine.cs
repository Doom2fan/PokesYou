using OpenTK.Input;
using System;

namespace PokesYou.Renderer {
    [Flags]
    public enum Billboarding {
        None = 0,
        X = 1,
        Y = 2,
        Both = X | Y,
    }
    
    public delegate void OnFocusChangeEventHandler (object sender, bool lost);
    /// <summary>
    /// Defines an interface for renderers.
    /// This also handles part of the input.
    /// </summary>
    public interface IRenderEngine : IDisposable {
        #region Properties/Fields
        /// <summary>
        /// Gets the window's width
        /// </summary>
        int WindowWidth { get; }
        /// <summary>
        /// Gets the window's height
        /// </summary>
        int WindowHeight { get; }
        /// <summary>
        /// Gets or sets the window's fullscreen state
        /// </summary>
        bool IsFullscreen { get; set; }
        /// <summary>
        /// Gets or sets the window's VSync
        /// </summary>
        bool WindowVSync { get; set; }
        /// <summary>
        /// Gets or sets the window's title
        /// </summary>
        string WindowTitle { get; set; }
        #endregion

        #region Events
        event OnFocusChangeEventHandler OnFocusChange;
        #endregion

        #region Functions
        /// <summary>
        /// Initializes the renderer
        /// </summary>
        void Initialize (int width, int height, bool fullscreen);
        /// <summary>
        /// Is the renderer usable?
        /// </summary>
        /// <returns></returns>
        bool IsUsable ();
        /// <summary>
        /// Renders the game
        /// </summary>
        /// <param name="ticDelta">The delta time since the ticker last ran</param>
        void Render (long ticDelta);
        /// <summary>
        /// Changes the window's resolution
        /// </summary>
        /// <param name="width">The window's new width</param>
        /// <param name="height">The window's new height</param>
        void ChangeResolution (int width, int height);
        /// <summary>
        /// Changes whether the cursor is visible and grabbed (locked inside the window)
        /// If the visible argument is false, the cursor is grabbed and not visible.
        /// </summary>
        /// <param name="visible">Whether or not the cursor is visible</param>
        void SetCursorVisibility (bool visible);
        /// <summary>
        /// Sets the cursor's image
        /// </summary>
        /// <param name="icon">The image to set the cursor to</param>
        void SetCursor (Support.CursorIcon icon);
        /// <summary>
        /// Positions the cursor at the center of the RenderEngine's window
        /// </summary>
        void CenterCursor ();
        /// <summary>
        /// Gets the render window's keyboard state
        /// </summary>
        /// <returns>The keyboard's state as an OpenTK KeyboardState class</returns>
        KeyboardState GetKeyboardState ();
        /// <summary>
        /// Gets the render window's mouse state
        /// </summary>
        /// <returns>The mouse's state as an OpenTK MouseState class</returns>
        MouseState GetMouseState ();
        #endregion
    }
}
