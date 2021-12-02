using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyMathTools;


public class CylinderGenerator : MonoBehaviour
{
    delegate float ComputeHeigthXDelegate(float kX);
    delegate float ComputeValueDelegate(float kX, float kZ);
    delegate Vector3 ComputeVertexPos(float k1, float k2);

    MeshFilter m_Mf;

    [SerializeField] AnimationCurve m_GlassProfile;
    int nSegmentsTheta;
    int nSegmentsY;
    float radius;
    float height;


    private void Awake()
    {

        nSegmentsTheta = 15;
        nSegmentsY = 40;
        radius = 1;
        height = 50;

        m_Mf = GetComponent<MeshFilter>();
        m_Mf.sharedMesh = GenerateCylinder(nSegmentsTheta, nSegmentsY, radius, height, (kx,kZ)=>m_GlassProfile.Evaluate(kZ));
        gameObject.AddComponent<MeshCollider>();
    }

    Mesh GenerateCylinder(int nSegmentsTheta, int nSegmentsY, float radius, float height, ComputeValueDelegate rhoFunc)
    {
        ComputeVertexPos cylFunc = (kX, kZ) =>
                                CoordConvert.CylindricalToCartesian(new Cylindrical(radius * rhoFunc(kX, kZ), kX * Mathf.PI * 2, height * kZ));
        return GeneratePlane(nSegmentsTheta, nSegmentsY, cylFunc);
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
