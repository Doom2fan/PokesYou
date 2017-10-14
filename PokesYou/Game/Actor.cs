using PokesYou.CMath;
using PokesYou.Data;
using PokesYou.G_Console;
using System;

namespace PokesYou.Game {
    [Flags]
    public enum ActorFlags : uint {
        /// <summary>The actor does not interact with the environment and with other objects.</summary>
        NoInteraction = 1,
        /// <summary>The actor cannot take damage</summary>
        Invulnerable  = 1 << 1,
        /// <summary>The actor is dormant</summary>
        Dormant       = 1 << 2,
        /// <summary>The actor has been killed or destroyed</summary>
        Killed        = 1 << 3,
        /// <summary>
        /// The actor is excluded from the blockmap/collision culling tree. (?)
        /// It can still interact with other things, but only as the instigator (missiles will run into other things, but nothing can run into a missile)
        /// </summary>
        NoBlockmap    = 1 << 4,
    }
    public class ActorState {
        public IRotationSet RotationSet { get; set; }
        public int Tics { get; set; }
        public ActorState Next { get; set; }
        public ActorState Prev { get; set; }
    }
    public class Actor : Thinker, IDestroyable {
        protected const int MINVELOCITY = 0x0000028F;

        #region Variables
        protected int stTime;
        protected ActorState state;
        protected int health;
        protected int maxHealth;
        protected BoundingCylinder bCylinder;
        protected Vector3k vel;
        protected Accum angle;
        protected Accum pitch;
        protected Vector3k prevPos;
        protected Accum prevAngle;
        protected Accum prevPitch;
        protected ActorFlags flags;
        protected Accum speed;
        protected Accum gravity;
        protected Camera cam;
        #endregion

        #region Constructors
        protected Actor () {
            maxHealth = health = 1000;
            prevPos = bCylinder.Position = vel = Vector3k.Zero;
            speed = angle = pitch = Accum.Zero;
            prevAngle = prevPitch = Accum.Zero;
            flags = 0;
            gravity = Accum.One;
            bCylinder = new BoundingCylinder (new Accum (16), new Accum (20), new Vector3k (Accum.Zero, Accum.Zero, Accum.Zero));
            cam = new Camera ();
        }
        public Actor (ActorState st) : this () {
            stTime = 0;
            state = st;
        }
        #endregion

        #region Properties

        #region States
        public ActorState State { get { return state; } }
        #endregion

        #region Physics
        /// <summary>
        /// Gets the actor's XYZ coordinates
        /// </summary>
        public virtual Vector3k PrevPosition {
            get { return prevPos; }
        }
        /// <summary>
        /// Gets the actor's XYZ coordinates
        /// </summary>
        public virtual Vector3k Position {
            get { return bCylinder.Position; }
        }
        /// <summary>
        /// Gets the actor's XYZ velocities
        /// </summary>
        public virtual Vector3k Velocity {
            get { return vel; }
        }
        /// <summary>
        /// Gets the actor's previous angle
        /// </summary>
        public virtual Accum PrevAngle {
            get { return prevAngle; }
        }
        /// <summary>
        /// Gets the actor's previous pitch
        /// </summary>
        public virtual Accum PrevPitch {
            get { return prevPitch; }
        }
        /// <summary>
        /// Gets the actor's angle
        /// </summary>
        public virtual Accum Angle {
            get { return angle; }
        }
        /// <summary>
        /// Gets the actor's pitch
        /// </summary>
        public virtual Accum Pitch {
            get { return pitch; }
        }
        /// <summary>
        /// Gets the actor's angle (In radians)
        /// </summary>
        public virtual Accum AngleRadians {
            get { return FixedMath.DegreesToRadians (angle); }
        }
        /// <summary>
        /// Gets the actor's pitch (In radians)
        /// </summary>
        public virtual Accum PitchRadians {
            get { return FixedMath.DegreesToRadians (pitch); }
        }
        /// <summary>
        /// Gets the actor's speed property
        /// </summary>
        public virtual Accum Speed {
            get { return speed; }
        }
        /// <summary>
        /// Gets the actor's radius in map units
        /// </summary>
        public virtual Accum Radius {
            get { return bCylinder.Radius; }
        }
        /// <summary>
        /// Gets the actor's height in map units
        /// </summary>
        public virtual Accum Height {
            get { return bCylinder.Height; }
        }

