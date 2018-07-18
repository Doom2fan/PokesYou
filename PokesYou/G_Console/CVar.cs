using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PokesYou.G_Console {
    [Flags]
    public enum CVarFlags : int {
        /// <summary>Saved to config</summary>
        Archive = 1,
        /// <summary>Cannot be changed by the user</summary>
        ReadOnly = 1 << 1,
        /// <summary>Add to userinfo when changed</summary>
        User = 1 << 2,
        /// <summary>Add to serverinfo when changed</summary>
        Server = 1 << 3,
        /// <summary>Saved to demos</summary>
        SaveDemo = 1 << 4,
        /// <summary>Don't save to savegame</summary>
        NoSave = 1 << 5,
        /// <summary>Don't change until server restarts (e.g. a new game is started)</summary>
        Latch = 1 << 6,
    }

    /// <summary>
    /// Thrown by CVar related functions
    /// </summary>
    public class CVarTypeException : Exception { }
    
    /// <summary>
    /// Defines a console variable
    /// </summary>
    public abstract class CVar {
        public CVar (string name) : this (name, 0) { }
        public CVar (string name, CVarFlags flags) {
            Name = name;
            Flags = flags;

            GConsole.RegisterCVar (this);
        }

        /// <summary>
        /// Gets or sets the CVar's name
        /// </summary>
        public virtual string Name { get; set; } = null;
        /// <summary>
        /// Gets or sets the CVar's flags
        /// </summary>
        public virtual CVarFlags Flags { get; set; } = 0;
        /// <summary>
        /// Gets the CVar's type
        /// </summary>
        public abstract Type CVarType { get; }
        /// <summary>
        /// Gets or sets the CVar's value
        /// </summary>
        public abstract object Value { get; set; }
        /// <summary>
        /// Converts the CVar's value to a string
        /// </summary>
        /// <returns>Returns the CVar's value in a string representation</returns>
        public abstract string AsString ();
    }

    #region Built-in CVar types

    /// <summary>
    /// Defines an int CVar
    /// </summary>
    public sealed class IntCVar : CVar {
        private int val;

        public IntCVar (string name, CVarFlags flags = 0, int init = 0) :
            base (name, flags) {
            val = init;
        }
        
        public override Type CVarType { get { return typeof (int); } }
        public override object Value {
            get { return val; }
            set {
                if (value.GetType () != typeof (int))
                    throw new CVarTypeException ();

                val = (int) value;
            }
        }
        public override string AsString () {
            return val.ToString ();
        }

        public static implicit operator int (IntCVar value) {
            return value.val;
        }
    }

    /// <summary>
    /// Defines a string CVar
    /// </summary>
    public sealed class StringCVar : CVar {
        private string val;

        public StringCVar (string name, CVarFlags flags = 0, string init = "") :
            base (name, flags) {
            val = init;
        }

        public override Type CVarType { get { return typeof (string); } }
        public override object Value {
            get { return val; }
            set {
                if (value.GetType () != typeof (string))
                    throw new CVarTypeException ();

                val = (string) value;
            }
        }
        public override string AsString () {
            return val;
        }

        public static implicit operator string (StringCVar value) {
            return value.val;
        }
    }

    /// <summary>
    /// Defines a bool CVar
    /// </summary>
    public sealed class BoolCVar : CVar {
        private bool val;

        public BoolCVar (string name, CVarFlags flags = 0, bool init = false) :
            base (name, flags) {
            val = init;
        }

        public override Type CVarType { get { return typeof (bool); } }
        public override object Value {
            get { return val; }
            set {
                if (value.GetType () != typeof (bool))
                    throw new CVarTypeException ();

                val = (bool) value;
            }
        }
        public override string AsString () {
            return val.ToString ();
        }

        public static implicit operator bool (BoolCVar value) {
            return value.val;
        }
    }

    /// <summary>
    /// Defines a fixed-point CVar
    /// </summary>
    public sealed class FixedCVar : CVar {
        private CMath.Accum val;

        public FixedCVar (string name, CVarFlags flags = 0) : base (name, flags) { }

        public override Type CVarType { get { return typeof (CMath.Accum); } }
        public override object Value {
            get { return val; }
            set {
                if (value.GetType () != typeof (CMath.Accum))
                    throw new CVarTypeException ();

                val = (CMath.Accum) value;
            }
        }
        public override string AsString () {
            return val.ToString ();
        }

        public static implicit operator CMath.Accum (FixedCVar value) {
            return value.val;
        }
    }

    /// <summary>
    /// Defines a RGB color CVar
    /// </summary>
    public sealed class ColorCVar : CVar {
        private Color val;

        public ColorCVar (string name, CVarFlags flags = 0) : base (name, flags) { }

        public override Type CVarType { get { return typeof (Color); } }
        public override object Value {
            get { return val; }
            set {
                if (value.GetType () != typeof (Color))
                    throw new CVarTypeException ();

                val = (Color) value;
            }
        }
        public override string AsString () {
            return val.ToString ();
        }

        public static implicit operator Color (ColorCVar value) {
            return value.val;
        }
    }

    /// <summary>
    /// Defines a dummy CVar (Just redirects to another CVar)
    /// </summary>
    public sealed class DummyCVar : CVar {
        private CVar val;

        public DummyCVar (string name, CVarFlags flags = 0, CVar init = null) :
            base (name, flags) {
            val = init;
        }

        public override Type CVarType { get { return typeof (CVar); } }
        public override object Value {
            get { return val; }
            set {
                if (value.GetType () != typeof (CVar))
                    throw new CVarTypeException ();

                val = (CVar) value;
            }
        }
        public override string AsString () {
            return val.AsString ();
        }
    }

    #endregion
}
