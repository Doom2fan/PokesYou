using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PokesYou.Game {
    public interface IDestroyable {
        /// <summary>
        /// Gets the object's health
        /// </summary>
        int Health { get; }
        /// <summary>
        /// Gets the object's maximum health
        /// </summary>
        int MaxHealth { get; }
        /// <summary>
        /// Gets whether the object has been destroyed or not
        /// </summary>
        bool IsDead { get; }

        /// <summary>
        /// Sets the object's health
        /// </summary>
        /// <param name="health">The object's new health value</param>
        void SetHealth (int newHealth);
        /// <summary>
        /// Sets the object's maximum health
        /// </summary>
        /// <param name="newMax">The object's new maximum health</param>
        void SetMaxHealth (int newMax);
        /// <summary>
        /// Damages the object
        /// </summary>
        /// <param name="inflictor">The GameObj that inflicted the damage</param>
        /// <param name="source">The GameObj that caused the damage</param>
        /// <param name="damage">The amount of damage to be dealt</param>
        /// <returns>Returns the amount of damage actually dealt. (Or -1 if the damage was cancelled)</returns>
        int Damage (GameObj inflictor, GameObj source, int damage);
        /// <summary>
        /// Destroys the object
        /// </summary>
        /// <param name="inflictor">The GameObj that destroyed the object</param>
        /// <param name="source">The GameObj that caused the object's destruction</param>
        void Die (GameObj inflictor, GameObj source);
        /// <summary>
        /// Reverts the object's death
        /// </summary>
        /// <returns>Returns true if the object was successfully revived</returns>
        bool Revive ();
    }
}
