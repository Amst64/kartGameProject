using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class Mesh2D : ScriptableObject
{
    [System.Serializable]
    public class Vertex
    {
        public Vector2 point;
        public Vector2 normal;
        public float Us; //UVs sans le V car on le genere
    }

    public Vertex[] vertices;
    public int[] lineIndices;
}
