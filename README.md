# LL-Game-Engine-Project-
A game engine project constructed entirely with C# (CURRENTLY IN PROGRESS) 

CODE STRUCTURE:

Window Generation
// using OpenTK to generate window + render .fbx mesh data
- openTKWindow.cs
  openGLRenderer class:
    camera
    shader code
    window size
    bg color
    load mesh data

FBX Interpretation
// using Assimp to interpret .fbx data to be rendered by OpenGL
- fbxInterpretation.cs
  FBXModelLoader class:
    Scene Object
    PostProcessing Steps
    Extract Mesh Data (vertices, indices, normals)
    Extract UV Data
    Logging Mesh data 
