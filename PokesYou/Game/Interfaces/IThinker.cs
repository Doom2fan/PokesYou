using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.Game {
    public class ThinkerEnumeratorSentinel : IThinker {
        public IThinker NextThinker { get; set; }
        public IThinker PrevThinker { get { throw new NotImplementedException (); } set { throw new NotImplementedException (); } }
        public GameObjFlags ObjFlags { get { throw new NotImplementedException (); } set { throw new NotImplementedException (); } }
        public void AddThinker () { throw new NotImplementedException (); }
        public void Destroy () { throw new NotImplementedException (); }
        public void RemoveThinker () { throw new NotImplementedException (); }
        public void Tick () { throw new NotImplementedException (); }
    }
    public class ThinkerEnumerator : IEnumerable {
        protected IThinker first;

        public ThinkerEnumerator (IThinker firstNode) {
            first = firstNode;
        }

        public IThinker First {
            get {
                return first;
            }
        }

        public IEnumerator GetEnumerator () {
            return new Enumerator (this);
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator ();
        }

        public void SetFirst (IThinker newFirst) {
            first = newFirst;
        }

        protected sealed class Enumerator : IEnumerator {
            private IThinker first = new ThinkerEnumeratorSentinel ();
            private IThinker cur;
            private ThinkerEnumerator enumerator;

            public Enumerator (ThinkerEnumerator thinkerEnum) {
                enumerator = thinkerEnum;
                first.NextThinker = enumerator.first;
                cur = first;
            }

            public IThinker Current {
                get {
                    if (cur == null)
                        throw new InvalidOperationException ();

                    return cur;
                }
            }

            object IEnumerator.Current {
                get { return Current; }
            }

            public bool MoveNext () {
                if (cur != null && cur.NextThinker != null) {
                    cur = cur.NextThinker;
                    return true;
                } else
                    return false;
            }

            public void Reset () {
                cur = enumerator.first;
            }

            public void Dispose () { }
        }
    }

    public interface IThinker {
        /// <summary>
        /// The next thinker in the list
        /// </summary>
        GameObjFlags ObjFlags { get; set; }

        /// <summary>
        /// Performs the object's actions
        /// </summary>
        void Tick ();

        /// <summary>
        /// Destroys the thinker
        /// </summary>
        void Destroy ();

        /// <summary>
        /// The previous thinker in the list
        /// </summary>
        IThinker PrevThinker { get; set; }
        /// <summary>
        /// The next thinker in the list
        /// </summary>
        IThinker NextThinker { get; set; }

        /// <summary>
        /// Adds the thinker to the thinker list
        /// </summary>
        void AddThinker ();
        /// <summary>
        /// Removes the thinker from the thinker list
        /// </summary>
        void RemoveThinker ();
    }
}
