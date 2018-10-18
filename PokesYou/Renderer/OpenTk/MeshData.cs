using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace PokesYou.Renderer.OpenTk.Internal {
    [StructLayout (LayoutKind.Sequential, Pack=0)]
    struct MeshVertex {
        public Vector3 Position;
        public Vector3 Normal;
        public Vector2 TexCoords;
    }

    struct MeshTex {
        public GLTex Tex;
        public string Type;
        public string Path;
    }

    class MeshData {
        public MeshVertex [] Vertices;
        public uint [] Indices;
        public List<MeshTex> Textures;
        public MeshData (MeshVertex [] vertices, uint [] indices, List<MeshTex> textures) {
            this.Vertices = vertices;
            this.Indices  = indices;
            this.Textures = textures;

            SetupMesh ();
        }
        public void Draw (Shader shader) {
            uint diffuseN = 1;
            uint specularN = 1;

            for (int i = 0; i < Textures.Count; i++) {
                GL.ActiveTexture (TextureUnit.Texture0 + i);
                string number = String.Empty, name = Textures [i].Type;

                if (name.Equals ("matDiffuse", StringComparison.CurrentCulture))
                    number = (diffuseN++).ToString ();
                if (name.Equals ("matSpecular", StringComparison.CurrentCulture))
                    number = (specularN++).ToString ();

                shader.SetFloat (String.Format ("{0}{1}", name, number), i);
                Textures [i].Tex.Bind ();
            }
            GL.ActiveTexture (TextureUnit.Texture0);

            // Draw mesh
            GL.BindVertexArray (VAO);
            GL.DrawElements (BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray (0);
        }

        private int VAO, VBO, EBO;
        private void SetupMesh () {
            int vertexSize = Marshal.SizeOf (typeof (MeshVertex));

            VAO = GL.GenVertexArray ();
            VBO = GL.GenBuffer ();
            EBO = GL.GenBuffer ();

            // Bind the vertex array object
            GL.BindVertexArray (VAO);
            // Bind and fill the vertex buffer object
            GL.BindBuffer (BufferTarget.ArrayBuffer, VBO);
            GL.BufferData (BufferTarget.ArrayBuffer, new IntPtr (Vertices.Length * vertexSize), Vertices, BufferUsageHint.StaticDraw);
            // Bind and fill the element buffer object
            GL.BindBuffer (BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData (BufferTarget.ElementArrayBuffer, new IntPtr (Indices.Length * sizeof (uint)), Indices, BufferUsageHint.StaticDraw);

            // Vertex positions
            GL.EnableVertexAttribArray (0);
            GL.VertexAttribPointer (0, 3, VertexAttribPointerType.Float, false, vertexSize, 0);
            // Vertex normals
            GL.EnableVertexAttribArray (1);
            GL.VertexAttribPointer (1, 3, VertexAttribPointerType.Float, false, vertexSize, Marshal.OffsetOf (typeof (MeshVertex), "Normal"));
            // Texture coordinates
            GL.EnableVertexAttribArray (2);
            GL.VertexAttribPointer (2, 2, VertexAttribPointerType.Float, false, vertexSize, Marshal.OffsetOf (typeof (MeshVertex), "TexCoords"));

            GL.BindVertexArray (0);
        }
    }
}
