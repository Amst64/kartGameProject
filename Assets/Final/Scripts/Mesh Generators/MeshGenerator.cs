using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyMathTools;

public class MeshGenerator : MonoBehaviour
{
    delegate float ComputeHeightXDelagate(float kX);
    delegate float ComputeValueDelegate(float kX, float kZ);
    delegate Vector3 ComputeVertexPos(float k1, float k2);

    MeshFilter m_Mf;

    [SerializeField] AnimationCurve m_GlassProfile;
    [SerializeField] Texture2D m_HeightMap;

    private void Awake()
    {
        Vector3 halfSize = new Vector3(4, 0, 2);
        m_Mf = GetComponent<MeshFilter>();
        m_Mf.sharedMesh = GenerateStripCorrige(new Vector3(4,0,2), 1000, k=>.125f*Mathf.Sin(k*Mathf.PI*2*4));
        /*m_Mf.sharedMesh = GeneratePlane(new Vector3(4, 0, 2), 20, 10, (kX, kZ) => CoordConvert.CylindricalToCartesian(new Cylindrical(1,kX*2*Mathf.PI
                                                                                ,kZ)));*/ //cylindre

        /* m_Mf.sharedMesh = GeneratePlane( 60, 30, (kX, kZ) => CoordConvert.SphericalToCartesian(new Spherical(1, kX*Mathf.PI*2
                                                                                  , kZ*Mathf.PI*2)));*/ //sphere pas ouf, trop de triangle aux pôles

        //gameObject.AddComponent<MeshCollider>();

        

        m_Mf.sharedMesh = GenerateTerrainFromHeightFunction(500, 500, new Vector3(10, 3, 10),
            (kX, kZ) => m_HeightMap.GetPixel((int)(kX * m_HeightMap.width), (int)(kZ * m_HeightMap.height)).grayscale);

    }

    Mesh GenerateTriangle()
    {
        Mesh mesh = new Mesh();
        mesh.name = "triangle";

        Vector3[] vertices = new Vector3[3];

        //index buffer
        int[] triangles = new int[3];

        vertices[0] = new Vector3(1, 0, 0);
        vertices[1] = new Vector3(0, 1, 0);
        vertices[2] = new Vector3(0, 0, 1);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
    }

    Mesh GenerateQuad(Vector3 size)
    {
        Mesh mesh = new Mesh();
        mesh.name = "quad";

        Vector3[] vertices = new Vector3[4];

        //index buffer
        int[] triangles = new int[6];

        vertices[0] = new Vector3(-size.x * .5f, 0, size.z * .5f);
        vertices[1] = new Vector3(size.x *.5f, 0, size.z * .5f);
        vertices[2] = new Vector3(-size.x * .5f, 0, -size.z * .5f);
        vertices[3] = new Vector3(size.x * .5f, 0, -size.z * .5f);

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;

        triangles[3] = 1;
        triangles[4] = 3;
        triangles[5] = 2;

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        return mesh;
    }


    Mesh GenerateStripCorrige(Vector3 size, int nSegmentsX, ComputeHeightXDelagate heightFunction)
    {
        Mesh mesh = new Mesh();
        mesh.name = "strip";

        Vector3 halfSize = size * .5f;

        Vector3[] vertices = new Vector3[2 * nSegmentsX + 2];

        //index buffer
        int[] triangles = new int[2 * 3 * nSegmentsX];

        Vector2[] uv = new Vector2[vertices.Length];

        //Vector3[] normals = new Vector3[vertices.Length];
        //on va de bas en haut
        //Vertices
        for (int i = 0; i < nSegmentsX + 1; i++)
        {
            float k = (float)i / nSegmentsX; //ratio

            float y = heightFunction(k);
            vertices[i] = Vector3.Lerp(new Vector3(-halfSize.x,y,-halfSize.z),new Vector3(halfSize.x,y,-halfSize.z),k); //bottom line
            vertices[i + nSegmentsX +1] = Vector3.Lerp(new Vector3(-halfSize.x, y, halfSize.z), new Vector3(halfSize.x, y, halfSize.z), k); //top line

            uv[i] = new Vector2(k,0);
            uv[i + nSegmentsX + 1] = new Vector2(k, 1);

            /*normals[i] = Vector3.up;
            normals[i + nSegmentsX + 1] = Vector3.up;*/ 
        }

        //triangles
        int index = 0;
        for (int i = 1; i < nSegmentsX; i++)
        {
            //1st triangle of segment
            triangles[index++] = i;
            triangles[index++] = i + nSegmentsX + 1;
            triangles[index++] = i + nSegmentsX + 1 + 1;

            //2nd triangle of segment
            triangles[index++] = i;
            triangles[index++] = i + nSegmentsX + 1 + 1;
            triangles[index++] = i + 1;

        }



        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.normals = normals;
        mesh.uv = uv;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }


