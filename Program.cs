using System;
using ScreenNameSpace;
using openTKWindowSpace;
using System.Windows;
using FBXModelLoaderSpace;
using Assimp;
using System.IO;
using TerrainGeneration;

// using PlayerNameSpace;
// using ApplicationNameSpace;

namespace mainNameSpace
{
    public class MainClass
    {
        [STAThread]
        public static void Main()
        {
            Console.WriteLine("Starting main function: ");
            FBXModelLoader modelLoader = new FBXModelLoader();
            string filePath = @"C:\Users\liamb\OneDrive\Desktop\3DModels\bruh\exports\solarPunkTV.fbx";
            Console.WriteLine($"Looking for file: {filePath}");
            if (System.IO.File.Exists(filePath))
            {
                Console.WriteLine("File found!");
            }
            else
            {
                Console.WriteLine("ERROR: File not found!");
                return;
            }
            
            Scene scene = modelLoader.LoadFBX(filePath);

            if (scene == null)
            {
                Console.WriteLine("Failed to load FBX file");
                return;
            }
            // for logging
            else if (scene != null)
            {
                ProcessModelData(modelLoader, scene);
            }


            // Get Mesh Data
            Mesh mesh = scene.Meshes[0];
            modelLoader.ExtractMeshData(mesh, out float[] vertices, out uint[] indices, out float[] normals);
            modelLoader.ExtractUVData(mesh, out float[] uvs);

            // Create OpenGL render window
            Console.WriteLine("Starting OpenGL window...");
            var renderer = new openGLRenderer(1280, 720, "Game Engine Window");

            // Load fbx data
            renderer.LoadMeshData(vertices, indices, normals, uvs);

            // Load Terrain Data 
            var terrainGen = new TerrainGenerator(500,500, 1.0f); // 50x50 terrain , 1 unit cells
            terrainGen.GenerateFlatTerrain(out float[] terrainVertices, out uint[] terrainIndices, out float[] terrainNormals);
            terrainGen.GenerateUVs(out float[] terrainUVs);
            Console.WriteLine($"Terrain generated: {terrainVertices.Length / 3} verts, {terrainIndices.Length / 3} tris");
            renderer.LoadTerrainData(terrainVertices,terrainIndices, terrainNormals, terrainUVs);


            // Compile Shaders and run
            renderer.CompileShaders(openGLRenderer.VertexShaderSource, openGLRenderer.FragmentShaderSource);
            Console.WriteLine("Shaders Compiled");
            renderer.RotateMesh(0,MathF.PI / 2,0);
            renderer.SetMeshScale(0.1f);

            renderer.SetMeshColor(1.0f, 0.0f, 0.0f);

            renderer.Run();


            // This code executes after the window closes
            modelLoader.Dispose();
            Console.WriteLine("Window Closed");
        }

        static void StoreModelInMemory(int meshIndex, float[] vertices, uint[] indices, float[] normals, float[] uvs)
        {
            Console.WriteLine($"Mesh {meshIndex}: Stored {vertices.Length / 3} vertices, {indices.Length / 3} triangles");
            // TODO: Store this data in a structure for rendering later
        }

        // For mesh logging
        static void ProcessModelData(FBXModelLoader loader, Scene scene)
        {
            using (StreamWriter writer = new StreamWriter("debug_output.txt", append: true))
            {
                DateTime now = DateTime.Now;
                writer.WriteLine($"Scene loaded with {scene.MeshCount} meshes");
                writer.WriteLine(now);
                
                for(int i = 0; i < scene.MeshCount; i++)
                {
                    Mesh mesh = scene.Meshes[i];
                    writer.WriteLine($"Mesh {i}: {mesh.Name}");
                    writer.WriteLine($"  Vertices: {mesh.VertexCount}");
                    writer.WriteLine($"  Faces: {mesh.FaceCount}");
                }
            }
            Console.WriteLine("Debug output written to debug_output.txt");
        }
    }
}

