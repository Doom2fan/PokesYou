using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.CMath {
    public class Vertex {
        public Accum X { get; set; }
        public Accum Y { get; set; }
        public Accum Z { get; set; }
        public Accum Normal { get; set; }
        public Vector3k Pos {
            get { return new Vector3k (X, Y, Z); }
            set {
                X = value.X;
                Y = value.Y;
                Z = value.Z;
            }
        }

        public Vertex (Accum x, Accum y, Accum z) {
            X = x;
            Y = y;
            Z = z;
        }

        public Vertex (Accum x, Accum y, Accum z, Accum normal) :
            this (x, y, z) {
            Normal = normal;
        }
    }
}