    Mesh GeneratePlane(int nSegmentsX, int nSegmentsZ, ComputeVertexPos posFunction)
    {
        Mesh mesh = new Mesh();
        mesh.name = "3DWrappedPlaneObject";

        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        //Vector3 halfSize = size * .5f;

        Vector3[] vertices = new Vector3[(nSegmentsX+1)*(nSegmentsZ+1)];

        //index buffer
        int[] triangles = new int[2 * 3 * nSegmentsX * nSegmentsZ];

        Vector2[] uv = new Vector2[vertices.Length];

        //Vector3[] normals = new Vector3[vertices.Length];

        //Vertices
        for (int i = 0; i < nSegmentsX +1; i++)
        {
            float kX = (float)i / nSegmentsX; //ratio
            //Vector3 xPas = Vector3.Lerp(new Vector3(-halfSize.x, 0, 0), new Vector3(halfSize.x, 0, 0), kX);
            int indexOffset = i * (nSegmentsZ + 1);

            for (int j = 0; j< nSegmentsZ +1; j++)
            {
                float kZ = (float)j / nSegmentsZ; //ratio

                //float y = heightFunction(kX,kZ);
                vertices[indexOffset + j] = posFunction(kX,kZ); //new Vector3(Mathf.Lerp(-halfSize.x, halfSize.x,kX),y ,Mathf.Lerp(-halfSize.z, halfSize.z,kZ));
                //vertices[j+nSegmentsZ+1] = Vector3.Lerp(new Vector3(-halfSize.x * kX, y, halfSize.z), new Vector3(-halfSize.x * kX, y, -halfSize.z), kZ);

                uv[indexOffset+j] = new Vector2(kX, kZ);

            }
        }

        //triangles
        int index = 0;
        for (int i = 0; i < nSegmentsX; i++)
        {
            int indexOffset = i * (nSegmentsZ + 1);

            for(int j = 0; j<nSegmentsZ; j++)
            {
                //1st triangle of segment
                triangles[index++] = indexOffset+j;
                triangles[index++] = indexOffset + j + 1;
                triangles[index++] = indexOffset+j+1+nSegmentsZ+1;

                //2nd triangle of segment
                triangles[index++] = indexOffset + j;
                triangles[index++] = indexOffset + j + 1 + nSegmentsZ + 1;
                triangles[index++] = indexOffset + j + nSegmentsZ + 1;
            }
            

        }



        mesh.vertices = vertices;
        mesh.triangles = triangles;
        //mesh.normals = normals;
        mesh.uv = uv;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    Mesh GenerateCylinder(int nSegmentsTheta, int nSegmentsY, float radius, float height, ComputeValueDelegate rhoFunc) //rhoFunc pour dessiner une forme exemple un verre
    {
        ComputeVertexPos cylFunc = (kX, kZ) =>
                                CoordConvert.CylindricalToCartesian(new Cylindrical(radius * rhoFunc(kX, kZ), kX * Mathf.PI * 2, height * kZ));
        return GeneratePlane(nSegmentsTheta, nSegmentsY, cylFunc);
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

    /*private void Update()
    {
        m_Mf.sharedMesh = GenerateCylinder(40, 10, 2, 6, (kx, kZ) => m_GlassProfile.Evaluate(kZ));
    }*/

}
