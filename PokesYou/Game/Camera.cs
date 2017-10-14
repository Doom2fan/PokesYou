using PokesYou.CMath;

namespace PokesYou.Game {
    /// <summary>
    /// A camera object, to be used by renderers
    /// </summary>
    public class Camera {
        /// <summary>
        /// The camera's position.
        /// </summary>
        public Vector3k Position { get; set; }
        /// <summary>
        /// The camera's view height
        /// This must be added to the camera's Z position when determining the rendering location.
        /// </summary>
        public Accum ViewHeight { get; set; }
        /// <summary>
        /// The camera's angle.
        /// </summary>
        public Accum Angle { get; set; }
        /// <summary>
        /// The camera's pitch.
        /// </summary>
        public Accum Pitch { get; set; }
        /// <summary>
        /// The actor that the camera belongs to.
        /// </summary>
        public Actor Owner { get; set; }

        public void UpdateFromActor (Actor actor) {
            this.Position = actor.Position;
            this.Angle = actor.Angle;
            this.Pitch = actor.Pitch;
            this.Owner = actor;
        }
    }
}
