using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class PathMaker : MonoBehaviour
{
    public static PathMaker path;

    public int pathResolution = 2;

    public float curveScale = 2f;

    public Material pathMaterial;

    private MeshRenderer meshRenderer;

    private MeshFilter meshFilter;

    private MeshCollider meshCollider;

    [HideInInspector]
    public List<PathPoint> mainPath = new List<PathPoint>();

    private List<Vector3> meshVertices = new List<Vector3>();

    private List<Vector2> meshUVs = new List<Vector2>();

    private List<int> meshTris = new List<int>();

    //start point of the path
    [HideInInspector]
    public Vector3 startPoint;

    //direction of path
    private Vector3 dir;

    void Awake()
    {
        path = this;
    }

    private void Start()
    {

        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void CalculatePoints()
    {
        if(mainPath.Count < 2)
        {
            return;
        }   

        for(int i = -pathResolution; i < pathResolution + 1; i++)
        {
            for (int a = 0; a < mainPath.Count; a++)
            {
                var p1 = mainPath[a].Position;
                if (a < mainPath.Count - 1)
                {
                    var p2 = mainPath[a + 1].Position;
                    dir = Quaternion.AngleAxis(90, transform.up) * (p2 - p1).normalized;
                }

                var point = p1 + i * curveScale * dir;
                point += Mathf.Abs(i) * curveScale * Vector3.up;
                meshVertices.Add(point);
            }
        }
        CreateTris();
        MakePath();
    }

    void CreateTris()
    {   
        for(int a = 0; a < 2 * pathResolution; a++)
        {
            for(int b = 0; b < mainPath.Count-1; b++)
            {
                meshTris.Add(a * mainPath.Count + b);
                meshTris.Add((a + 1) * mainPath.Count + b + 1);
                meshTris.Add((a + 1) * mainPath.Count + b);

                meshTris.Add((a + 1) * mainPath.Count + b + 1);
                meshTris.Add(a * mainPath.Count + b);
                meshTris.Add(a * mainPath.Count + b + 1);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(meshVertices.Count == 0)
        {
            return;
        }
        foreach(var v in meshVertices)
        {
            Gizmos.DrawSphere(v, 0.01f);
        }
    }

    public void MakePath()
    {
        Mesh pathMesh = new Mesh();
        pathMesh.vertices = meshVertices.ToArray();
        pathMesh.triangles = meshTris.ToArray();
        pathMesh.RecalculateNormals();
        meshFilter.mesh = pathMesh;
        meshRenderer.sharedMaterial = pathMaterial;
        meshCollider.sharedMesh = pathMesh;
    }
    
    public void DeletePath()
    {
        mainPath.Clear();
        meshVertices.Clear();
        meshTris.Clear();
        meshUVs.Clear();
    }
}
