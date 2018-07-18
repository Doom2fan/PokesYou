using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PokesYou.G_Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using OpenTK;
using PokesYou.Data.Managers;

namespace PokesYou.Renderer.OpenTk.Internal {
    class Shader : IDisposable {
        public int Id { get; protected set; }

        private string ReadShaderCode (string path) {
            string codeStr;

            var codeLump = LumpManager.GetLumpFullPath (path);
            if (codeLump == null)
                throw new FileNotFoundException (String.Format ("Unable to load \"{0}\"", path));

            using (var stream = codeLump.AsStream) {
                byte [] buffer = new byte [stream.Length];

                stream.Seek (0, SeekOrigin.Begin);
                stream.Read (buffer, 0, (int) stream.Length);
                codeStr = Encoding.ASCII.GetString (buffer);
            }

            return codeStr;
        }

        public Shader (string vertexPath, string fragPath) {
            string vertCode;
            string fragCode;
            
            string inclCode;

            inclCode = ReadShaderCode (@"Shaders\GLSL\shaderdefs.i");
            vertCode = ReadShaderCode (vertexPath);
            fragCode = ReadShaderCode (fragPath);

            inclCode = String.Format ("#version {0} core\n{1}\n#line 0\n", 330, inclCode);
            vertCode = String.Format ("{0}{1}", inclCode, vertCode);
            fragCode = String.Format ("{0}{1}", inclCode, fragCode);
            
            int success;

            int vert = GL.CreateShader (ShaderType.VertexShader);
            GL.ShaderSource (vert, vertCode);
            GL.CompileShader (vert);
            // Print any errors
            GL.GetShader (vert, ShaderParameter.CompileStatus, out success);
            if (success == 0) {
                GConsole.Debug.WriteLine ("Failed to compile vertex shader:\n{0}", GL.GetShaderInfoLog (vert));
            }

            int frag = GL.CreateShader (ShaderType.FragmentShader);
            GL.ShaderSource (frag, fragCode);
            GL.CompileShader (frag);
            // Print any errors
            GL.GetShader (frag, ShaderParameter.CompileStatus, out success);
            if (success == 0) {
                GConsole.Debug.WriteLine ("Failed to compile fragment shader:\n{0}", GL.GetShaderInfoLog (frag));
            }

            Id = GL.CreateProgram ();
            GL.AttachShader (Id, vert);
            GL.AttachShader (Id, frag);
            GL.LinkProgram (Id);
            // Print any linking errors
            GL.GetProgram (Id, GetProgramParameterName.LinkStatus, out success);
            if (success == 0) {
                GConsole.Debug.WriteLine ("Failed to link shader:\n{0}", GL.GetProgramInfoLog (Id));
            }

            GL.DeleteShader (vert);
            GL.DeleteShader (frag);
        }

        public void Use () {
            GL.UseProgram (Id);
        }
        public void Delete () {
            GL.DeleteProgram (Id);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose (bool disposing) {
            if (!disposedValue) {
                this.Delete ();
                disposedValue = true;
            }
        }

        public void Dispose () {
            Dispose (true);
        }
        #endregion

        public void SetBool (string name, bool value) { GL.Uniform1 (GL.GetUniformLocation (Id, name), value ? 1 : 0); }
        public void SetInt (string name, int value) { GL.Uniform1 (GL.GetUniformLocation (Id, name), value); }
        public void SetFloat (string name, float value) { GL.Uniform1 (GL.GetUniformLocation (Id, name), value); }
        public void SetVector2 (string name, Vector2 value) { GL.Uniform2 (GL.GetUniformLocation (Id, name), value); }
        public void SetVector3 (string name, Vector3 value) { GL.Uniform3 (GL.GetUniformLocation (Id, name), value); }
        public void SetVector4 (string name, Vector4 value) { GL.Uniform4 (GL.GetUniformLocation (Id, name), value); }
        public void SetVector2 (string name, float x, float y)                   { GL.Uniform2 (GL.GetUniformLocation (Id, name), x, y); }
        public void SetVector3 (string name, float x, float y, float z)          { GL.Uniform3 (GL.GetUniformLocation (Id, name), x, y, z); }
        public void SetVector4 (string name, float x, float y, float z, float w) { GL.Uniform4 (GL.GetUniformLocation (Id, name), x, y, z, w); }
        public void SetMatrix (string name, Matrix2 value, bool transpose = false) { GL.UniformMatrix2 (GL.GetUniformLocation (Id, name), transpose, ref value); }
        public void SetMatrix (string name, Matrix3 value, bool transpose = false) { GL.UniformMatrix3 (GL.GetUniformLocation (Id, name), transpose, ref value); }
        public void SetMatrix (string name, Matrix4 value, bool transpose = false) { GL.UniformMatrix4 (GL.GetUniformLocation (Id, name), transpose, ref value); }
    }
}
