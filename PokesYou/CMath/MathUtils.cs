using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.CMath {
    public static class MathUtils {
        public static int WrapAngle (int x) { return x % 360; }
        public static double WrapAngle (double x) { return x % 360d; }
        public static Accum WrapAngle (Accum x) {
            const int val = 360 << 16; // Just to make sure it doesn't calculate 360 << 16 on every WrapAngle call...
            return Accum.MakeAccum (x.Value % val);
        }

        public static bool IsBetween (Accum x, Accum min, Accum max) {
            return (x >= min && x <= max);
        }

        public static Vector3k CalculateSurfaceNormal (Vertex [] verts) {
            Vector3k normal = new Vector3k (Accum.Zero, Accum.Zero, Accum.Zero);
            Vertex cur, next;
            int vCount = verts.Length;

            for (int i = 0; i < vCount; i++) {
                cur = verts [i];
                next = verts [(i + 1) % vCount];

                normal.X += (cur.Y - next.Y) * (cur.Z + next.Z);
                normal.Y += (cur.Z - next.Z) * (cur.X + next.X);
                normal.Z += (cur.X - next.X) * (cur.Y + next.Y);
            }

            normal.Normalize ();

            return normal;
        }
    }
}
