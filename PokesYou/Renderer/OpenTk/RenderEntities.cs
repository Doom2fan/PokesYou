using OpenTK;
using OpenTK.Graphics.OpenGL;
using PokesYou.CMath;
using PokesYou.Data;
using PokesYou.G_Console;
using PokesYou.Game;
using System;
using System.Drawing;

namespace PokesYou.Renderer.OpenTk.Internal {
    public static class RenderEntities {
        static Vertex [] floorVerts = {
            new Vertex (new Accum (-250),new Accum ( 0),new Accum ( -250)),
            new Vertex (new Accum (-250),new Accum ( 0), new Accum ( 250)),
            new Vertex (new Accum ( 250),new Accum ( 0), new Accum ( 250)),
            new Vertex (new Accum ( 250),new Accum ( 0),new Accum ( -250)),
        };
        // Right wall
        static Vertex [] northWallVerts = {
            new Vertex (new Accum (-250),new Accum ( 50),new Accum ( -250)), new Vertex (new Accum (-250), new Accum ( 0),new Accum ( -250)),
            new Vertex (new Accum ( 250), new Accum ( 0),new Accum ( -250)), new Vertex (new Accum ( 250),new Accum ( 50),new Accum ( -250)),
        };
        // Left wall
        static Vertex [] southWallVerts = {
                new Vertex (new Accum ( 250),new Accum ( 50),new Accum ( 250)), new Vertex (new Accum ( 250), new Accum ( 0),new Accum ( 250)),
                new Vertex (new Accum (-250), new Accum ( 0),new Accum ( 250)), new Vertex (new Accum (-250),new Accum ( 50),new Accum ( 250)),
        };
        // North wall
        static Vertex [] rightWallVerts = {
                new Vertex (new Accum (250),new Accum ( 100),new Accum ( -250)), new Vertex (new Accum (250),  new Accum ( 0),new Accum ( -250)),
                new Vertex (new Accum (250),  new Accum ( 0), new Accum ( 250)), new Vertex (new Accum (250),new Accum ( 100), new Accum ( 250)),
        };
        // South wall
        static Vertex [] leftWallVerts = {
                new Vertex (new Accum (-250),new Accum ( 50), new Accum ( 250)), new Vertex (new Accum (-250), new Accum ( 0), new Accum ( 250)),
                new Vertex (new Accum (-250), new Accum ( 0),new Accum ( -250)), new Vertex (new Accum (-250),new Accum ( 50),new Accum ( -250)),
        };
        public static void ProcessGeometry (this GameWindow window) {
            GL.Disable (EnableCap.Texture2D);
            Vector4 ambient = new Vector4 (0.2f, 0.2f, 0.2f, 1.0f);
            Vector4 diffuse = new Vector4 (1.0f, 1.0f, 1.0f, 1.0f);
            Vector4 specular = new Vector4 (0.0f, 0.0f, 0.0f, 1.0f);

            GL.Material (MaterialFace.Front, MaterialParameter.Ambient, ambient);
            GL.Material (MaterialFace.Front, MaterialParameter.Diffuse, diffuse);
            GL.Material (MaterialFace.Front, MaterialParameter.Specular, specular);
            GL.Material (MaterialFace.Front, MaterialParameter.Shininess, 40.0f);

            // Floor
            Vertex [] [] room = { floorVerts, northWallVerts, southWallVerts, rightWallVerts, leftWallVerts };

            GL.Begin (PrimitiveType.Quads);
            for (int i = 0; i < room.Length; i++) {
                if (room [i].Length < 4)
                    continue;

                Vertex [] verts = room [i];
                Vector3k normal = MathUtils.CalculateSurfaceNormal (verts);
                GL.Normal3 (normal.ToVec3d ());
                for (int v = 0; v < 4; v++)
                    GL.Vertex3 (verts [v].Pos.ToVec3 ());
            }
            GL.End ();
        }

        public static void ProcessActors (this OpenTkRenderer window) {
            foreach (Actor actor in Core.Ticker.Thinkers) {
                if (actor == null || actor.State == null || window.localCamera.Owner == actor)
                    return;
                if (actor.State.RotationSet == null)
                    return;

                double angle = MathHelper.RadiansToDegrees (Math.Atan2 ((double) (window.localCamera.Position.X - actor.Position.X), (double) (window.localCamera.Position.Y - actor.Position.Y)));
                Sprite spr = actor.State.RotationSet.GetSprite (angle);

                if (spr == null)
                    continue;

                window.DrawSprite (spr, actor.Position.ToVec3d ());
            }
        }

