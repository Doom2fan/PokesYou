using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.Game {
    public sealed class QuadtreeNode {
        private QuadtreeNode [] _nodes = null;

        /// <summary>
        /// The objects contained in this node.
        /// </summary>
        public List<Actor> Objects { get; set; } = new List<Actor> ();
        /// <summary>
        /// The node's subnodes.
        /// </summary>
        public QuadtreeNode [] Nodes {
            get {
                return _nodes;
            }
            set {
                if (value != null && value.Length != 4)
                    throw new ArgumentException ("Nodes must be a 4 element array");

                _nodes = value;
            }
        }
        /// <summary>
        /// The node's parent node.
        /// </summary>
        public QuadtreeNode Parent { get; set; } = null;
    }

    public class Quadtree {
        public const int ObjectCountThreshold = 15;
        public QuadtreeNode Root { get; protected set; }

        public Quadtree () {
            this.Root = new QuadtreeNode ();
        }

        
    }
}
