using System;
using OpenTK.Mathematics;

namespace TerrainGeneration
{
    public class TerrainGenerator
    {
        private int _gridWidth;
        private int _gridDepth;
        private float _cellSize;

        public TerrainGenerator(int gridWidth, int gridDepth, float cellSize)
        {
            _gridWidth = gridWidth;
            _gridDepth = gridDepth;
            _cellSize = cellSize;
        }

        public void GenerateFlatTerrain(out float[] vertices, out uint[] indices, out float[] normals)
        {
            int vertexCountX = _gridWidth + 1;
            int vertexCountZ = _gridDepth + 1;
            int totalVertices = vertexCountX * vertexCountZ;


            vertices = new float[totalVertices * 3];

            int vertexIndex = 0;
            for (int z = 0; z < vertexCountZ; z++)
            {
                for (int x = 0; x < vertexCountX; x++)
                {
                    vertices[vertexIndex++] = x * _cellSize;
                    vertices[vertexIndex++] = 0f;
                    vertices[vertexIndex++] =z * _cellSize;
                }
            }

            int triangleCount = _gridWidth * _gridDepth * 2;
            indices = new uint[triangleCount * 3];

            int indexPointer = 0;
            for (int z = 0; z < _gridDepth; z++)
            {
                for (int x = 0; x < _gridWidth; x++)
                {
                    uint topLeft = (uint)(z * vertexCountX + x);
                    uint topRight = topLeft + 1;
                    uint bottomLeft = (uint)((z + 1) * vertexCountX + x);
                    uint bottomRight = bottomLeft + 1;

                    indices[indexPointer++] = topLeft;
                    indices[indexPointer++] = bottomLeft;
                    indices[indexPointer++] = topRight;

                    indices[indexPointer++] = topRight;
                    indices[indexPointer++] = bottomLeft;
                    indices[indexPointer++] = bottomRight;
                }
            }

            normals = new float[totalVertices * 3];
            for (int i = 0; i < totalVertices; i++)
            {
                normals[i * 3] = 0f;
                normals[i * 3 + 1] = 1f;
                normals[i * 3 + 2] = 0f;
            }

            Console.WriteLine($"Generated flat terrain: {totalVertices} vertices, {triangleCount} triangles");
            Console.WriteLine($"Terrain size: {_gridWidth * _cellSize} x {_gridDepth * _cellSize} units");
        }

        public void GenerateUVs(out float[] uvs)
        {
            int vertexCountX = _gridWidth + 1;
            int vertexCountZ = _gridDepth + 1;
            int totalVertices = vertexCountX * vertexCountZ;

            uvs = new float[totalVertices * 2];

            int uvIndex = 0;
            for (int z = 0; z < vertexCountZ; z++)
            {
                for (int x = 0; x < vertexCountX; x++)
                {
                    uvs[uvIndex++] = (float)x / _gridWidth;
                    uvs[uvIndex++] = (float) z / _gridDepth;
                }
            } 
            Console.WriteLine("Generated UV coordinates for terrain");
        }
    }
}