/*
using System.Linq;
using UnityEditor;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class Bezier : MonoBehaviour
{

    [Range(2, 32)]
    [SerializeField]
    int segment;

    [SerializeField]
    Transform pointA;

    [SerializeField]
    Transform pointB;

    [SerializeField]
    Transform pointC;

    [SerializeField]
    Transform pointD;

    [Range(0, 1)]
    [SerializeField]
    float t_ratio;

    static int numPoints = 200;

    [SerializeField]
    Mesh2D shape2D;

    Vector3[] L1_positions = new Vector3[numPoints]; // positions de l'interpolation lineaire des points A à B 
    Vector3[] L2_positions = new Vector3[numPoints]; // positions de l'interpolation lineaire des points B à C
    Vector3[] L3_positions = new Vector3[numPoints]; // positions de l'interpolation lineaire des points C à D

    Vector3[] Q1_positions = new Vector3[numPoints]; // positions de l'interpolation lineaire des points L1 et L2
    Vector3[] Q2_positions = new Vector3[numPoints]; // positions de l'interpolation lineaire des points L2 et L3

    Vector3[] final_positions = new Vector3[numPoints]; // positions de l'interpolation lineaire des points Q1 et Q2


    Mesh mesh;


    private void Awake()
    {
        mesh = new Mesh();
        mesh.name = "Bezier curve mesh";
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    // Update is called once per frame
    void Update()
    {
        
        //DrawLinearCurve();
        GenerateMesh();
    }

    void DrawLinearCurve()
    {
        SetCurvePositions();
        //line.SetPositions(final_positions);
    }


    //premiere methode qui permet de placer la position des points sur la courbe de Bezier, à utiliser si on veut dessiner une courbe avec un lineRenderer
    void SetCurvePositions()
    {
        for (int i = 0; i < numPoints; i++)
        {
            float ratio = (float)i / (float)numPoints;
            L1_positions[i] = new Vector3(Mathf.Lerp(pointA.position.x, pointB.position.x, ratio), Mathf.Lerp(pointA.position.y, pointB.position.y, ratio), Mathf.Lerp(pointA.position.z, pointB.position.z, ratio));
            L2_positions[i] = new Vector3(Mathf.Lerp(pointB.position.x, pointC.position.x, ratio), Mathf.Lerp(pointB.position.y, pointC.position.y, ratio), Mathf.Lerp(pointB.position.z, pointC.position.z, ratio));
            L3_positions[i] = new Vector3(Mathf.Lerp(pointC.position.x, pointD.position.x, ratio), Mathf.Lerp(pointC.position.y, pointD.position.y, ratio), Mathf.Lerp(pointC.position.z, pointD.position.z, ratio));

            Q1_positions[i] = new Vector3(Mathf.Lerp(L1_positions[i].x, L2_positions[i].x, ratio), Mathf.Lerp(L1_positions[i].y, L2_positions[i].y, ratio), Mathf.Lerp(L1_positions[i].z, L2_positions[i].z, ratio));
            Q2_positions[i] = new Vector3(Mathf.Lerp(L2_positions[i].x, L3_positions[i].x, ratio), Mathf.Lerp(L2_positions[i].y, L3_positions[i].y, ratio), Mathf.Lerp(L2_positions[i].z, L3_positions[i].z, ratio));

            final_positions[i] = new Vector3(Mathf.Lerp(Q1_positions[i].x, Q2_positions[i].x, ratio), Mathf.Lerp(Q1_positions[i].y, Q2_positions[i].y, ratio), Mathf.Lerp(Q1_positions[i].z, Q2_positions[i].z, ratio));
        }
    }

    OrientedPoint GetCurvePosAndTangentWithEquation(float ratio)
    {
        Vector3 A = pointA.position;
        Vector3 B = pointB.position;
        Vector3 C = pointC.position;
        Vector3 D = pointD.position;

        //Equation de la courbe de Bezier Cubique
        Vector3 cubicBezier = A * (-ratio * ratio * ratio + 3 * ratio * ratio - 3 * ratio + 1) +
                              B * (3 * ratio * ratio * ratio - 6 * ratio * ratio + 3 * ratio) +
                              C * (-3 * ratio * ratio * ratio + 3 * ratio * ratio) +
                              D * (ratio * ratio * ratio);

        //derivée de l'équation precedente
        Vector3 tangent = A * (-3 * ratio * ratio + 6 * ratio - 3) +
                          B * (9 * ratio * ratio - 12 * ratio + 3) +
                          C * (-9 * ratio * ratio + 6 * ratio) +
                          D * (3 * ratio * ratio);

        tangent = tangent.normalized;

        return new OrientedPoint(cubicBezier, tangent);
    }

    Vector3 GetCurveTangent(float ratio)
    {
        Vector3 A = pointA.position;
        Vector3 B = pointB.position;
        Vector3 C = pointC.position;
        Vector3 D = pointD.position;

        //derivée de l'équation precedente
        Vector3 tangent = A * (-3 * ratio * ratio + 6 * ratio - 3) +
                          B * (9 * ratio * ratio - 12 * ratio + 3) +
                          C * (-9 * ratio * ratio + 6 * ratio) +
                          D * (3 * ratio * ratio);

        

        return tangent.normalized;
    }



    private void OnDrawGizmos()
    {
        //pointA.position = pointA.position + transform.position
        //pointB.position = pointB.position + transform.position
        //pointD.position = pointC.position + transform.position
        //pointD.position = pointD.position + transform.position

        Handles.DrawBezier(pointA.position + transform.position, pointD.position + transform.position, pointB.position + transform.position, pointC.position + transform.position, Color.blue, null, 5f);
        Handles.DrawBezier(pointA.position + transform.position, pointB.position + transform.position, pointA.position + transform.position, pointB.position + transform.position, Color.white, null, 3f);
        Handles.DrawBezier(pointD.position + transform.position, pointC.position + transform.position, pointD.position + transform.position, pointC.position + transform.position, Color.white, null, 3f);

        //Handles.DrawBezier(pointA.position, pointD.position, pointB.position, pointC.position,Color.blue,null,5f);
        //Handles.DrawBezier(pointA.position, pointB.position, pointA.position, pointB.position, Color.white, null, 3f);
        //Handles.DrawBezier(pointD.position, pointC.position, pointD.position, pointC.position, Color.white, null, 3f);

        //OrientedPoint CurvePointsPosAndOrient = GetCurvePosAndTangentWithEquation(t_ratio);
        //Gizmos.DrawSphere(CurvePointsPosAndOrient.position, 0.2f);

        //void DrawPoint(Vector2 localPos) => Gizmos.DrawSphere(CurvePointsPosAndOrient.LocalToWorldPos(localPos), 0.1f);

        *//*Vector3[] localVerts = shape2D.vertices.Select(v => CurvePointsPosAndOrient.LocalToWorldPos(v.point)).ToArray();

        for(int i = 0; i<shape2D.lineIndices.Length; i+=2)
        {
            Vector3 a = localVerts[shape2D.lineIndices[i]];
            Vector3 b = localVerts[shape2D.lineIndices[i+1]];

            Gizmos.DrawLine(a, b);

        }*//*

        //Handles.PositionHandle(CurvePointsPosAndOrient.position,CurvePointsPosAndOrient.orientation);
        
    }

    void GenerateMesh()
    {
        

        mesh.Clear();
        int vertexIndex = 0;

        Vector3[] vertex = new Vector3[(segment+1)*(shape2D.vertices.Length+1)];
        Vector3[] normals = new Vector3[vertex.Length];
        Vector2[] uv = new Vector2[vertex.Length];

        //Vertices
        for (int i = 0; i < segment+1; i++)
        {

            float t = i / (segment - 1f);

            OrientedPoint op = GetCurvePosAndTangentWithEquation(t);
            for (int j = 0; j < shape2D.vertices.Length; j++)
            {
                vertex[vertexIndex] = op.LocalToWorldPos(shape2D.vertices[j].point);
                normals[vertexIndex] = op.LocalToWorldVect(shape2D.vertices[j].normal);
                uv[vertexIndex] = new Vector2(shape2D.vertices[j].Us, t);
                vertexIndex += 1;
            }
        }


        //Triangles
        int[] triangles = new int[6*segment*shape2D.vertices.Length];
        int triIndex = 0;


        for (int i = 0; i < segment - 1; i++)
        {
            int rootIndex = i * shape2D.vertices.Length;
            int rootIndexNext = (i+1) * shape2D.vertices.Length;


            for (int line = 0; line < shape2D.lineIndices.Length; line += 2)
            {
                int lineIndexA = shape2D.lineIndices[line];
                int lineIndexB = shape2D.lineIndices[line+1];

                int currentA = rootIndex + lineIndexA;
                int currentB = rootIndex + lineIndexB;
                int nextA = rootIndexNext + lineIndexA;
                int nextB = rootIndexNext + lineIndexB;

                triangles[triIndex] = currentA;
                triangles[triIndex + 1] = nextA;
                triangles[triIndex + 2] = nextB;

                triangles[triIndex + 3] = currentA;
                triangles[triIndex + 4] = nextB;
                triangles[triIndex + 5] = currentB;

                triIndex += 6;

            }



        }

        mesh.vertices = vertex;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;

    }

}
*/