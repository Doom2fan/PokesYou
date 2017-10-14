using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.G_Console {
    public class ConsoleException : Exception {
        public ConsoleException (string message) : base (message) { }
        public ConsoleException (string message, Exception innerException) : base (message, innerException) { }
    }
    public static class GConsole {
        internal static List<CVar> cvarList = new List<CVar> ();
        public static BoolCVar debugMode = new BoolCVar ("debugMode", CVarFlags.Archive | CVarFlags.NoSave, true);

        internal static void RegisterCVar (CVar var) {
            if (cvarList.Exists (v => v.Name == var.Name))
                throw new ConsoleException (String.Format ("Cannot add CVar - a CVar with the name {0} already exists", var.Name));

            cvarList.Add (var);
        }

        public static void Write (object args) {
            try {
                Console.Write (args);
            } catch {
                throw;
            }
        }
        public static void Write (string format, params object [] args) {
            try {
                Console.Write (format, args);
            } catch {
                throw;
            }
        }
        public static void WriteLine (object args) {
            try {
                Write (args); Console.WriteLine ();
            } catch {
                throw;
            }
        }
        public static void WriteLine (string format, params object [] args) {
            try {
                Write (format, args); Console.WriteLine ();
            } catch {
                throw;
            }
        }

        public static class Debug {
            public static void Write (object args) {
                try {
                    if (debugMode)
                        Console.Write (args);
                } catch {
                    throw;
                }
            }
            public static void Write (string format, params object [] args) {
                try {
                    if (debugMode)
                        Console.Write (format, args);
                } catch {
                    throw;
                }
            }
            public static void WriteLine (object args) {
                try {
                    if (debugMode) {
                        Write (args);
                        Console.WriteLine ();
                    }
                } catch {
                    throw;
                }
            }
            public static void WriteLine (string format, params object [] args) {
                try {
                    if (debugMode) {
                        Write (format, args);
                        Console.WriteLine ();
                    }
                } catch {
                    throw;
                }
            }
        }
    }
}