        /// <summary>
        /// Gets or sets the actor's base gravity
        /// </summary>
        public virtual Accum Gravity {
            get { return gravity; }
            set { gravity = value; }
        }
        #endregion

        #region Destroyable
        /// <summary>
        /// Gets the object's health
        /// </summary>
        public int Health {
            get { return health; }
        }

        /// <summary>
        /// Gets the object's maximum health
        /// </summary>
        public int MaxHealth {
            get { return maxHealth; }
        }

        /// <summary>
        /// Gets whether the object has been destroyed or not
        /// </summary>
        public bool IsDead {
            get { return (flags & ActorFlags.Killed) == ActorFlags.Killed; }
        }
        #endregion

        #region Flags
        /// <summary>
        /// Gets the actor's flags
        /// </summary>
        public ActorFlags Flags {
            get { return flags; }
        }
        #endregion

        #region Rendering
        /// <summary>
        /// Gets the actor's camera.
        /// </summary>
        public Camera Camera { get { return cam; } }
        #endregion

        #endregion

        #region Functions

        #region Physics
        /// <summary>
        /// Checks if the actor is colliding with anything
        /// </summary>
        /// <returns>Returns true if the actor is colliding with something</returns>
        public virtual bool IsColliding () {
            return false;
        }

        #region Position and velocities
        /// <summary>
        /// Sets the actor's velocity vector.
        /// </summary>
        /// <param name="newVel">The new velocity values</param>
        public virtual void SetVelocity (Vector3k newVel) {
            vel = newVel;
        }

        /// <summary>
        /// Set the actor's position
        /// </summary>
        /// <param name="newPos">The position to set the actor to</param>
        /// <param name="interpolate">Whether the actor should interpolate to the new position</param>
        public virtual void SetPosition (Vector3k newPos, bool interpolate = false) {
            if (interpolate) {
                prevPos = bCylinder.Position;
                bCylinder.Position = newPos;
            } else
                prevPos = bCylinder.Position = newPos;
        }

        /// <summary>
        /// Changes the actor's velocity vector.
        /// </summary>
        /// <param name="newVel">The values to change the velocity by</param>
        public virtual void ChangeVelocity (Vector3k newVel) {
            vel += newVel;
        }

        /// <summary>
        /// Changes the actor's position
        /// </summary>
        /// <param name="newPos">The values to change the position by</param>
        /// <param name="interpolate">Whether the actor should interpolate to the new position</param>
        public virtual void ChangePosition (Vector3k newPos, bool interpolate = false) {
            if (interpolate) {
                prevPos = bCylinder.Position;
                bCylinder.Position += newPos;
            } else
                prevPos = (bCylinder.Position += newPos);
        }
        #endregion

        #region Angles
        #region Degrees
        /// <summary>
        /// Set the actor's angle
        /// </summary>
        /// <param name="x">The new angle</param>
        /// <param name="interpolate">If true, the change will be interpolated</param>
        public virtual void SetAngle (Accum x, bool interpolate = false) {
            prevAngle = (interpolate ? angle : prevAngle);
            angle = MathUtils.WrapAngle (x);
        }
        /// <summary>
        /// Set the actor's pitch
        /// </summary>
        /// <param name="x">The new pitch</param>
        /// <param name="interpolate">If true, the change will be interpolated</param>
        public virtual void SetPitch (Accum x, bool interpolate = false) {
            prevPitch = (interpolate ? pitch : prevPitch);
            pitch = FixedMath.ClampInt (x, -90, 90);
        }

        /// <summary>
        /// Changes the actor's angle
        /// </summary>
        /// <param name="x">The value to change the angle by</param>
        /// <param name="interpolate">If true, the change will be interpolated</param>
        public virtual void ChangeAngle (Accum x, bool interpolate = false) {
            prevAngle = (interpolate ? angle : prevAngle);
            angle = MathUtils.WrapAngle (angle + x);
        }
        /// <summary>
        /// Changes the actor's pitch
        /// </summary>
        /// <param name="x">The value to change the pitch by</param>
        /// <param name="interpolate">If true, the change will be interpolated</param>
        public virtual void ChangePitch (Accum x, bool interpolate = false) {
            prevPitch = (interpolate ? pitch : prevPitch);
            pitch = FixedMath.ClampInt ((pitch + x), -90, 90);
        }
        #endregion

