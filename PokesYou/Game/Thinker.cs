namespace PokesYou.Game {
    public class Thinker : GameObj, IThinker {
        public virtual void Tick () {
            if (this.ObjFlags.HasFlag (GameObjFlags.EuthanizeMe)) {
                this.RemoveThinker ();
                return;
            }
        }

        public virtual void Destroy () {
            this.ObjFlags |= GameObjFlags.EuthanizeMe;
            this.RemoveThinker ();
        }

        public IThinker PrevThinker { get; set; }
        public IThinker NextThinker { get; set; }

        public virtual void AddThinker () {
            this.ObjFlags &= ~GameObjFlags.EuthanizeMe; // Remove EuthanizeMe, just in case.
            Core.Ticker.AddThinker (this);
        }

        public virtual void RemoveThinker () {
            Core.Ticker.RemoveThinker (this);
        }
    }
}
