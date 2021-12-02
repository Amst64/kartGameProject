using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    delegate float ComputeHeigthXDelegate(float kX);
    delegate float ComputeValueDelegate(float kX, float kZ);
    delegate Vector3 ComputeVertexPos(float k1, float k2);

    MeshFilter m_Mf;

    [SerializeField] Texture2D m_HeightMap;
    [SerializeField] int SizeX;
    [SerializeField] int SizeZ;
    [SerializeField] 

    private void Awake()
    {

        SizeX = 100;
        SizeZ = 100;
        m_Mf = GetComponent<MeshFilter>();

        m_Mf.sharedMesh = GenerateTerrainFromHeightFunction(SizeX, SizeZ, new Vector3(1, 1, 1),
            (kX, kZ) => m_HeightMap.GetPixel((int)(kX * m_HeightMap.width), (int)(kZ * m_HeightMap.height)).grayscale
            );

        gameObject.AddComponent<MeshCollider>();
    }

    Mesh GenerateTerrainFromHeightFunction(int nSegmentsX, int nSegmentsZ, Vector3 size, ComputeValueDelegate heightFunc)
    {
        Vector3 halfSize = size * .5f;
        ComputeVertexPos terrainFunc = (kX, kZ) => new Vector3(
                            Mathf.Lerp(-halfSize.x, halfSize.x, kX),
                            size.y * heightFunc(kX, kZ),
                            Mathf.Lerp(-halfSize.z, halfSize.z, kZ));
        return GeneratePlane(nSegmentsX, nSegmentsZ, terrainFunc);
    }

    Mesh GeneratePlane(int nSegmentsX, int nSegmentsZ, ComputeVertexPos posFunction)
    {
        Mesh mesh = new Mesh();
        mesh.name = "3DWrappedPlaneObject";

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        Vector3[] vertices = new Vector3[(nSegmentsX + 1) * (nSegmentsZ + 1)];
        int[] triangles = new int[2 * nSegmentsX * nSegmentsZ * 3];
        Vector2[] uv = new Vector2[vertices.Length];

        //vertices
        for (int i = 0; i < nSegmentsX + 1; i++)
        {
            float kX = (float)i / nSegmentsX; // ratio
            int indexOffset = i * (nSegmentsZ + 1);

            for (int j = 0; j < nSegmentsZ + 1; j++)
            {
                float kZ = (float)j / nSegmentsZ; // ratio

                vertices[indexOffset + j] = posFunction(kX, kZ);

                uv[indexOffset + j] = new Vector2(kX, kZ);
            }
        }

        //triangles
        int index = 0;
        for (int i = 0; i < nSegmentsX; i++)
        {
            int indexOffset = (nSegmentsZ + 1) * i;
            for (int j = 0; j < nSegmentsZ; j++)
            {
                // 1st triangle of segment
                triangles[index++] = indexOffset + j;
                triangles[index++] = indexOffset + j + 1;
                triangles[index++] = indexOffset + j + 1 + nSegmentsZ + 1;

                //2nd triangle of segment
                triangles[index++] = indexOffset + j;
                triangles[index++] = indexOffset + j + 1 + nSegmentsZ + 1;
                triangles[index++] = indexOffset + j + nSegmentsZ + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }
}