        #region Radians
        /// <summary>
        /// Set the actor's angle (In radians)
        /// </summary>
        /// <param name="x">The new angle</param>
        /// <param name="interpolate">If true, the change will be interpolated</param>
        public virtual void SetAngleRadians (Accum x, bool interpolate = false) {
            prevAngle = (interpolate ? angle : prevAngle);
            angle = MathUtils.WrapAngle (FixedMath.RadiansToDegrees (x));
        }
        /// <summary>
        /// Set the actor's pitch (In radians)
        /// </summary>
        /// <param name="x">The new pitch</param>
        /// <param name="interpolate">If true, the change will be interpolated</param>
        public virtual void SetPitchRadians (Accum x, bool interpolate = false) {
            prevPitch = (interpolate ? pitch : prevPitch);
            pitch = FixedMath.ClampInt (FixedMath.RadiansToDegrees (x), -90, 90);
        }

        /// <summary>
        /// Changes the actor's angle (In radians)
        /// </summary>
        /// <param name="x">The value to change the angle by</param>
        /// <param name="interpolate">If true, the change will be interpolated</param>
        public virtual void ChangeAngleRadians (Accum x, bool interpolate = false) {
            prevAngle = (interpolate ? angle : prevAngle);
            angle = MathUtils.WrapAngle (angle + FixedMath.RadiansToDegrees (x));
        }
        /// <summary>
        /// Changes the actor's pitch (In radians)
        /// </summary>
        /// <param name="x">The value to change the pitch by</param>
        /// <param name="interpolate">If true, the change will be interpolated</param>
        public virtual void ChangePitchRadians (Accum x, bool interpolate = false) {
            prevPitch = (interpolate ? pitch : prevPitch);
            pitch = FixedMath.ClampInt ((pitch + FixedMath.RadiansToDegrees (x)), -90, 90);
        }
        #endregion
        #endregion

        #region Sizes
        // Height
        /// <summary>
        /// Sets the actor's height
        /// </summary>
        /// <param name="newHeight">The actor's new height</param>
        /// <param name="checkSpace">Check for space. Reverts the change and returns false if the actor didn't fit after the height change.</param>
        /// <returns>If checkSpace is true, returns true if the new height was set successfully, and returns false if the actor didn't fit.
        /// If checkSpace is false, always returns true.</returns>
        public virtual bool SetHeight (Accum newHeight, bool checkSpace) {
            Accum oldHeight = bCylinder.Height;
            bCylinder.Height = newHeight;

            if (checkSpace && IsColliding ()) {
                bCylinder.Height = oldHeight;
                return false;
            } else
                return true;
        }
        /// <summary>
        /// Changes the actor's height
        /// </summary>
        /// <param name="newHeight">The value to change the actor's height by</param>
        /// <param name="checkSpace">Check for space. Reverts the change and returns false if the actor didn't fit after the height change.</param>
        /// <returns>If checkSpace is true, returns true if the new height was set successfully, and returns false if the actor didn't fit.
        /// If checkSpace is false, always returns true.</returns>
        public virtual bool ChangeHeight (Accum heightOff, bool checkSpace) {
            bCylinder.Height += heightOff;

            if (checkSpace && IsColliding ()) {
                bCylinder.Height -= heightOff;
                return false;
            } else
                return true;
        }
        // Radius
        /// <summary>
        /// Sets the actor's radius
        /// </summary>
        /// <param name="newHeight">The actor's new radius</param>
        /// <param name="checkSpace">Check for space. Reverts the change and returns false if the actor didn't fit after the radius change.</param>
        /// <returns>If checkSpace is true, returns true if the new radius was set successfully, and returns false if the actor didn't fit.
        /// If checkSpace is false, always returns true.</returns>
        public virtual bool SetRadius (Accum newRadius, bool checkSpace) {
            Accum oldRadius = bCylinder.Radius;
            bCylinder.Radius = newRadius;

            if (checkSpace && IsColliding ()) {
                bCylinder.Radius = oldRadius;
                return false;
            } else
                return true;
        }
        /// <summary>
        /// Changes the actor's radius
        /// </summary>
        /// <param name="newHeight">The value to change the actor's radius by</param>
        /// <param name="checkSpace">Check for space. Reverts the change and returns false if the actor didn't fit after the radius change.</param>
        /// <returns>If checkSpace is true, returns true if the new radius was set successfully, and returns false if the actor didn't fit.
        /// If checkSpace is false, always returns true.</returns>
        public virtual bool ChangeRadius (Accum radiusOff, bool checkSpace) {
            bCylinder.Radius += radiusOff;

            if (checkSpace && IsColliding ()) {
                bCylinder.Radius -= radiusOff;
                return false;
            } else
                return true;
        }
        #endregion
        #endregion

