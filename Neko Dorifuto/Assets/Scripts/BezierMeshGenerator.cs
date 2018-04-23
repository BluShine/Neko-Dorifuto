using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BezierMeshGenerator : MonoBehaviour
{
    public List<Vector3> crossSection; //the vertices that are used to extrude a mesh.
    public List<int> capTris; //the triangle indices for the caps at the two ends of the curve.
    public List<float> crossUVs;
    public float uvLengthScale = 10;

    public void GenerateMesh()
    {
        BezierCurve curve = GetComponent<BezierCurve>();
        Mesh m = curve.extrudeMesh(crossSection, capTris, crossUVs, uvLengthScale);
        MeshFilter mFilter = GetComponent<MeshFilter>();
        mFilter.mesh = m;
        MeshCollider mCol = GetComponent<MeshCollider>();
        mCol.sharedMesh = m;
    }

    public void AutoUVs()
    {
        crossUVs = new List<float>();
        float totalUvLength = 0;
        for (int i = 0; i < crossSection.Count; i++)
        {
            Vector2 current = crossSection[i];
            Vector2 next;
            if (i == crossSection.Count - 1)
                next = crossSection[0];
            else
                next = crossSection[i + 1];
            crossUVs.Add(Vector2.Distance(current, next));
            totalUvLength += Vector2.Distance(current, next);
        }
        float uvAccumulator = 0;
        for (int i = 0; i < crossUVs.Count; i++)
        {
            crossUVs[i] = uvAccumulator + crossUVs[i] / totalUvLength;
            uvAccumulator = crossUVs[i];
        }
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(BezierMeshGenerator))]
public class BezierMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        BezierMeshGenerator myScript = (BezierMeshGenerator)target;
        if(GUILayout.Button("Generate Mesh"))
        {
            myScript.GenerateMesh();
        }
        if (GUILayout.Button("Auto UVs"))
        {
            myScript.AutoUVs();
        }
    }
}
#endif