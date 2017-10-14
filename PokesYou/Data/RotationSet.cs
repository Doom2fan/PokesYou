using PokesYou.CMath;
using System;

namespace PokesYou.Data {
    /// <summary>
    /// Provides a sprite with a set of rotations.
    /// </summary>
    public interface IRotationSet {
        /// <summary>
        /// Gets a Sprite from the rotation set.
        /// </summary>
        /// <param name="angle">The angle of the sprite.</param>
        /// <returns>A Sprite.</returns>
        Sprite GetSprite (Accum angle);
        /// <summary>
        /// Gets a Sprite from the rotation set.
        /// </summary>
        /// <param name="angle">The angle of the sprite.</param>
        /// <returns>A Sprite.</returns>
        Sprite GetSprite (double angle);
    }

    /// <summary>
    /// A rotation set with an arbitrary number of rotations.
    /// </summary>
    public struct RotationSet : IRotationSet {
        private Sprite [] sprites;
        private double directionDiff;

        /// <summary>
        /// Creates a new rotation set.
        /// </summary>
        /// <param name="spriteSet">The set of sprites to use.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="spriteSet"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="spriteSet"/> is empty.</exception>
        public RotationSet (params Sprite [] spriteSet) {
            if (spriteSet == null)
                throw new ArgumentNullException ("spriteSet");
            if (spriteSet.Length < 1)
                throw new ArgumentException ("The sprite set cannot be empty.", "spriteSet");

            sprites = spriteSet;
            directionDiff = 360d / spriteSet.Length;
        }

        /// <summary>
        /// Gets a Sprite from the rotation set.
        /// </summary>
        /// <param name="angle">The angle of the sprite.</param>
        /// <returns>A Sprite.</returns>
        public Sprite GetSprite (Accum angle) {
            angle = MathUtils.WrapAngle (angle);
            if (angle > Accum.Zero)
                angle = new Accum (-360) + angle;
            return sprites [(int) Math.Floor (Math.Abs ((double) angle / directionDiff))];
        }
        /// <summary>
        /// Gets a Sprite from the rotation set.
        /// </summary>
        /// <param name="angle">The angle of the sprite.</param>
        /// <returns>A Sprite.</returns>
        public Sprite GetSprite (double angle) {
            angle = MathUtils.WrapAngle (angle);
            if (angle > 0)
                angle = -360 + angle;
            return sprites [(int) Math.Floor (Math.Abs (angle / directionDiff))];
        }
    }

    /// <summary>
    /// A rotation set that never renders.
    /// Should always be preferred over a blank sprite for performance reasons.
    /// </summary>
    public struct NullRotationSet : IRotationSet {
        /// <summary>
        /// Gets an empty sprite.
        /// </summary>
        /// <param name="angle">Does nothing.</param>
        /// <returns>A Sprite.</returns>
        public Sprite GetSprite (Accum angle) { return Sprite.EmptySprite; }
        /// <summary>
        /// Gets an empty sprite.
        /// </summary>
        /// <param name="angle">Does nothing.</param>
        /// <returns>A Sprite.</returns>
        public Sprite GetSprite (double angle) { return Sprite.EmptySprite; }
    }
}