        #region Destroyable
        /// <summary>
        /// Sets the object's health
        /// </summary>
        /// <param name="health">The object's new health value</param>
        public void SetHealth (int newHealth) {
            health = newHealth;
        }

        /// <summary>
        /// Sets the object's maximum health
        /// </summary>
        /// <param name="newMax">The object's new maximum health</param>
        public void SetMaxHealth (int newMax) {
            maxHealth = newMax;
        }

        /// <summary>
        /// Damages the object
        /// </summary>
        /// <param name="inflictor">The GameObj that inflicted the damage</param>
        /// <param name="source">The GameObj that caused the damage</param>
        /// <param name="damage">The amount of damage to be dealt</param>
        /// <returns>Returns the amount of damage actually dealt. (Or -1 if the damage was cancelled)</returns>
        public virtual int Damage (GameObj inflictor, GameObj source, int damage) {
            health -= damage;

            if (health <= 0)
                Die (inflictor, source);

            return damage;
        }

        /// <summary>
        /// Destroys the object
        /// </summary>
        /// <param name="inflictor">The GameObj that destroyed the object</param>
        /// <param name="source">The GameObj that caused the object's destruction</param>
        public virtual void Die (GameObj inflictor, GameObj source) {
            flags |= ActorFlags.Killed;
        }

        /// <summary>
        /// Reverts the object's death
        /// </summary>
        /// <returns>Returns true if the object was successfully revived</returns>
        public virtual bool Revive () {
            return false;
        }
        #endregion

        #region Flags
        /// <summary>
        /// Sets the specified flags
        /// </summary>
        /// <param name="newFlags">The flags to set</param>
        public void SetFlags (ActorFlags val) {
            flags |= val;
        }
        /// <summary>
        /// Removes the specified flags
        /// </summary>
        /// <param name="val">The flags to remove</param>
        public void RemoveFlags (ActorFlags val) {
            flags &= ~val;
        }
        /// <summary>
        /// Toggles the specified flags
        /// </summary>
        /// <param name="val">The flags to toggle</param>
        public void ToggleFlags (ActorFlags val) {
            flags ^= val;
        }
        /// <summary>
        /// Checks if the specified flags are set
        /// </summary>
        public bool CheckFlags (ActorFlags val) {
            return (flags & val) == val;
        }
        #endregion

        public virtual void ChangeState (ActorState newState) {
            ActorState nextState = newState;
            do {
                state = nextState;
                stTime = state != null ? state.Tics : -1;
                if (state == null) {
                    ObjFlags |= GameObjFlags.EuthanizeMe;
                    this.Remove ();
                }
                nextState = state != null ? state.Next : null;
            } while (stTime == 0);
        }

        /// <summary>
        /// Destroys the actor
        /// </summary>
        public override void Destroy () {
            base.Destroy ();
        }

        /// <summary>
        /// Destroys the actor and removes it from the game
        /// </summary>
        public void Remove () {
            this.Destroy ();
        }
        #endregion

        public override void Tick () {
            base.Tick ();
            Camera.UpdateFromActor (this);

            if (state == null)
                ObjFlags |= GameObjFlags.EuthanizeMe;
            else if (stTime != -1 && --stTime <= 0) {
                if (state.Next != null)
                    ChangeState (state.Next);
                else {
                    GConsole.Debug.WriteLine ("Actor tried to change to null state.");
                    stTime = -1;
                }
            }

            if (ObjFlags.HasFlag (GameObjFlags.EuthanizeMe))
                return;

            DoMovement ();
            DoZMovement ();

            Accum friction = GetFriction ();
            vel.X *= ((Math.Abs (vel.X.Value) > MINVELOCITY) ? friction : Accum.Zero);
            vel.Y *= ((Math.Abs (vel.Y.Value) > MINVELOCITY) ? friction : Accum.Zero);

            if (bCylinder.X < Constants.CoordinatesMin || bCylinder.Y < Constants.CoordinatesMin || bCylinder.Z < Constants.CoordinatesMin ||
                bCylinder.X > Constants.CoordinatesMax || bCylinder.Y > Constants.CoordinatesMax || bCylinder.Z > Constants.CoordinatesMax) {
                this.Destroy ();
            }

            if (gravity > 0 && bCylinder.Z > 0)
                vel.Z -= ((flags & ActorFlags.NoInteraction) == ActorFlags.NoInteraction) ? GetLocalGravity () : GetGravity ();

            prevPos = bCylinder.Position;
        }

