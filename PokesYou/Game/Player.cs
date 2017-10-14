using PokesYou.CMath;
using PokesYou.Game.Actors;

namespace PokesYou.Game {
    public interface PlayerControlInterface {
        /// <summary>
        /// Gets the local player's pressed keys (previous tic)
        /// </summary>
        Input.Keys PrevButtons { get; set; }
        /// <summary>
        /// Gets the local player's pressed keys
        /// </summary>
        Input.Keys Buttons { get; set; }
        /// <summary>
        /// The local player's forward/backwards movement
        /// </summary>
        Accum ForwardMove { get; set; }
        /// <summary>
        /// The local player's sideways movement
        /// </summary>
        Accum SidewaysMove { get; set; }
        /// <summary>
        /// The local player's yaw delta
        /// </summary>
        Accum YawDelta { get; set; }
        /// <summary>
        /// The local player's pitch delta
        /// </summary>
        Accum PitchDelta { get; set; }
    }

    public struct PlayerControl : PlayerControlInterface {
        public Input.Keys Buttons { get; set; }
        public Input.Keys PrevButtons { get; set; }

        public Accum ForwardMove { get; set; }
        public Accum SidewaysMove { get; set; }

        public Accum YawDelta { get; set; }
        public Accum PitchDelta { get; set; }
    }

    public class Player {
        protected PlayerPawn actor;

        /// <summary>
        /// The player's PlayerPawn
        /// </summary>
        public PlayerPawn Pawn {
            get { return actor; }
            set { actor = value; }
        }

        public PlayerControlInterface ControlInterface { get; set; }

        /// <summary>
        /// The player's name
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// The player's health. Only used between levels - Pawn.Health and Pawn.SetHealth are used during levels
        /// </summary>
        public int Health { get; set; }

        public void Tick () {
            Pawn.ChangeAngle (ControlInterface.YawDelta, true);
            Pawn.ChangePitch (ControlInterface.PitchDelta, true);
            
            if ((ControlInterface.ForwardMove | ControlInterface.SidewaysMove) != 0) {
                var speeds = new CMath.Vector3k (
                    (ControlInterface.ForwardMove) * FixedMath.SinDegrees (-Pawn.Angle) + (ControlInterface.SidewaysMove) * FixedMath.CosDegrees (-Pawn.Angle),
                    (ControlInterface.ForwardMove) * FixedMath.CosDegrees (-Pawn.Angle) - (ControlInterface.SidewaysMove) * FixedMath.SinDegrees (-Pawn.Angle),
                    Accum.Zero
                );
                
                Pawn.ChangeVelocity (speeds);
            }
        }
    }
}
