using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Mesh2D : ScriptableObject {
    
    [System.Serializable]
    public class Vertex {
        public Vector2[] point;
        public Vector2[] normal;
        public float[] u; // UVs but no V -- V will be generated
        // vertex color 
        // ... and more
    }

    public Vertex[] vertices;
    public int[] lineIndices;



}
