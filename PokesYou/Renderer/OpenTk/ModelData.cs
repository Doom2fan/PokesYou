using Assimp;
using OpenTK;
using PokesYou.Data;
using PokesYou.Data.Managers;
using PokesYou.G_Console;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PokesYou.Renderer.OpenTk.Internal {
    class Model {
        private Model () { } // Disable parameterless constructor

        public static Model LoadModel (string path) {
            if (path == null)
                throw new ArgumentNullException ("path", "Path must not be null");

            var mdl = new Model ();

            Scene scene;
            var lmp = LumpManager.GetLumpFullPath (path);
            using (Stream stream = lmp.AsStream) {
                using (AssimpContext context = new AssimpContext ()) {
                    LumpIOSystem ioSys = new LumpIOSystem ();
                    context.SetIOSystem (ioSys);
                    scene = context.ImportFileFromStream (stream, PostProcessSteps.Triangulate | PostProcessSteps.FlipUVs);

                    if (scene == null || scene.SceneFlags.HasFlag (SceneFlags.Incomplete) || scene.RootNode == null)
                        throw new Core.FatalError (String.Format ("Error loading model \"{0}\"", path));
                }
            }
            mdl.directory = Path.GetDirectoryName (path);

            mdl.meshes = new List<MeshData> (scene.MeshCount);
            mdl.ProcessNode (scene.RootNode, scene);

            return mdl;
        }

        public void Draw (Shader shader) {
            shader.Use ();
            for (int i = 0; i < meshes.Count; i++)
                meshes [i].Draw (shader);
        }

        /* Model data */
        private List<MeshData> meshes;
        private string directory;
        /* Functions */
        private void ProcessNode (Node node, Scene scene) {
            // Process all of the node's meshes
            for (int i = 0; i < node.MeshCount; i++) {
                var mesh = scene.Meshes [node.MeshIndices [i]];
                meshes.Add (ProcessMesh (mesh, scene));
            }

            // Process all of the node's meshes
            for (int i = 0; i < node.ChildCount; i++)
                ProcessNode (node.Children [i], scene);
        }

        private MeshData ProcessMesh (Mesh mesh, Scene scene) {
            List<MeshVertex> vertices = new List<MeshVertex> (mesh.VertexCount);
            List<uint> indices = new List<uint> ();
            List<MeshTex> textures = new List<MeshTex> ();

            // Process vertices
            for (int i = 0; i < mesh.VertexCount; i++) {
                var vertex = new MeshVertex ();

                vertex.Position = new Vector3 (mesh.Vertices [i].X, mesh.Vertices [i].Y, mesh.Vertices [i].Z);
                vertex.Normal = new Vector3 (mesh.Normals [i].X, mesh.Normals [i].Y, mesh.Normals [i].Z);
                
                if (mesh.HasTextureCoords (0))
                    vertex.TexCoords = new Vector2 (mesh.TextureCoordinateChannels [0] [i].X, mesh.TextureCoordinateChannels [0] [i].Y);
                else
                    vertex.TexCoords = new Vector2 (0f, 0f);

                vertices.Add (vertex);
            }

            // Process indices
            for (int i = 0; i < mesh.FaceCount; i++) {
                var face = mesh.Faces [i];
                for (int j = 0; j < face.IndexCount; j++)
                    indices.Add ((uint) face.Indices [j]);
            }

            // Process material
            if (mesh.MaterialIndex >= 0) {
                var material = scene.Materials [mesh.MaterialIndex];

                List<MeshTex> diffMaps = LoadMaterialTextures (material, TextureType.Diffuse, "matDiffuse");
                textures.AddRange (diffMaps);
                List<MeshTex> specMaps = LoadMaterialTextures (material, TextureType.Specular, "matSpecular");
                textures.AddRange (specMaps);
            }

            return new MeshData (vertices.ToArray (), indices.ToArray (), textures);
        }

        private List<MeshTex> LoadMaterialTextures (Material mat, TextureType type, string typeName) {
            List<MeshTex> textures = new List<MeshTex> (mat.GetMaterialTextureCount (type));

            var texSlots = mat.GetMaterialTextures (type);
            for (int i = 0; i < texSlots.Length; i++) {
                MeshTex tex = new MeshTex ();
                tex.Tex = GLTexManager.GetOrAddTex (TextureManager.LoadTexture (Path.Combine (directory, texSlots [i].FilePath)));
                tex.Type = typeName;
                tex.Path = texSlots [i].FilePath;
                textures.Add (tex);
            }

            return textures;
        }
    }
}