        #region Movement and collision detection
        /// <summary>
        /// Performs movement and collision detection in the XY plane.
        /// </summary>
        protected virtual void DoMovement () {
            if ((vel.X | vel.Y) == Accum.Zero) {
                DoXYCollisionDetection (true);
            } else if (FixedMath.Abs (vel.X * vel.X + vel.Y * vel.Y) < (this.Radius * this.Radius)) {
                bCylinder.X += vel.X;
                bCylinder.Y += vel.Y;
                if (DoXYCollisionDetection (true) == CollisionType.SpcColResp_StopMovement)
                    vel = Vector3k.Zero;
            } else {
                Accum moveCount = FixedMath.Ceil ((vel.X * vel.X + vel.Y * vel.Y) / this.Radius);
                Vector3k stepVel = new Vector3k (vel.X / this.Radius, vel.Y / this.Radius, Accum.Zero);
                for (int i = 0; i < moveCount; i += 1) {
                    bCylinder.X += vel.X;
                    bCylinder.Y += vel.Y;
                    if (DoXYCollisionDetection (true) == CollisionType.SpcColResp_StopMovement) {
                        vel = Vector3k.Zero;
                        return;
                    }
                }
                if (DoXYCollisionDetection (true) == CollisionType.SpcColResp_StopMovement)
                    vel = Vector3k.Zero;
            }
        }
        /// <summary>
        /// Performs movement and collision detection in the Z plane.
        /// </summary>
        protected virtual void DoZMovement () {
            Accum velZAbs = FixedMath.Abs (vel.Z);
            if (vel.Z == Accum.Zero) {
                DoZCollisionDetection (true);
            } else if (velZAbs < this.Height) {
                bCylinder.Position += vel;
                if (DoZCollisionDetection (true) == CollisionType.SpcColResp_StopMovement)
                    vel = Vector3k.Zero;
            } else {
                Accum moveCount = FixedMath.Ceil (velZAbs / this.Height);
                Accum stepVel = vel.Z / this.Height;
                for (int i = 0; i < moveCount; i += 1) {
                    bCylinder.Position += stepVel;
                    if (DoZCollisionDetection (true) == CollisionType.SpcColResp_StopMovement) {
                        vel = Vector3k.Zero;
                        return;
                    }
                }
                if (DoZCollisionDetection (true) == CollisionType.SpcColResp_StopMovement)
                    vel = Vector3k.Zero;
            }
        }
        