        public static void DrawSprite (this OpenTkRenderer window, Sprite sprite, Vector3d pos) {
            double width, height;
            width = sprite.Texture.Width * sprite.Scale.X; height = sprite.Texture.Height * sprite.Scale.Y;

            GL.PushMatrix ();
            Vector3d origin = pos.ToGLVec3d ();
            origin.Y -= ((Math.Sign (origin.Y) == -1) ? -height : height) / 2;
            origin.Z -= ((Math.Sign (origin.Z) == -1) ? -width : width) / 2;
            GL.Translate (origin);
            GL.Translate (0.0, sprite.Offset.Y * sprite.Scale.Y, sprite.Offset.X * sprite.Scale.X);

            if (sprite.Billboard.HasFlag (Billboarding.X) || sprite.Billboard.HasFlag (Billboarding.Y)) {
                Matrix4d mat;
                GL.GetDouble (GetPName.ModelviewMatrix, out mat);

                if (sprite.Billboard.HasFlag (Billboarding.X)) { // Do X billboarding
                    mat [0, 0] = Matrix4d.Identity [0, 2];
                    mat [0, 1] = Matrix4d.Identity [0, 1];
                    mat [0, 2] = Matrix4d.Identity [0, 0];
                    mat [2, 0] = Matrix4d.Identity [2, 2];
                    mat [2, 1] = Matrix4d.Identity [2, 1];
                    mat [2, 2] = Matrix4d.Identity [2, 0];
                }
                if (sprite.Billboard.HasFlag (Billboarding.Y)) { // Do Y billboarding
                    mat [1, 0] = Matrix4d.Identity [1, 2];
                    mat [1, 1] = Matrix4d.Identity [1, 1];
                    mat [1, 2] = Matrix4d.Identity [1, 0];
                }

                GL.LoadMatrix (ref mat);
            }

            GL.Rotate (sprite.Rotation, Vector3d.UnitX); // Do rotations
            GL.Rotate (sprite.Angle, Vector3d.UnitY);
            GL.Rotate (sprite.Pitch, Vector3d.UnitZ);

            GLTex tex = GLTexManager.GetOrAddTex (sprite.Texture);
            GL.Enable (EnableCap.Texture2D);
            GL.BindTexture (TextureTarget.Texture2D, tex.Id);

            Vector4 ambient = new Vector4 (0.2f, 0.2f, 0.2f, 1.0f);
            Vector4 diffuse = new Vector4 (1.0f, 1.0f, 1.0f, 1.0f);
            Vector4 specular = new Vector4 (0.0f, 0.0f, 0.0f, 1.0f);

            GL.Material (MaterialFace.Front, MaterialParameter.Ambient, ambient);
            GL.Material (MaterialFace.Front, MaterialParameter.Diffuse, diffuse);
            GL.Material (MaterialFace.Front, MaterialParameter.Specular, specular);
            GL.Material (MaterialFace.Front, MaterialParameter.Shininess, 40.0f);

            GL.Begin (PrimitiveType.Quads);
            GL.Normal3 (0.0f, 0.0f, 1.0f);
            GL.TexCoord2 (0, 0); GL.Vertex3 (0.0,  height / 2, -width / 2);
            GL.TexCoord2 (0, 1); GL.Vertex3 (0.0, -height / 2, -width / 2);
            GL.TexCoord2 (1, 1); GL.Vertex3 (0.0, -height / 2,  width / 2);
            GL.TexCoord2 (1, 0); GL.Vertex3 (0.0,  height / 2,  width / 2);

            if (!sprite.Billboard.HasFlag (Billboarding.X)) { // If X billboarding is off, draw on the other side too
                GL.TexCoord2 (1, 0); GL.Vertex3 (0.0,  height / 2, -width / 2);
                GL.TexCoord2 (0, 0); GL.Vertex3 (0.0,  height / 2,  width / 2);
                GL.TexCoord2 (0, 1); GL.Vertex3 (0.0, -height / 2,  width / 2);
                GL.TexCoord2 (1, 1); GL.Vertex3 (0.0, -height / 2, -width / 2);
            }
            GL.End ();

            GL.PopMatrix ();
        }
    }
}
