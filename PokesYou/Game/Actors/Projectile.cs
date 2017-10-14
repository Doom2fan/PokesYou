using PokesYou.CMath;
using PokesYou.Data;
using System;

namespace PokesYou.Game.Actors {
    public class Projectile : Actor {
        protected Projectile () : base () { flags |= ActorFlags.NoBlockmap; }
        public Projectile (ActorState state) : base (state) { flags |= ActorFlags.NoBlockmap; }

        /// <summary>
        /// The actor that fired this projectile.
        /// </summary>
        public Actor Shooter { get; set; }
        /// <summary>
        /// The damage this projectile does on impact.
        /// </summary>
        public int ImpactDamage { get; set; }

        /// <summary>
        /// Called when a projectile hits something -or- when a projectile is destructible and is shot down.
        /// Source will always be null when the projectile hit something.
        /// Source must NEVER be null if the projectile was destroyed, or this will cause severe problems.
        /// </summary>
        /// <param name="inflictor">The actor that got hit by the projectile -or- the actor that destroyed it.</param>
        /// <param name="source">Null if the </param>
        public override void Die (GameObj inflictor, GameObj source) {
            base.Die (inflictor, source);

            if (inflictor.GetType () == typeof (Actor)) {
                Actor act = (Actor) inflictor;

                if (source == null)
                    act.Damage (Shooter, this, ImpactDamage);
            }
        }

        /// <summary>
        /// Called to perform special collision response for collisions in the XY plane.
        /// </summary>
        /// <param name="performResponse">Whether to perform collision response.</param>
        /// <param name="actorCollided">The actor that was collided with.</param>
        /// <returns>A bool indicating whether the actor should stop movement.</returns>
        protected override bool SpecialCollisionResponseXY (bool performResponse, Actor actorCollided, bool firstCollision = false) {
            if (firstCollision)
                this.Die (actorCollided, null);

            return true;
        }
        /// <summary>
        /// Called to perform special collision response for collisions in the Z plane.
        /// </summary>
        /// <param name="performResponse">Whether to perform collision response.</param>
        /// <param name="actorCollided">The actor that was collided with.</param>
        /// <returns>A bool indicating whether the actor should stop movement.</returns>
        protected override bool SpecialCollisionResponseZ (bool performResponse, Actor actorCollided, bool firstCollision = false) {
            // The behaviour is the same, so just call SpecialCollisionResponseXY.
            return SpecialCollisionResponseXY (performResponse, actorCollided, firstCollision);
        }

        public override Accum GetFriction () { return Accum.One; }
    }
}