        /// <summary>
        /// Specifies the type of collision.
        /// </summary>
        public enum CollisionType {
            None = 0,
            Collision = 1,
            SpcColResp_StopMovement = 2,
        }
        /// <summary>
        /// Detects collisions in the XY plane.
        /// </summary>
        /// <param name="performResponse">Whether to fix any collisions. Defaults to true.</param>
        /// <returns>A CollisionType indicating whether a collision happened.</returns>
        public virtual CollisionType DoXYCollisionDetection (bool performResponse = true) {
            if (this.CheckFlags (ActorFlags.NoInteraction)) // Don't do collision detection if NoInteraction is set.
                return CollisionType.None;

            Vector3k deltaDist = Vector3k.Zero;
            Actor firstCollision = null;
            bool spcColRespStopMove = false;

            foreach (Actor act in Core.Ticker.Thinkers) { // Iterate through all actors.
                if (act == null || act == this) // Skip if the actor is null or act refers to itself
                    continue;
                if (act.CheckFlags (ActorFlags.NoBlockmap) || act.CheckFlags (ActorFlags.NoInteraction)) // Skip the actor if it has NoBlockmap or NoInteraction
                    continue;

                Vector3k distXY = bCylinder.IntersectionDistXY (act.bCylinder);
                if (distXY.LengthSquared < FixedMath.Square (this.Radius + act.Radius)) {
                    if (performResponse) {
                        deltaDist += (((act.Radius + this.Radius) / distXY.Length) - Accum.One) * distXY;
                    }
                    if (firstCollision == null)
                        firstCollision = act;

                    if (SpecialCollisionResponseXY (performResponse, act, act == firstCollision))
                        spcColRespStopMove = true;
                }
            }

            if (performResponse)
                bCylinder.Position += deltaDist;

            if (spcColRespStopMove)
                return CollisionType.SpcColResp_StopMovement;
            else if (firstCollision != null)
                return CollisionType.Collision;
            else
                return CollisionType.None;
        }
        /// <summary>
        /// Detects collisions in the Z plane.
        /// </summary>
        /// <param name="performResponse">Whether to fix any collisions. Defaults to true.</param>
        /// <returns>A CollisionType indicating whether a collision happened.</returns>
        public virtual CollisionType DoZCollisionDetection (bool performResponse = true) {
            if (this.CheckFlags (ActorFlags.NoInteraction)) // Don't do collision detection if NoInteraction is set.
                return CollisionType.None;

            Actor firstCollision = null;
            bool spcColRespStopMove = false;

            Accum deltaDist = bCylinder.Z;

            foreach (Actor act in Core.Ticker.Thinkers) { // Iterate through all actors.
                if (act == null || act == this) // Skip if the actor is null or act refers to itself
                    continue;
                if (act.CheckFlags (ActorFlags.NoBlockmap) || act.CheckFlags (ActorFlags.NoInteraction)) // Skip the actor if it has NoBlockmap or NoInteraction
                    continue;

                if (!bCylinder.IntersectsXY (act.bCylinder)) // If we're not colliding in the XY axes, skip. They can't be colliding in the Z axis if they aren't colliding in the XY axes!
                    continue;

                if (bCylinder.Z >= act.bCylinder.Z) {
                    if (performResponse)
                        bCylinder.Z = act.bCylinder.Top;

                    if (firstCollision == null)
                        firstCollision = act;

                    if (SpecialCollisionResponseZ (performResponse, act, act == firstCollision))
                        spcColRespStopMove = true;
                } else if (bCylinder.Top <= act.bCylinder.Top) {
                    if (performResponse)
                        bCylinder.Z = act.bCylinder.Z - Height;

                    if (firstCollision == null)
                        firstCollision = act;

                    if (SpecialCollisionResponseZ (performResponse, act, act == firstCollision))
                        spcColRespStopMove = true;
                }
            }

            if (performResponse)
                bCylinder.Z = deltaDist;

            if (spcColRespStopMove)
                return CollisionType.SpcColResp_StopMovement;
            else if (firstCollision != null)
                return CollisionType.Collision;
            else
                return CollisionType.None;
        }

        /// <summary>
        /// Called to perform special collision response for collisions in the XY plane.
        /// </summary>
        /// <param name="performResponse">Whether to perform collision response.</param>
        /// <param name="actorCollided">The actor that was collided with.</param>
        /// <returns>A bool indicating whether the actor should stop movement.</returns>
        protected virtual bool SpecialCollisionResponseXY (bool performResponse, Actor actorCollided, bool firstCollision = false) { return false; }
        /// <summary>
        /// Called to perform special collision response for collisions in the Z plane.
        /// </summary>
        /// <param name="performResponse">Whether to perform collision response.</param>
        /// <param name="actorCollided">The actor that was collided with.</param>
        /// <returns>A bool indicating whether the actor should stop movement.</returns>
        protected virtual bool SpecialCollisionResponseZ (bool performResponse, Actor actorCollided, bool firstCollision = false) { return false; }
        #endregion

        static readonly Accum _friction = Accum.MakeAccum (0x0000E800);
        public virtual Accum GetFriction () { return FixedMath.Clamp (_friction, Accum.Zero, Accum.One); }

        public virtual Accum GetGravity () { return gravity * Constants.BaseGravity; } // Right now these are the exact same, but will be different when level geometry is added
        public virtual Accum GetLocalGravity () { return gravity * Constants.BaseGravity; }
    }
}
