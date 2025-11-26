using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace openTKWindowSpace
{
    public class openGLRenderer : GameWindow
    {
        private int _vertexArrayObject;
        private int _vertexBufferObject;
        private int _elementBufferObject;
        private int _normalBufferObject;
        private int _uvBufferObject;
        private int _shaderProgram;
        private int _indiceCount;


        //Camera and transformation
        private Matrix4 _viewMatrix;
        private Matrix4 _projectionMatrix;
        private float _cameraDistance = 35f; // how far cam is from mesh
        private float _cameraHeight = 0f; // how high cam is 
        private float _meshRotationX = 0f;
        private float _meshRotationY = 0f;
        private float _meshRotationZ = 0f;

        // Shader source code
        public static readonly string VertexShaderSource = @"
                    #version 410 core
                    layout (location = 0) in vec3 aPosition;
                    layout (location = 1) in vec3 aNormal;
                    layout (location = 2) in vec2 aTexCoord;

                    out vec3 vertexNormal;
                    out vec2 texCoord;

                    uniform mat4 model;
                    uniform mat4 view;
                    uniform mat4 projection;

                    void main(void)
                    {
                        gl_Position = projection * view * model * vec4(aPosition, 1.0);
                        vertexNormal = aNormal;
                        texCoord = aTexCoord;
                    }
                ";
        public static readonly string FragmentShaderSource = @"
            #version 410 core
            in vec3 vertexNormal;
            in vec2 texCoord;

            out vec4 FragColor;

            void main(void)
            {
                // Simple lighting based on normals
                vec3 norm = normalize(vertexNormal);
                float brightness = max(dot(norm, vec3(0.5, 1.0, 0.5)), 0.3);
                
                FragColor = vec4(brightness * vec3(0.8, 0.9, 0.7), 1.0);
            }
        ";

        public openGLRenderer(int width, int height, string title) 
            : base(GameWindowSettings.Default, 
                new NativeWindowSettings()
                {
                    Size = (width, height),
                    Title = title
                })
        {
        }

        // Called when window first loads. Loads in OpenGL settings
        protected override void OnLoad()
        {
            base.OnLoad();

            // Sets bg color
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);

            // Enable depth testing for 3D rendering
            GL.Enable(EnableCap.DepthTest);

            // Set up Camera matrices
            _viewMatrix = Matrix4.LookAt(
                new Vector3(0,5,10), // Camera Position
                new Vector3(0,0,0), // Look at point (center of mesh)
                new Vector3(0,1,0) // Up
            );

            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
                MathF.PI / 4,               // Field of view (45 degrees)
                (float)Size.X / Size.Y,     // Aspect ratio
                0.1f,                       // Near plane
                100f                        // Far plane
            );

            Console.WriteLine("OpenGL loaded successfully");
            Console.WriteLine($"OpenGL Version: {GL.GetString(StringName.Version)}");
        }

        // Load Mesh Data: CHECK OVER
         public void LoadMeshData(float[] vertices, uint[] indices, float[] normals, float[] uvs)
        {
            try
            {
                _indiceCount = indices.Length;

                // Create Vertex Array Object
                _vertexArrayObject = GL.GenVertexArray();
                GL.BindVertexArray(_vertexArrayObject);

                // Create Vertex Buffer Object for vertices
                _vertexBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
                GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

                // Vertex attribute pointer for position (3 floats per vertex)
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                GL.EnableVertexAttribArray(0);

                // Create Normal Buffer Object
                if (normals != null && normals.Length > 0)
                {
                    _normalBufferObject = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _normalBufferObject);
                    GL.BufferData(BufferTarget.ArrayBuffer, normals.Length * sizeof(float), normals, BufferUsageHint.StaticDraw);

                    // Vertex attribute pointer for normals (3 floats per vertex)
                    GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
                    GL.EnableVertexAttribArray(1);
                }

                // Create UV Buffer Object
                if (uvs != null && uvs.Length > 0)
                {
                    _uvBufferObject = GL.GenBuffer();
                    GL.BindBuffer(BufferTarget.ArrayBuffer, _uvBufferObject);
                    GL.BufferData(BufferTarget.ArrayBuffer, uvs.Length * sizeof(float), uvs, BufferUsageHint.StaticDraw);

                    // Vertex attribute pointer for UVs (2 floats per vertex)
                    GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
                    GL.EnableVertexAttribArray(2);
                }

                // Create Element Buffer Object for indices
                _elementBufferObject = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
                GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindVertexArray(0);

                Console.WriteLine($"Mesh data loaded: {vertices.Length / 3} vertices, {indices.Length / 3} triangles");



                Console.WriteLine($"Mesh data loaded: {vertices.Length / 3} vertices, {indices.Length / 3} triangles");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR loading mesh data: {ex.Message}");
            }
        }

        // Compiles Shader programs: CHECK OVER
        public void CompileShaders(string vertexShaderSource, string fragmentShaderSource)
        {
            try
            {
                // Compile vertex shader
                int vertexShader = GL.CreateShader(ShaderType.VertexShader);
                GL.ShaderSource(vertexShader, vertexShaderSource);
                GL.CompileShader(vertexShader);

                GL.GetShader(vertexShader, ShaderParameter.CompileStatus, out int vertexSuccess);
                if (vertexSuccess == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(vertexShader);
                    Console.WriteLine($"Vertex shader compilation failed: {infoLog}");
                }

                // Compile fragment shader
                int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
                GL.ShaderSource(fragmentShader, fragmentShaderSource);
                GL.CompileShader(fragmentShader);

                GL.GetShader(fragmentShader, ShaderParameter.CompileStatus, out int fragmentSuccess);
                if (fragmentSuccess == 0)
                {
                    string infoLog = GL.GetShaderInfoLog(fragmentShader);
                    Console.WriteLine($"Fragment shader compilation failed: {infoLog}");
                }

                // Link shaders into program
                _shaderProgram = GL.CreateProgram();
                GL.AttachShader(_shaderProgram, vertexShader);
                GL.AttachShader(_shaderProgram, fragmentShader);
                GL.LinkProgram(_shaderProgram);

                GL.GetProgram(_shaderProgram, GetProgramParameterName.LinkStatus, out int linkSuccess);
                if (linkSuccess == 0)
                {
                    string infoLog = GL.GetProgramInfoLog(_shaderProgram);
                    Console.WriteLine($"Shader program linking failed: {infoLog}");
                }

                // Clean up
                GL.DeleteShader(vertexShader);
                GL.DeleteShader(fragmentShader);

                Console.WriteLine("Shaders compiled and linked successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR compiling shaders: {ex.Message}");
            }
        }

        // Called when rendering a frame: CHECK OVER
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Clear the screen
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Use shader program
            if (_shaderProgram != 0)
            {
                GL.UseProgram(_shaderProgram);

                // setup transformation matrices
                Matrix4 modelMatrix = Matrix4.Identity;

                // Pass matrices to shader
                int modelLoc = GL.GetUniformLocation(_shaderProgram, "model");
                int viewLoc = GL.GetUniformLocation(_shaderProgram, "view");
                int projLoc = GL.GetUniformLocation(_shaderProgram, "projection");

                GL.UniformMatrix4(modelLoc, false, ref modelMatrix);
                GL.UniformMatrix4(viewLoc, false, ref _viewMatrix);
                GL.UniformMatrix4(projLoc, false, ref _projectionMatrix);    
            }

            // Render mesh
            if (_vertexArrayObject != 0 && _indiceCount > 0)
            {
                GL.BindVertexArray(_vertexArrayObject);
                GL.DrawElements(PrimitiveType.Triangles, _indiceCount, DrawElementsType.UnsignedInt, 0);
            }

            // Swap buffers
            SwapBuffers();
        }

        // Called every frame to handle input
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            var input = KeyboardState;
            if (input.IsKeyDown(OpenTK.Windowing.GraphicsLibraryFramework.Keys.Q))
            {
                Close();
            }

            // Update Camera Position 
            _viewMatrix = Matrix4.LookAt(
                new Vector3(0, _cameraHeight, _cameraDistance),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 0)
            );
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            // Adjust camera distance with scroll wheel
            _cameraDistance -= e.OffsetY * 0.5f;

            // Clamp distance to prevent going too close or too far
            _cameraDistance = Math.Clamp(_cameraDistance, 2f, 100f);

            Console.WriteLine($"Camera Distance: {_cameraDistance:F2}");
        }

        // rotate the mesh
        public void RotateMesh(float rotationX, float rotationY, float rotationZ)
        {
            _meshRotationX = rotationX;
            _meshRotationY = rotationY;
            _meshRotationZ = rotationZ;
        }

        // Resizes window: CHECK OVER
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Size.X, Size.Y);
        }

        // Cleans up resources: CHECK OVER
        protected override void OnUnload()
        {
            base.OnUnload();

            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteBuffer(_elementBufferObject);
            if (_normalBufferObject != 0) GL.DeleteBuffer(_normalBufferObject);
            if (_uvBufferObject != 0) GL.DeleteBuffer(_uvBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
            GL.DeleteProgram(_shaderProgram);

            Console.WriteLine("OpenGL resources cleaned up");
        }

    }
}