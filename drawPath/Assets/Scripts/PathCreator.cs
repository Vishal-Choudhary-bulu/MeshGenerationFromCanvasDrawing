using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class PathCreator : MonoBehaviour
{
    public static PathCreator path;

    [Range(1,10)]
    public int pathResolution = 2;

    public float PathScale = 1f;

    public Material pathMaterial;

    private MeshRenderer meshRenderer;

    private MeshFilter meshFilter;

    private MeshCollider meshCollider;

    private List<PathPoint> points = new List<PathPoint>();

    private List<Vector3> meshVertices = new List<Vector3>();

    private Vector2[] uvs;

    private List<int> meshTris = new List<int>();


    void Awake()
    {
        path = this;
    }

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();
    }

    public void AddToPoints(PathPoint newPoint)
    {
        points.Add(newPoint);
    }

    public void CalculatePoints()
    {
        if(points.Count < 2)
        {
            return;
        }

        //new list of angles 
        var angles = new List<float>();
        //individual angle
        var angle = 0;
        //counter
        int x = 1;

        //until it circles back 
        while(angle < 360)
        {
            angle = 90 * x / pathResolution;
            angles.Add(angle);
            x++;
        }

        foreach(PathPoint point in points)
        {
            Vector3 pos = point.Position;
            Vector3 dir = GetDirAtPoint(point);

            foreach(var a in angles)
            {
                Vector3 newPos = pos + dir * Mathf.Cos(Mathf.Deg2Rad* a) + Vector3.up * Mathf.Sin(Mathf.Deg2Rad * a);
                meshVertices.Add(newPos);
            }
        }

        CalculateTris();
    }

    private void CalculateTris()
    {
        for(int i = 0; i < points.Count; i++)
        {
            for(int j = 1; j < pathResolution * 4; j++)
            {
                var v1 = i * pathResolution + j;
                var v2 = (i + 1) * pathResolution + j;
                var v3 = (i + 1) * pathResolution + j + 1;
                var v4 = i * pathResolution + j + 1;

                meshTris.Add(v1);
                meshTris.Add(v3);
                meshTris.Add(v2);
                meshTris.Add(v1);
                meshTris.Add(v4);
                meshTris.Add(v3);
            }

            var v5 = pathResolution * i + pathResolution;//4
            var v6 = pathResolution * i + 1;//1
            var v7 = pathResolution * (i + 1) + 1;//5
            var v8 = pathResolution * (i+1) + pathResolution;//8

            meshTris.Add(v5);
            meshTris.Add(v7);
            meshTris.Add(v8);
            meshTris.Add(v5);
            meshTris.Add(v6);
            meshTris.Add(v7);
        }

        MakePath();
    }

    private void OnDrawGizmos()
    {
        if (meshVertices.Count == 0)
        {
            return;
        }
        foreach (var v in meshVertices)
        {
            Gizmos.DrawSphere(v, 0.01f);
        }
    }


    private Vector3 GetDirAtPoint(PathPoint point)
    {
        int i = points.IndexOf(point);
        if(i == points.Count - 1)
        {
            i -= 1;
        }
        return Quaternion.AngleAxis(90, transform.up) * (points[i+1].Position - points[i].Position).normalized;
    }


    public void MakePath()
    {
        uvs = new Vector2[meshVertices.Count];
        Mesh pathMesh = new Mesh();
        pathMesh.vertices = meshVertices.ToArray();
        pathMesh.triangles = meshTris.ToArray();
        pathMesh.uv = uvs;
        pathMesh.RecalculateNormals();
        meshFilter.mesh = pathMesh;
        meshRenderer.sharedMaterial = pathMaterial;
        meshCollider.sharedMesh = pathMesh;
    }

    public void DeletePath()
    {
        points.Clear();
        meshVertices.Clear();
        meshTris.Clear();
        uvs = null;
    }

}
