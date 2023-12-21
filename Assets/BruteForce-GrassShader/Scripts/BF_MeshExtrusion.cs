using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BF_MeshExtrusion : MonoBehaviour
{
    public Mesh originalMesh;

    public float offsetValue = 1f;
    public Vector3 offsetVector = Vector3.zero;
    public int numberOfStacks = 1;
    private readonly List<Color> cols = new();

    private readonly List<int> triangles = new();
    private readonly List<Vector2> uvs = new();
    private readonly List<Vector3> vertexs = new();
    private MeshFilter meshFilter;
    private int numberOfStacksMem = 1;
    private float offsetValueMem = 1f;
    private Vector3 offsetVectorMem = Vector3.zero;
    private Vector3[] oldNorm;
    private int[] oldTri;
    private Vector2[] oldUV;
    private Vector3[] oldVert;

    private void Awake()
    {
        CheckValues();
        BuildGeometry();
    }

    private void Update()
    {
        if (offsetValueMem != offsetValue || numberOfStacks != numberOfStacksMem || offsetVectorMem != offsetVector)
        {
            ClearGeometry();
            BuildGeometry();
            offsetValueMem = offsetValue;
            offsetVectorMem = offsetVector;
            numberOfStacksMem = numberOfStacks;
        }
    }

    private void OnEnable()
    {
        CheckValues();
    }

    private void CheckValues()
    {
        offsetValueMem = offsetValue;
        offsetVectorMem = offsetVector;
        numberOfStacksMem = numberOfStacks;
        meshFilter = gameObject.GetComponent<MeshFilter>();
        oldTri = originalMesh.triangles;
        oldVert = originalMesh.vertices;
        oldNorm = originalMesh.normals;
        oldUV = originalMesh.uv;
    }

    private void ClearGeometry()
    {
        triangles.Clear();
        triangles.TrimExcess();
        vertexs.Clear();
        vertexs.TrimExcess();
        uvs.Clear();
        uvs.TrimExcess();
        cols.Clear();
        cols.TrimExcess();
    }

    private void BuildGeometry()
    {
        if (meshFilter == null) meshFilter = gameObject.GetComponent<MeshFilter>();
        Mesh mesh = new();
        meshFilter.mesh = mesh;

        int faces = Mathf.Min(numberOfStacks, 100);
        for (int i = 0; i < faces; i++)
        {
            int triangleOffset = i * oldVert.Length;
            int indexNewV = 0;
            foreach (Vector3 v in oldVert)
            {
                vertexs.Add(v + oldNorm[indexNewV] * offsetValue * 0.01f * i + offsetVectorMem * 0.01f * i);
                uvs.Add(oldUV[indexNewV]);
                cols.Add(new Color(1 * (i / (float)(faces - 1)), 1 * (i / (float)(faces - 1)),
                    1 * (i / (float)(faces - 1))));

                indexNewV++;
            }

            indexNewV = 0;
            foreach (int innt in oldTri)
            {
                triangles.Add(oldTri[indexNewV] + triangleOffset);

                indexNewV++;
            }
        }

        mesh.vertices = vertexs.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.colors = cols.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
    }
}
