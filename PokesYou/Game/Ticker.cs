using PokesYou.CMath;
using PokesYou.Data;
using PokesYou.Data.Managers;
using PokesYou.Game.Actors;

namespace PokesYou.Game {
    public interface ITicker {
        Player LocalPlayer { get; set; }
        Player [] Players { get; set; }
        ThinkerEnumerator Thinkers { get; }

        void Initialize ();
        void Update (long ticDelta);
        void AddThinker (IThinker thinker);
        void RemoveThinker (IThinker thinker);
    }

    public class Ticker : ITicker {
        Player [] playerList = new Player [32];
        ThinkerEnumerator thinkers = new ThinkerEnumerator (null);

        public Player LocalPlayer { get; set; }
        public Player [] Players {
            get { return playerList; }
            set { playerList = value; }
        }
        public ThinkerEnumerator Thinkers { get { return thinkers; } }

        public void Initialize () {
            ActorState state = new ActorState ();
            state.Tics = 1; state.Next = state; state.Prev = state;
            state.RotationSet = new RotationSet (new Sprite [] {
                new Sprite (TextureManager.LoadTexture ("PLAYA1", false), System.Drawing.Color.White, new OpenTK.Vector2 (12, 56), 0, 0, 0, OpenTK.Vector2.One, Renderer.Billboarding.Both),
                new Sprite (TextureManager.LoadTexture ("PLAYA2", false), System.Drawing.Color.White, new OpenTK.Vector2 (21, 55), 0, 0, 0, OpenTK.Vector2.One, Renderer.Billboarding.Both),
                new Sprite (TextureManager.LoadTexture ("PLAYA3", false), System.Drawing.Color.White, new OpenTK.Vector2 (26, 53), 0, 0, 0, OpenTK.Vector2.One, Renderer.Billboarding.Both),
                new Sprite (TextureManager.LoadTexture ("PLAYA4", false), System.Drawing.Color.White, new OpenTK.Vector2 (18, 52), 0, 0, 0, OpenTK.Vector2.One, Renderer.Billboarding.Both),
                new Sprite (TextureManager.LoadTexture ("PLAYA5", false), System.Drawing.Color.White, new OpenTK.Vector2 (12, 51), 0, 0, 0, OpenTK.Vector2.One, Renderer.Billboarding.Both),
                new Sprite (TextureManager.LoadTexture ("PLAYA6", false), System.Drawing.Color.White, new OpenTK.Vector2 (18, 52), 0, 0, 0, OpenTK.Vector2.One, Renderer.Billboarding.Both),
                new Sprite (TextureManager.LoadTexture ("PLAYA7", false), System.Drawing.Color.White, new OpenTK.Vector2 (26, 53), 0, 0, 0, OpenTK.Vector2.One, Renderer.Billboarding.Both),
                new Sprite (TextureManager.LoadTexture ("PLAYA8", false), System.Drawing.Color.White, new OpenTK.Vector2 (21, 55), 0, 0, 0, OpenTK.Vector2.One, Renderer.Billboarding.Both),
            });

            PlayerPawn pawn = new PlayerPawn (state);
            pawn.SetMaxHealth (100);
            pawn.SetHeight (new Accum (56), false);
            pawn.SetRadius (new Accum (16), false);
            pawn.Camera.ViewHeight = new Accum (48);

            LocalPlayer = new Player ();
            LocalPlayer.ControlInterface = new PlayerControl ();
            LocalPlayer.Pawn = pawn;
            pawn.Player = LocalPlayer;
            playerList [0] = LocalPlayer;

            pawn.AddThinker ();

            Actor actor = new Actor (state);
            actor.SetMaxHealth (100);
            actor.SetHeight (new Accum (56), false);
            actor.SetRadius (new Accum (16), false);
            actor.SetPosition (new CMath.Vector3k (new Accum (0), new Accum (150), new Accum (0)));
            //actor.AddThinker ();

            actor = new Actor (state);
            actor.SetMaxHealth (100);
            actor.SetHeight (new Accum (56), false);
            actor.SetRadius (new Accum (16), false);
            actor.SetPosition (new CMath.Vector3k (new Accum (0), new Accum (-150), new Accum (0)));
            actor.AddThinker ();

            actor = new Projectile (state);
            actor.SetHeight (new Accum (16), false);
            actor.SetRadius (new Accum (16), false);
            actor.SetPosition (new CMath.Vector3k (new Accum (0), new Accum (250), new Accum (0)));
            actor.AddThinker ();
            actor.SetVelocity (new Vector3k (Accum.Zero, new Accum (-25), Accum.Zero));
        }

        public void Update (long ticDelta) {
            LocalPlayer.ControlInterface.PrevButtons = Input.PrevButtons;
            LocalPlayer.ControlInterface.Buttons = Input.Buttons;
            LocalPlayer.ControlInterface.ForwardMove = new Accum (Input.ForwardMove);
            LocalPlayer.ControlInterface.SidewaysMove = new Accum (Input.SidewaysMove);
            LocalPlayer.ControlInterface.YawDelta = new Accum (Input.TotalYawDelta);
            LocalPlayer.ControlInterface.PitchDelta = new Accum (Input.TotalPitchDelta);

            foreach (Player player in playerList) {
                if (player == null)
                    break;

                player.Tick ();
            }

            foreach (IThinker thinker in thinkers)
                thinker.Tick ();
        }

        public void AddThinker (IThinker thinker) {
            if (thinkers.First == null) {
                thinker.PrevThinker = null;
                thinker.NextThinker = null;
                thinkers.SetFirst (thinker);
            } else {
                thinker.PrevThinker = null;
                thinker.NextThinker = thinkers.First;
                thinkers.First.PrevThinker = thinker;
                thinkers.SetFirst (thinker);
            }
        }
        public void RemoveThinker (IThinker thinker) {
            if (thinker.NextThinker != null)
                thinker.NextThinker.PrevThinker = thinker.PrevThinker;
            if (thinker.PrevThinker != null)
                thinker.PrevThinker.NextThinker = thinker.NextThinker;
        }
    }
}
