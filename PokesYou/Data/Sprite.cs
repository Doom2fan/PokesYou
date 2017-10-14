using OpenTK;
using PokesYou.CMath;
using PokesYou.Renderer;
using System;
using System.Drawing;

namespace PokesYou.Data {
    /// <summary>
    /// A sprite.
    /// A null Sprite is valid, and means that the renderer shouldn't render it.
    /// </summary>
    public class Sprite {
        /// <summary>
        /// Gets an empty sprite.
        /// </summary>
        public const Sprite EmptySprite = null;

        protected double rot, angle, pitch;

        /// <summary>
        /// Gets or sets the sprite's texture.
        /// </summary>
        public Texture Texture { get; set; }
        /// <summary>
        /// Gets or sets the global color of the sprite.
        /// This color is modulated (multiplied) with the sprite's texture. It can be used to colorize the sprite, or change its global opacity. By default, the sprite's color is opaque white.
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// Gets or sets the sprite's offset.
        /// </summary>
        public Vector2 Offset { get; set; }
        /// <summary>
        /// Gets or sets the sprite's rotation.
        /// </summary>
        public double Rotation {
            get { return rot; }
            set { rot = MathUtils.WrapAngle (value); }
        }
        /// <summary>
        /// Gets or sets the sprite's angle.
        /// Only used when X billboarding is off.
        /// </summary>
        public double Angle {
            get { return angle; }
            set { angle = MathUtils.WrapAngle (value); }
        }
        /// <summary>
        /// Gets or sets the sprite's pitch.
        /// Only used when Y billboarding is off.
        /// </summary>
        public double Pitch {
            get { return pitch; }
            set { pitch = MathUtils.WrapAngle (value); }
        }
        /// <summary>
        /// Gets or sets the sprite's scaling.
        /// </summary>
        public Vector2 Scale { get; set; }
        /// <summary>
        /// Gets or sets the sprite's billboarding.
        /// </summary>
        public Billboarding Billboard { get; set; }

        protected Sprite () { } // Disallow the default constructor

        /// <summary>
        /// Creates a new Sprite.
        /// </summary>
        /// <param name="tex">The texture to use.</param>
        /// <exception cref="ArgumentNullException">Tex is null.</exception>
        public Sprite (Texture tex) : this (tex, Color.White, Vector2.One, 0d, 0d, 0d, Vector2.One, Billboarding.X) { }
        /// <summary>
        /// Creates a new Sprite.
        /// </summary>
        /// <param name="tex">The texture to use.</param>
        /// <param name="col">The sprite's color.</param>
        /// <exception cref="ArgumentNullException">Tex is null.</exception>
        public Sprite (Texture tex, Color col) : this (tex, col, Vector2.One, 0d, 0d, 0d, Vector2.One, Billboarding.X) { }
        /// <summary>
        /// Creates a new Sprite.
        /// </summary>
        /// <param name="tex">The texture to use.</param>
        /// <param name="col">The sprite's color.</param>
        /// <param name="offs">The sprite's offsets.</param>
        /// <exception cref="ArgumentNullException">Tex is null.</exception>
        public Sprite (Texture tex, Color col, Vector2 offs) : this (tex, col, offs, 0d, 0d, 0d, Vector2.One, Billboarding.X) { }
        /// <summary>
        /// Creates a new Sprite.
        /// </summary>
        /// <param name="tex">The texture to use.</param>
        /// <param name="col">The sprite's color.</param>
        /// <param name="offs">The sprite's offsets.</param>
        /// <param name="rot">The sprite's rotation.</param>
        /// <exception cref="ArgumentNullException">Tex is null.</exception>
        public Sprite (Texture tex, Color col, Vector2 offs, double rot) : this (tex, col, offs, rot, 0d, 0d, Vector2.One, Billboarding.X) { }
        /// <summary>
        /// Creates a new Sprite.
        /// </summary>
        /// <param name="tex">The texture to use.</param>
        /// <param name="col">The sprite's color.</param>
        /// <param name="offs">The sprite's offsets.</param>
        /// <param name="rot">The sprite's rotation.</param>
        /// <param name="ang">The sprite's angle.</param>
        /// <exception cref="ArgumentNullException">Tex is null.</exception>
        public Sprite (Texture tex, Color col, Vector2 offs, double rot, double ang) : this (tex, col, offs, rot, ang, 0d, Vector2.One, Billboarding.X) { }
        /// <summary>
        /// Creates a new Sprite.
        /// </summary>
        /// <param name="tex">The texture to use.</param>
        /// <param name="col">The sprite's color.</param>
        /// <param name="offs">The sprite's offsets.</param>
        /// <param name="rot">The sprite's rotation.</param>
        /// <param name="ang">The sprite's angle.</param>
        /// <param name="pitch">The sprite's pitch.</param>
        /// <exception cref="ArgumentNullException">Tex is null.</exception>
        public Sprite (Texture tex, Color col, Vector2 offs, double rot, double ang, double pitch) : this (tex, col, offs, rot, ang, pitch, Vector2.One, Billboarding.X) { }
        /// <summary>
        /// Creates a new Sprite.
        /// </summary>
        /// <param name="tex">The texture to use.</param>
        /// <param name="col">The sprite's color.</param>
        /// <param name="offs">The sprite's offsets.</param>
        /// <param name="rot">The sprite's rotation.</param>
        /// <param name="ang">The sprite's angle.</param>
        /// <param name="pitch">The sprite's pitch.</param>
        /// <param name="scale">The sprite's scale.</param>
        /// <exception cref="ArgumentNullException">Tex is null.</exception>
        public Sprite (Texture tex, Color col, Vector2 offs, double rot, double ang, double pitch, Vector2 scale) : this (tex, col, offs, rot, ang, pitch, scale, Billboarding.X) { }
        /// <summary>
        /// Creates a new Sprite.
        /// </summary>
        /// <param name="tex">The texture to use.</param>
        /// <param name="col">The sprite's color.</param>
        /// <param name="offs">The sprite's offsets.</param>
        /// <param name="rot">The sprite's rotation.</param>
        /// <param name="ang">The sprite's angle.</param>
        /// <param name="pitch">The sprite's pitch.</param>
        /// <param name="scale">The sprite's scale.</param>
        /// <param name="bb">The sprite's billboarding mode.</param>
        /// <exception cref="ArgumentNullException">Tex is null.</exception>
        public Sprite (Texture tex, Color col, Vector2 offs, double rot, double ang, double pitch, Vector2 scale, Billboarding bb) {
            if (tex == null)
                throw new ArgumentNullException ("tex", "The texture of a Sprite cannot be null. Use Sprite.EmptySprite instead!");

            this.Texture = tex;
            this.Color = col;
            this.Offset = offs;
            this.Rotation = rot;
            this.Angle = ang;
            this.Pitch = pitch;
            this.Scale = scale;
            this.Billboard = bb;
        }
    }
}
